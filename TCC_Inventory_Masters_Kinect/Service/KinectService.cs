using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Kinect;
using TCC_Inventory_Masters_Kinect.Logs;
using TCC_Inventory_Masters_Kinect.Model;

namespace TCC_Inventory_Masters_Kinect.Service
{
    public class KinectService
    {
        private KinectSensor _sensor;
        private short[] _depthCalibrado;
        private int _larguraDepth;
        private int _alturaDepth;

        private const int ANGULO_MIN = -27;
        private const int ANGULO_MAX = 27;
        private const int PASSO_ANGULO = 5;
        private const int FRAMES_POR_ANGULO = 5;
        private const int ESPERA_MOTOR_MS = 1500;
        private const int DEPTH_MAX_MM = 4000;
        private const int DEPTH_MIN_MM = 500;
        private const int LIMITE_MINIMO_ALTURA_MM = 30;

        public event Action<BitmapSource> CameraFrameAtualizado;

        public bool IsConnected =>
            _sensor != null && _sensor.Status == KinectStatus.Connected;

        public KinectService()
        {
            if (KinectSensor.KinectSensors.Count > 0)
            {
                _sensor = KinectSensor.KinectSensors[0];
            }
        }

        public void Start()
        {
            if (_sensor == null)
            {
                LoggerService.Erro("Nenhum Kinect foi encontrado no computador.");
                throw new InvalidOperationException("Nenhum Kinect foi encontrado no computador.");
            }

            if (_sensor.Status != KinectStatus.Connected)
            {
                LoggerService.Erro("Kinect v1 nao esta conectado.");
                throw new InvalidOperationException("Kinect v1 nao esta conectado.");
            }

            _sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
            _sensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
            _sensor.ColorFrameReady += Sensor_ColorFrameReady;
            _sensor.Start();

            LoggerService.Info("Kinect iniciado com camera RGB e profundidade.");
        }

        public void Stop()
        {
            if (_sensor != null)
            {
                _sensor.ColorFrameReady -= Sensor_ColorFrameReady;
                _sensor.Stop();
                LoggerService.Info("Kinect finalizado.");
            }
        }

        private void Sensor_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            try
            {
                using (var frame = e.OpenColorImageFrame())
                {
                    if (frame == null)
                    {
                        return;
                    }

                    byte[] pixelData = new byte[frame.PixelDataLength];
                    frame.CopyPixelDataTo(pixelData);

                    var imagem = BitmapSource.Create(
                        frame.Width,
                        frame.Height,
                        96,
                        96,
                        PixelFormats.Bgr32,
                        null,
                        pixelData,
                        frame.Width * frame.BytesPerPixel
                    );

                    imagem.Freeze();
                    CameraFrameAtualizado?.Invoke(imagem);
                }
            }
            catch
            {
                LoggerService.Erro("Erro ao atualizar camera RGB em tempo real.");
            }
        }

        public BitmapSource CapturarFrameCamera()
        {
            if (!IsConnected || _sensor.ColorStream == null)
            {
                return null;
            }

            try
            {
                using (var frame = _sensor.ColorStream.OpenNextFrame(1000))
                {
                    if (frame == null)
                    {
                        return null;
                    }

                    byte[] pixelData = new byte[frame.PixelDataLength];
                    frame.CopyPixelDataTo(pixelData);

                    var imagem = BitmapSource.Create(
                        frame.Width,
                        frame.Height,
                        96,
                        96,
                        PixelFormats.Bgr32,
                        null,
                        pixelData,
                        frame.Width * frame.BytesPerPixel
                    );

                    imagem.Freeze();
                    return imagem;
                }
            }
            catch
            {
                LoggerService.Erro("Erro ao capturar camera RGB.");
                return null;
            }
        }

        public BitmapSource CapturarDepthColorido()
        {
            if (!IsConnected)
            {
                return null;
            }

            try
            {
                using (var frame = _sensor.DepthStream.OpenNextFrame(1000))
                {
                    if (frame == null)
                    {
                        return null;
                    }

                    short[] depthData = new short[frame.PixelDataLength];
                    frame.CopyPixelDataTo(depthData);

                    byte[] pixels = new byte[frame.Width * frame.Height * 4];
                    int index = 0;

                    for (int i = 0; i < depthData.Length; i++)
                    {
                        int depth = depthData[i] >> 3;

                        byte intensity = depth == 0
                            ? (byte)0
                            : (byte)(255 - Math.Min(255, depth * 255 / DEPTH_MAX_MM));

                        pixels[index++] = intensity;
                        pixels[index++] = (byte)(intensity * 0.7);
                        pixels[index++] = (byte)(intensity * 0.4);
                        pixels[index++] = 255;
                    }

                    var imagem = BitmapSource.Create(
                        frame.Width,
                        frame.Height,
                        96,
                        96,
                        PixelFormats.Bgra32,
                        null,
                        pixels,
                        frame.Width * 4
                    );

                    imagem.Freeze();
                    return imagem;
                }
            }
            catch
            {
                LoggerService.Erro("Erro ao capturar profundidade colorida.");
                return null;
            }
        }

