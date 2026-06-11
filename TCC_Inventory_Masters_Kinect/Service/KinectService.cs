using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Kinect;
using TCC_Inventory_Masters_Kinect.Logs;

namespace TCC_Inventory_Masters_Kinect.Service
{
    public class KinectService
    {
        private KinectSensor _sensor;

       
        private const int ANGULO_MIN = -27;
        private const int ANGULO_MAX = 27;
        private const int PASSO_ANGULO = 5;    
        private const int FRAMES_POR_ANGULO = 5;    
        private const int ESPERA_MOTOR_MS = 1500; 
        private const int DEPTH_MAX_MM = 4000; 

        public bool IsConnected =>
            _sensor != null && _sensor.Status == KinectStatus.Connected;

        public KinectService()
        {
            if (KinectSensor.KinectSensors.Count > 0)
                _sensor = KinectSensor.KinectSensors[0];
        }

       
        public void Start()
        {
            if (_sensor == null)
                throw new Exception("Nenhum Kinect foi encontrado no computador.");

            if (_sensor.Status != KinectStatus.Connected)
                throw new Exception("Kinect v1 nao esta conectado.");

            _sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
            _sensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
            _sensor.SkeletonStream.Enable();
            _sensor.Start();
        }

        public void Stop()
        {
            _sensor?.Stop();
        }

     
        public BitmapSource CapturarFrameCamera()
        {
            if (!IsConnected || _sensor.ColorStream == null)
                return null;

            try
            {
                using (var frame = _sensor.ColorStream.OpenNextFrame(1000))
                {
                    if (frame == null) return null;

                    byte[] pixelData = new byte[frame.PixelDataLength];
                    frame.CopyPixelDataTo(pixelData);

                    return BitmapSource.Create(
                        frame.Width, frame.Height,
                        96, 96,
                        PixelFormats.Bgr32,
                        null,
                        pixelData,
                        frame.Width * frame.BytesPerPixel
                    );
                }
            }
            catch (Exception ex)
            {
                LoggerService.Erro("Erro ao capturar camera", ex);
                return null;
            }
        }


        public BitmapSource CapturarDepthColorido()
        {
            if (!IsConnected) return null;

            try
            {
                using (var frame = _sensor.DepthStream.OpenNextFrame(1000))
                {
                    if (frame == null) return null;

                    short[] depthData = new short[frame.PixelDataLength];
                    frame.CopyPixelDataTo(depthData);

                    byte[] pixels = new byte[frame.Width * frame.Height * 4];
                    int index = 0;

                    for (int i = 0; i < depthData.Length; i++)
                    {
                        int depth = depthData[i] >> 3;
                        byte intensity = (depth == 0)
                            ? (byte)0
                            : (byte)(255 - Math.Min(255, depth * 255 / DEPTH_MAX_MM));

                        pixels[index++] = intensity;
                        pixels[index++] = (byte)(intensity * 0.7);
                        pixels[index++] = (byte)(intensity * 0.4);
                        pixels[index++] = 255;
                    }

                    return BitmapSource.Create(
                        frame.Width, frame.Height,
                        96, 96,
                        PixelFormats.Bgra32,
                        null,
                        pixels,
                        frame.Width * 4
                    );
                }
            }
            catch (Exception ex)
            {
                LoggerService.Erro("Erro ao capturar depth colorido", ex);
                return null;
            }
        }


        public async Task<double> MeasureCurrentVolumeAsync(CancellationToken token)
        {
            if (!IsConnected) return 0;

            try
            {
                using (var frame = _sensor.DepthStream.OpenNextFrame(1000))
                {
                    if (frame == null) return 0;

                    short[] depthData = new short[frame.PixelDataLength];
                    frame.CopyPixelDataTo(depthData);

                    double soma = 0;
                    int validos = 0;

                    for (int i = 0; i < depthData.Length; i++)
                    {
                        int depth = depthData[i] >> 3;
                        if (depth > 0 && depth < DEPTH_MAX_MM)
                        {
                            soma += depth;
                            validos++;
                        }
                    }

                    if (validos == 0) return 0;

                    return (soma / validos) * 100;
                }
            }
            catch (Exception ex)
            {
                LoggerService.Erro("Erro ao medir volume", ex);
                return 0;
            }
        }


        public async Task<CalibrationResult> CalibrateAsync(
            CancellationToken token,
            IProgress<CalibrationProgress> progress = null)
        {
            if (!IsConnected)
                throw new Exception("Kinect nao esta conectado para calibrar.");

            LoggerService.Info("Iniciando calibracao com motor de inclinacao.");

            int anguloOriginal = _sensor.ElevationAngle;
            var leiturasPorAngulo = new List<(int Angulo, double MediaDepth, int Pontos)>();

            try
            {
         
                int totalPassos = ((ANGULO_MAX - ANGULO_MIN) / PASSO_ANGULO) + 1;
                int passoAtual = 0;

                for (int angulo = ANGULO_MIN; angulo <= ANGULO_MAX; angulo += PASSO_ANGULO)
                {
                    token.ThrowIfCancellationRequested();

                    await MoverMotorAsync(angulo, token, progress);

                    progress?.Report(new CalibrationProgress
                    {
                        Status = $"Estabilizando em {angulo} graus...",
                        Percentual = (int)((passoAtual / (double)totalPassos) * 80)
                    });

                    await Task.Delay(ESPERA_MOTOR_MS, token);

                  
                    var (mediaDepth, totalPontos) =
                        await CapturarMediaDepthAsync(FRAMES_POR_ANGULO, token);
          

                    leiturasPorAngulo.Add((angulo, mediaDepth, totalPontos));

                    LoggerService.Info(
                        $"Angulo {angulo} graus | " +
                        $"Media depth: {mediaDepth:F1} mm | " +
                        $"Pontos: {totalPontos}");

                    passoAtual++;
                    progress?.Report(new CalibrationProgress
                    {
                        Status = $"Capturado angulo {angulo} graus ({passoAtual}/{totalPassos})",
                        Percentual = (int)((passoAtual / (double)totalPassos) * 80)
                    });
                }

             
                progress?.Report(new CalibrationProgress
                {
                    Status = "Calculando plano do chao...",
                    Percentual = 85
                });

                var resultadoChao = DetectarChao(leiturasPorAngulo);

              
                progress?.Report(new CalibrationProgress
                {
                    Status = "Calculando volume maximo...",
                    Percentual = 92
                });

                double volumeMaximo = CalcularVolumeMaximo(resultadoChao.DistanciaChaoMm);

              
                progress?.Report(new CalibrationProgress
                {
                    Status = "Restaurando posicao do Kinect...",
                    Percentual = 96
                });

                await MoverMotorAsync(anguloOriginal, token, null);
                await Task.Delay(ESPERA_MOTOR_MS, token);

                progress?.Report(new CalibrationProgress
                {
                    Status = "Calibracao concluida!",
                    Percentual = 100
                });

                LoggerService.Info(
                    $"Calibracao concluida | " +
                    $"Chao em: {resultadoChao.DistanciaChaoMm:F1} mm | " +
                    $"Volume: {volumeMaximo:F0} cm3");

                return new CalibrationResult
                {
                    MaxVolume = volumeMaximo,
                    TotalPointsFound = resultadoChao.TotalPontos,
                    DistanciaChaoMm = resultadoChao.DistanciaChaoMm,
                    AnguloChao = resultadoChao.AnguloDetectado
                };
            }
            catch (OperationCanceledException)
            {
                LoggerService.LogWarning("Calibracao cancelada pelo usuario.");
                try { await MoverMotorAsync(anguloOriginal, CancellationToken.None, null); } catch { }
                throw;
            }
            catch (Exception ex)
            {
                LoggerService.Erro("Erro durante a calibracao", ex);
                try { await MoverMotorAsync(anguloOriginal, CancellationToken.None, null); } catch { }
                throw;
            }
        }

        
        private async Task MoverMotorAsync(
            int angulo,
            CancellationToken token,
            IProgress<CalibrationProgress> progress)
        {
            int anguloSeguro = Math.Max(ANGULO_MIN, Math.Min(ANGULO_MAX, angulo));

            try
            {
                _sensor.ElevationAngle = anguloSeguro;

                progress?.Report(new CalibrationProgress
                {
                    Status = $"Movendo motor para {anguloSeguro} graus..."
                });

                await Task.Delay(300, token);
            }
            catch (InvalidOperationException ex)
            {
                
                LoggerService.LogWarning(
                    $"Motor ocupado, aguardando 2s. Erro: {ex.Message}");

                await Task.Delay(2000, token);

                try
                {
                    _sensor.ElevationAngle = anguloSeguro;
                }
                catch (Exception ex2)
                {
                    LoggerService.Erro($"Falha ao mover motor para {anguloSeguro} graus", ex2);
                }
            }
        }

  
        private async Task<(double MediaDepth, int TotalPontos)> CapturarMediaDepthAsync(
            int quantidadeFrames,
            CancellationToken token)
        {
            double somaTotal = 0;
            int pontosTotal = 0;
            int framesValidos = 0;

            for (int f = 0; f < quantidadeFrames; f++)
            {
                token.ThrowIfCancellationRequested();

                try
                {
                    using (var frame = _sensor.DepthStream.OpenNextFrame(500))
                    {
                        if (frame == null) continue;

                        short[] depthData = new short[frame.PixelDataLength];
                        frame.CopyPixelDataTo(depthData);

                        for (int i = 0; i < depthData.Length; i++)
                        {
                            int depth = depthData[i] >> 3;
                            if (depth > 300 && depth < DEPTH_MAX_MM)
                            {
                                somaTotal += depth;
                                pontosTotal++;
                            }
                        }

                        framesValidos++;
                    }
                }
                catch {  }

                await Task.Delay(30, token);
            }

            double media = (framesValidos > 0 && pontosTotal > 0)
                ? somaTotal / pontosTotal
                : 0;

            return (media, pontosTotal);
        }

    
        private (double DistanciaChaoMm, int AnguloDetectado, int TotalPontos) DetectarChao(
            List<(int Angulo, double MediaDepth, int Pontos)> leituras)
        {
            double menorMedia = double.MaxValue;
            int anguloChao = 0;
            int pontosChao = 0;

            foreach (var leitura in leituras)
            {
                if (leitura.MediaDepth > 0 && leitura.MediaDepth < menorMedia)
                {
                    menorMedia = leitura.MediaDepth;
                    anguloChao = leitura.Angulo;
                    pontosChao = leitura.Pontos;
                }
            }

            double anguloRad = Math.Abs(anguloChao) * Math.PI / 180.0;
            double distanciaChaoReal = menorMedia * Math.Cos(anguloRad);

            LoggerService.Info(
                $"Chao detectado | " +
                $"Angulo: {anguloChao} graus | " +
                $"Distancia real: {distanciaChaoReal:F1} mm");

            return (distanciaChaoReal, anguloChao, pontosChao);
        }

        
        private double CalcularVolumeMaximo(double distanciaChaoMm)
        {
            if (distanciaChaoMm <= 0) return 0;

          
            double fovH = 57.0 * Math.PI / 180.0;
            double fovV = 43.0 * Math.PI / 180.0;

            double largura = 2 * distanciaChaoMm * Math.Tan(fovH / 2);
            double profundidade = 2 * distanciaChaoMm * Math.Tan(fovV / 2);
            double altura = distanciaChaoMm;

           
            return (largura * profundidade * altura) / 1000.0;
        }
    }

    
    public class CalibrationResult
    {
        public double MaxVolume { get; set; }
        public int TotalPointsFound { get; set; }
        public double DistanciaChaoMm { get; set; }
        public int AnguloChao { get; set; }
    }

    public class CalibrationProgress
    {
        public string Status { get; set; }
        public int Percentual { get; set; }
    }
}