        public async Task<CalibrationResult> CalibrateAsync(
            CancellationToken token,
            IProgress<CalibrationProgress> progress = null)
        {
            if (!IsConnected)
            {
                LoggerService.Erro("Kinect nao esta conectado para calibrar.");
                throw new InvalidOperationException("Kinect nao esta conectado para calibrar.");
            }

            LoggerService.Info("Iniciando calibracao volumetrica.");

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
                        Percentage = (int)((passoAtual / (double)totalPassos) * 70)
                    });

                    await Task.Delay(ESPERA_MOTOR_MS, token);

                    var leitura = await CapturarMediaDepthAsync(FRAMES_POR_ANGULO, token);

                    leiturasPorAngulo.Add((angulo, leitura.MediaDepth, leitura.TotalPontos));

                    LoggerService.Info($"Angulo {angulo} graus | Media depth: {leitura.MediaDepth:F1} mm | Pontos: {leitura.TotalPontos}");

                    passoAtual++;

                    progress?.Report(new CalibrationProgress
                    {
                        Status = $"Capturado angulo {angulo} graus ({passoAtual}/{totalPassos})",
                        Percentage = (int)((passoAtual / (double)totalPassos) * 70)
                    });
                }

                progress?.Report(new CalibrationProgress
                {
                    Status = "Detectando plano de referencia...",
                    Percentage = 80
                });

                var resultadoChao = DetectarChao(leiturasPorAngulo);

                progress?.Report(new CalibrationProgress
                {
                    Status = "Capturando mapa volumetrico vazio...",
                    Percentage = 88
                });

                bool mapaCapturado = CapturarMapaDepthCalibrado();

                double volumeMaximo = mapaCapturado
                    ? CalcularVolumeReferenciaCm3(_depthCalibrado, _larguraDepth, _alturaDepth)
                    : 0;

                progress?.Report(new CalibrationProgress
                {
                    Status = "Restaurando posicao do Kinect...",
                    Percentage = 96
                });

                await MoverMotorAsync(anguloOriginal, token, null);
                await Task.Delay(ESPERA_MOTOR_MS, token);

                progress?.Report(new CalibrationProgress
                {
                    Status = "Calibracao concluida!",
                    Percentage = 100
                });

                LoggerService.Info($"Calibracao concluida | Chao em: {resultadoChao.DistanciaChaoMm:F1} mm | Volume referencia: {volumeMaximo:F0} cm3");

                return new CalibrationResult
                {
                    MaxVolume = volumeMaximo,
                    TotalPointsFound = resultadoChao.TotalPontos,
                    CalibratedAt = DateTime.Now
                };
            }
            catch (OperationCanceledException)
            {
                LoggerService.LogWarning("Calibracao cancelada pelo usuario.");

                try
                {
                    await MoverMotorAsync(anguloOriginal, CancellationToken.None, null);
                }
                catch
                {
                    LoggerService.Erro("Falha ao restaurar posicao do Kinect apos cancelamento.");
                }

                throw;
            }
            catch
            {
                LoggerService.Erro("Erro durante a calibracao.");

                try
                {
                    await MoverMotorAsync(anguloOriginal, CancellationToken.None, null);
                }
                catch
                {
                    LoggerService.Erro("Falha ao restaurar posicao do Kinect apos erro.");
                }

                throw;
            }
        }

        public double CalcularVolumeAtualCm3()
        {
            if (!IsConnected)
            {
                LoggerService.Erro("Kinect nao conectado para calcular volume atual.");
                return 0;
            }

            if (_depthCalibrado == null)
            {
                return 0;
            }

            try
            {
                using (var frame = _sensor.DepthStream.OpenNextFrame(1000))
                {
                    if (frame == null)
                    {
                        return 0;
                    }

                    short[] depthAtual = new short[frame.PixelDataLength];
                    frame.CopyPixelDataTo(depthAtual);

                    return CalcularVolumeRealCm3(
                        _depthCalibrado,
                        depthAtual,
                        frame.Width,
                        frame.Height
                    );
                }
            }
            catch
            {
                LoggerService.Erro("Erro ao calcular volume atual pelo Kinect.");
                return 0;
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
            catch
            {
                LoggerService.LogWarning($"Motor ocupado ou indisponivel ao mover para {anguloSeguro} graus.");

                await Task.Delay(2000, token);

                try
                {
                    _sensor.ElevationAngle = anguloSeguro;
                }
                catch
                {
                    LoggerService.Erro($"Falha ao mover motor para {anguloSeguro} graus.");
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
                        if (frame == null)
                        {
                            continue;
                        }

                        short[] depthData = new short[frame.PixelDataLength];
                        frame.CopyPixelDataTo(depthData);

                        for (int i = 0; i < depthData.Length; i++)
                        {
                            int depth = depthData[i] >> 3;

                            if (depth > DEPTH_MIN_MM && depth < DEPTH_MAX_MM)
                            {
                                somaTotal += depth;
                                pontosTotal++;
                            }
                        }

                        framesValidos++;
                    }
                }
                catch
                {
                    LoggerService.LogWarning("Falha ao capturar frame de profundidade durante calibracao.");
                }

                await Task.Delay(30, token);
            }

            double media = framesValidos > 0 && pontosTotal > 0
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

            if (menorMedia == double.MaxValue)
            {
                LoggerService.Erro("Nao foi possivel detectar o chao durante a calibracao.");
                return (0, 0, 0);
            }

            double anguloRad = Math.Abs(anguloChao) * Math.PI / 180.0;
            double distanciaChaoReal = menorMedia * Math.Cos(anguloRad);

            LoggerService.Info($"Chao detectado | Angulo: {anguloChao} graus | Distancia real: {distanciaChaoReal:F1} mm");

            return (distanciaChaoReal, anguloChao, pontosChao);
        }

        private bool CapturarMapaDepthCalibrado()
        {
            if (!IsConnected)
            {
                LoggerService.Erro("Kinect nao conectado para capturar mapa calibrado.");
                return false;
            }

            try
            {
                using (var frame = _sensor.DepthStream.OpenNextFrame(1000))
                {
                    if (frame == null)
                    {
                        LoggerService.Erro("Frame de profundidade calibrado nao capturado.");
                        return false;
                    }

                    _depthCalibrado = new short[frame.PixelDataLength];
                    frame.CopyPixelDataTo(_depthCalibrado);

                    _larguraDepth = frame.Width;
                    _alturaDepth = frame.Height;

                    LoggerService.Info("Mapa de profundidade calibrado capturado.");

                    return true;
                }
            }
            catch
            {
                LoggerService.Erro("Erro ao capturar mapa de profundidade calibrado.");
                return false;
            }
        }

        private double CalcularVolumeReferenciaCm3(short[] depthCalibrado, int largura, int altura)
        {
            if (depthCalibrado == null || largura <= 0 || altura <= 0)
            {
                LoggerService.Erro("Mapa calibrado invalido para calcular volume de referencia.");
                return 0;
            }

            double fovHorizontal = 57.0 * Math.PI / 180.0;
            double fovVertical = 43.0 * Math.PI / 180.0;
            double volumeTotalMm3 = 0;

            for (int i = 0; i < depthCalibrado.Length; i++)
            {
                int profundidadeMm = depthCalibrado[i] >> 3;

                if (profundidadeMm <= DEPTH_MIN_MM || profundidadeMm >= DEPTH_MAX_MM)
                {
                    continue;
                }

                double larguraPixelMm = (2 * profundidadeMm * Math.Tan(fovHorizontal / 2)) / largura;
                double alturaPixelMm = (2 * profundidadeMm * Math.Tan(fovVertical / 2)) / altura;

                volumeTotalMm3 += profundidadeMm * larguraPixelMm * alturaPixelMm;
            }

            return volumeTotalMm3 / 1000.0;
        }

        private double CalcularVolumeRealCm3(short[] depthCalibrado, short[] depthAtual, int largura, int altura)
        {
            if (depthCalibrado == null || depthAtual == null)
            {
                LoggerService.Erro("Mapa de profundidade invalido para calculo de volume.");
                return 0;
            }

            if (depthCalibrado.Length != depthAtual.Length)
            {
                LoggerService.Erro("Mapa calibrado e mapa atual possuem tamanhos diferentes.");
                return 0;
            }

            double fovHorizontal = 57.0 * Math.PI / 180.0;
            double fovVertical = 43.0 * Math.PI / 180.0;
            double volumeTotalMm3 = 0;

            for (int i = 0; i < depthAtual.Length; i++)
            {
                int profundidadeBaseMm = depthCalibrado[i] >> 3;
                int profundidadeAtualMm = depthAtual[i] >> 3;

                if (profundidadeBaseMm <= DEPTH_MIN_MM || profundidadeBaseMm >= DEPTH_MAX_MM)
                {
                    continue;
                }

                if (profundidadeAtualMm <= DEPTH_MIN_MM || profundidadeAtualMm >= DEPTH_MAX_MM)
                {
                    continue;
                }

                int diferencaMm = profundidadeBaseMm - profundidadeAtualMm;

                if (diferencaMm < LIMITE_MINIMO_ALTURA_MM)
                {
                    continue;
                }

                double larguraPixelMm = (2 * profundidadeBaseMm * Math.Tan(fovHorizontal / 2)) / largura;
                double alturaPixelMm = (2 * profundidadeBaseMm * Math.Tan(fovVertical / 2)) / altura;

                volumeTotalMm3 += diferencaMm * larguraPixelMm * alturaPixelMm;
            }

            return volumeTotalMm3 / 1000.0;
        }
    }
}