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
    /// <summary>
    /// Serviço responsável por controlar o Kinect v1, capturar imagem RGB,
    /// capturar profundidade, realizar calibração do ambiente e calcular volume em cm³.
    /// </summary>
    public class KinectService
    {
        private KinectSensor _sensor;
        private short[] _depthCalibrado;
        private int _larguraDepth;
        private int _alturaDepth;

        /// <summary>
        /// Indica se o ambiente já foi calibrado.
        /// Impede o cálculo de volume antes da captura do mapa de referência.
        /// </summary>
        private bool _calibrado = false;

        /// <summary>
        /// Último volume suavizado calculado.
        /// Usado para aplicar suavização ponderada entre leituras.
        /// </summary>
        private double _ultimoVolumeSuavizado = 0;

        /// <summary>
        /// Histórico dos últimos volumes calculados.
        /// Utilizado para suavizar oscilações naturais do sensor de profundidade.
        /// </summary>
        private readonly Queue<double> _historicoVolumes = new Queue<double>();

        /// <summary>
        /// Quantidade máxima de leituras usadas na média móvel do volume.
        /// </summary>
        private const int MAX_HISTORICO_VOLUME = 10;

        private const int ANGULO_MIN = -27;
        private const int ANGULO_MAX = 27;
        private const int PASSO_ANGULO = 5;
        private const int FRAMES_POR_ANGULO = 10;
        private const int ESPERA_MOTOR_MS = 1500;

        private const int DEPTH_MIN_MM = 1200;
        private const int DEPTH_MAX_MM = 3500;
        private const int ALTURA_MINIMA_OBJETO_MM = 30;
        private const int ALTURA_MAXIMA_OBJETO_MM = 1800;
        private const int PONTOS_MINIMOS_VOLUME = 1000;
        private const double FOV_HORIZONTAL_GRAUS = 57.0;
        private const double FOV_VERTICAL_GRAUS = 43.0;

        /// <summary>
        /// Peso usado na suavização ponderada.
        /// Quanto maior o valor, mais lenta e estável fica a alteração visual do volume.
        /// </summary>
        private const double PESO_SUAVIZACAO = 0.7;

        /// <summary>
        /// Evento disparado sempre que um novo frame da câmera RGB é capturado.
        /// </summary>
        public event Action<BitmapSource> CameraFrameAtualizado;

        /// <summary>
        /// Indica se o Kinect está conectado e disponível para uso.
        /// </summary>
        public bool IsConnected =>
            _sensor != null && _sensor.Status == KinectStatus.Connected;

        /// <summary>
        /// Inicializa o serviço tentando localizar o primeiro Kinect conectado ao computador.
        /// </summary>
        public KinectService()
        {
            if (KinectSensor.KinectSensors.Count > 0)
            {
                _sensor = KinectSensor.KinectSensors[0];
            }
        }

        /// <summary>
        /// Inicia o Kinect, habilitando a câmera RGB e o fluxo de profundidade.
        /// Também registra o evento de atualização da câmera em tempo real.
        /// </summary>
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

            _sensor.ColorFrameReady -= Sensor_ColorFrameReady;
            _sensor.ColorFrameReady += Sensor_ColorFrameReady;

            _sensor.Start();

            LoggerService.Info("Kinect iniciado com camera RGB e profundidade.");
        }

        /// <summary>
        /// Finaliza o Kinect e remove eventos ativos para evitar consumo desnecessário
        /// de recursos ou conflitos ao reiniciar o sensor.
        /// </summary>
        public void Stop()
        {
            if (_sensor != null)
            {
                _sensor.ColorFrameReady -= Sensor_ColorFrameReady;
                _sensor.Stop();
                LoggerService.Info("Kinect finalizado.");
            }
        }

        /// <summary>
        /// Evento interno responsável por capturar o frame RGB em tempo real
        /// e convertê-lo para BitmapSource para exibição na interface.
        /// </summary>
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

        /// <summary>
        /// Captura manualmente um frame da câmera RGB do Kinect.
        /// </summary>
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

        /// <summary>
        /// Captura o mapa de profundidade do Kinect e gera uma imagem colorida
        /// para visualização da distância dos objetos dentro da faixa válida.
        /// </summary>
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
                        byte intensity;

                        if (depth < DEPTH_MIN_MM || depth > DEPTH_MAX_MM)
                        {
                            intensity = 0;
                        }
                        else
                        {
                            double normalizado = (depth - DEPTH_MIN_MM) / (double)(DEPTH_MAX_MM - DEPTH_MIN_MM);
                            intensity = (byte)(255 - Math.Min(255, Math.Max(0, normalizado * 255)));
                        }

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

        /// <summary>
        /// Aplica média móvel e suavização ponderada ao volume atual.
        /// Reduz oscilações bruscas na exibição causadas pelo ruído natural do Kinect.
        /// </summary>
        private double EstabilizarVolume(double volumeAtual)
        {
            if (volumeAtual <= 0)
            {
                return 0;
            }

            _historicoVolumes.Enqueue(volumeAtual);

            while (_historicoVolumes.Count > MAX_HISTORICO_VOLUME)
            {
                _historicoVolumes.Dequeue();
            }

            double soma = 0;

            foreach (double volume in _historicoVolumes)
            {
                soma += volume;
            }

            double mediaHistorico = soma / _historicoVolumes.Count;

            double volumeSuavizado;

            if (_ultimoVolumeSuavizado <= 0)
            {
                volumeSuavizado = mediaHistorico;
            }
            else
            {
                volumeSuavizado =
                    (_ultimoVolumeSuavizado * PESO_SUAVIZACAO) +
                    (mediaHistorico * (1 - PESO_SUAVIZACAO));
            }

            _ultimoVolumeSuavizado = volumeSuavizado;

            return Math.Round(volumeSuavizado, 0);
        }

        /// <summary>
        /// Calcula o volume atual detectado pelo Kinect em centímetros cúbicos.
        /// O cálculo só é executado após a calibração do ambiente vazio.
        /// </summary>
        public double CalcularVolumeAtualCm3()
        {
            if (!IsConnected)
            {
                LoggerService.Erro("Kinect nao conectado para calcular volume atual.");
                return 0;
            }

            if (!_calibrado || _depthCalibrado == null)
            {
                LoggerService.LogWarning("Volume nao calculado: ambiente ainda nao calibrado.");
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

                    if (_depthCalibrado.Length != depthAtual.Length)
                    {
                        LoggerService.Erro("Mapa calibrado e frame atual possuem tamanhos diferentes.");
                        return 0;
                    }

                    double volumeCalculado = CalcularVolumeRealCm3(
                        _depthCalibrado,
                        depthAtual,
                        frame.Width,
                        frame.Height
                    );

                    double volumeEstabilizado = EstabilizarVolume(volumeCalculado);

                    LoggerService.Info($"Volume estabilizado exibido: {volumeEstabilizado:N0} cm3");

                    return volumeEstabilizado;
                }
            }
            catch
            {
                LoggerService.Erro("Erro ao calcular volume atual pelo Kinect.");
                return 0;
            }
        }

        /// <summary>
        /// Calcula o volume real do objeto comparando a profundidade calibrada
        /// do ambiente vazio com a profundidade atual capturada pelo Kinect.
        /// </summary>
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

            double fovHorizontal = FOV_HORIZONTAL_GRAUS * Math.PI / 180.0;
            double fovVertical = FOV_VERTICAL_GRAUS * Math.PI / 180.0;
            double volumeTotalMm3 = 0;
            int pontosValidos = 0;

            int margemX = largura / 10;
            int margemY = altura / 10;

            for (int y = margemY; y < altura - margemY; y++)
            {
                for (int x = margemX; x < largura - margemX; x++)
                {
                    int i = y * largura + x;

                    int profundidadeBaseMm = depthCalibrado[i] >> 3;
                    int profundidadeAtualMm = depthAtual[i] >> 3;

                    if (profundidadeBaseMm < DEPTH_MIN_MM || profundidadeBaseMm > DEPTH_MAX_MM)
                    {
                        continue;
                    }

                    if (profundidadeAtualMm < DEPTH_MIN_MM || profundidadeAtualMm > DEPTH_MAX_MM)
                    {
                        continue;
                    }

                    int alturaObjetoMm = profundidadeBaseMm - profundidadeAtualMm;

                    if (alturaObjetoMm < ALTURA_MINIMA_OBJETO_MM)
                    {
                        continue;
                    }

                    if (alturaObjetoMm > ALTURA_MAXIMA_OBJETO_MM)
                    {
                        continue;
                    }

                    double larguraPixelMm = (2 * profundidadeAtualMm * Math.Tan(fovHorizontal / 2)) / largura;
                    double alturaPixelMm = (2 * profundidadeAtualMm * Math.Tan(fovVertical / 2)) / altura;

                    volumeTotalMm3 += alturaObjetoMm * larguraPixelMm * alturaPixelMm;
                    pontosValidos++;
                }
            }

            if (pontosValidos < PONTOS_MINIMOS_VOLUME)
            {
                LoggerService.LogWarning("Leitura descartada: poucos pontos validos para volume.");
                return 0;
            }

            double volumeCm3 = volumeTotalMm3 / 1000.0;

            LoggerService.Info($"Volume calculado com filtros Kinect: {volumeCm3:F0} cm3 | Pontos validos: {pontosValidos}");

            return volumeCm3;
        }

        /// <summary>
        /// Realiza a calibração volumétrica do ambiente vazio.
        /// Cria o mapa base usado como referência para o cálculo posterior do volume.
        /// </summary>
        public async Task<CalibrationResult> CalibrateAsync(
            CancellationToken token,
            IProgress<CalibrationProgress> progress = null)
        {
            if (!IsConnected)
            {
                LoggerService.Erro("Kinect nao esta conectado para calibrar.");
                throw new InvalidOperationException("Kinect nao esta conectado para calibrar.");
            }

            _calibrado = false;

            LoggerService.Info("Iniciando calibracao volumetrica do ambiente vazio.");

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
                        Status = $"Estabilizando Kinect em {angulo} graus. Mantenha a área vazia...",
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
                    Status = "Analisando leituras do ambiente vazio...",
                    Percentage = 80
                });

                var resultadoChao = DetectarChao(leiturasPorAngulo);

                progress?.Report(new CalibrationProgress
                {
                    Status = "Mantenha a área vazia. Capturando referência do ambiente...",
                    Percentage = 88
                });

                bool mapaCapturado = CapturarMapaDepthCalibrado();

                if (!mapaCapturado)
                {
                    _calibrado = false;

                    LoggerService.Erro("Calibracao interrompida: mapa de referencia do ambiente nao capturado.");

                    return new CalibrationResult
                    {
                        MaxVolume = 0,
                        TotalPointsFound = 0,
                        CalibratedAt = DateTime.Now
                    };
                }

                double volumeMaximo = CalcularVolumeReferenciaCm3(_depthCalibrado, _larguraDepth, _alturaDepth);

                _calibrado = true;

                progress?.Report(new CalibrationProgress
                {
                    Status = "Restaurando posicao original do Kinect...",
                    Percentage = 96
                });

                await MoverMotorAsync(anguloOriginal, token, null);
                await Task.Delay(ESPERA_MOTOR_MS, token);

                progress?.Report(new CalibrationProgress
                {
                    Status = "Calibracao concluida. Ambiente vazio salvo como referencia.",
                    Percentage = 100
                });

                LoggerService.Info($"Calibracao concluida | Referencia angular: {resultadoChao.AnguloDetectado} graus | Distancia media: {resultadoChao.DistanciaChaoMm:F0} mm | Volume referencia: {volumeMaximo:F0} cm3");

                return new CalibrationResult
                {
                    MaxVolume = volumeMaximo,
                    TotalPointsFound = resultadoChao.TotalPontos,
                    CalibratedAt = DateTime.Now
                };
            }
            catch (OperationCanceledException)
            {
                _calibrado = false;

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
                _calibrado = false;

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

        /// <summary>
        /// Move o motor vertical do Kinect para o ângulo informado,
        /// respeitando os limites físicos de -27° a +27°.
        /// </summary>
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

        /// <summary>
        /// Captura múltiplos frames de profundidade e calcula a média dos pontos válidos.
        /// Usado durante a calibração para reduzir ruídos de leitura.
        /// </summary>
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

                        int margemX = frame.Width / 10;
                        int margemY = frame.Height / 10;

                        for (int y = margemY; y < frame.Height - margemY; y++)
                        {
                            for (int x = margemX; x < frame.Width - margemX; x++)
                            {
                                int i = y * frame.Width + x;
                                int depth = depthData[i] >> 3;

                                if (depth >= DEPTH_MIN_MM && depth <= DEPTH_MAX_MM)
                                {
                                    somaTotal += depth;
                                    pontosTotal++;
                                }
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

        /// <summary>
        /// Identifica a leitura angular com menor profundidade média.
        /// Essa informação é usada apenas como referência auxiliar da calibração.
        /// </summary>
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

            if (menorMedia == double.MaxValue || pontosChao < PONTOS_MINIMOS_VOLUME)
            {
                LoggerService.Erro("Nao foi possivel obter referencia angular com pontos suficientes.");
                return (0, 0, 0);
            }

            double anguloRad = Math.Abs(anguloChao) * Math.PI / 180.0;
            double distanciaChaoReal = menorMedia * Math.Cos(anguloRad);

            LoggerService.Info($"Referencia angular detectada | Angulo: {anguloChao} graus | Distancia media ajustada: {distanciaChaoReal:F1} mm");

            return (distanciaChaoReal, anguloChao, pontosChao);
        }

        /// <summary>
        /// Captura o mapa médio de profundidade do ambiente vazio.
        /// Ao recalibrar, limpa o histórico de volumes e zera a suavização anterior.
        /// </summary>
        private bool CapturarMapaDepthCalibrado()
        {
            if (!IsConnected)
            {
                LoggerService.Erro("Kinect nao conectado para capturar mapa calibrado.");
                return false;
            }

            try
            {
                const int quantidadeFrames = 10;

                int[] somaDepth = null;
                int[] contadorDepth = null;
                int largura = 0;
                int altura = 0;

                for (int f = 0; f < quantidadeFrames; f++)
                {
                    using (var frame = _sensor.DepthStream.OpenNextFrame(1000))
                    {
                        if (frame == null)
                        {
                            continue;
                        }

                        if (somaDepth == null)
                        {
                            somaDepth = new int[frame.PixelDataLength];
                            contadorDepth = new int[frame.PixelDataLength];
                            largura = frame.Width;
                            altura = frame.Height;
                        }

                        short[] depthFrame = new short[frame.PixelDataLength];
                        frame.CopyPixelDataTo(depthFrame);

                        for (int i = 0; i < depthFrame.Length; i++)
                        {
                            int depth = depthFrame[i] >> 3;

                            if (depth >= DEPTH_MIN_MM && depth <= DEPTH_MAX_MM)
                            {
                                somaDepth[i] += depth;
                                contadorDepth[i]++;
                            }
                        }
                    }
                }

                if (somaDepth == null || contadorDepth == null)
                {
                    LoggerService.Erro("Nenhum frame valido capturado para mapa calibrado.");
                    return false;
                }

                _depthCalibrado = new short[somaDepth.Length];

                _historicoVolumes.Clear();
                _ultimoVolumeSuavizado = 0;

                int pontosValidos = 0;

                for (int i = 0; i < somaDepth.Length; i++)
                {
                    if (contadorDepth[i] > 0)
                    {
                        int mediaDepth = somaDepth[i] / contadorDepth[i];
                        _depthCalibrado[i] = (short)(mediaDepth << 3);
                        pontosValidos++;
                    }
                }

                if (pontosValidos < PONTOS_MINIMOS_VOLUME)
                {
                    LoggerService.Erro("Mapa calibrado possui poucos pontos validos.");
                    return false;
                }

                _larguraDepth = largura;
                _alturaDepth = altura;

                LoggerService.Info($"Mapa de referencia do ambiente capturado. Pontos validos: {pontosValidos}");

                return true;
            }
            catch
            {
                LoggerService.Erro("Erro ao capturar mapa de profundidade calibrado.");
                return false;
            }
        }

        /// <summary>
        /// Calcula o volume máximo de referência do espaço escaneado,
        /// considerando o campo de visão horizontal e vertical do Kinect.
        /// </summary>
        private double CalcularVolumeReferenciaCm3(short[] depthCalibrado, int largura, int altura)
        {
            if (depthCalibrado == null || largura <= 0 || altura <= 0)
            {
                LoggerService.Erro("Mapa calibrado invalido para calcular volume de referencia.");
                return 0;
            }

            double fovHorizontal = FOV_HORIZONTAL_GRAUS * Math.PI / 180.0;
            double fovVertical = FOV_VERTICAL_GRAUS * Math.PI / 180.0;
            double volumeTotalMm3 = 0;
            int pontosValidos = 0;

            int margemX = largura / 10;
            int margemY = altura / 10;

            for (int y = margemY; y < altura - margemY; y++)
            {
                for (int x = margemX; x < largura - margemX; x++)
                {
                    int i = y * largura + x;
                    int profundidadeMm = depthCalibrado[i] >> 3;

                    if (profundidadeMm < DEPTH_MIN_MM || profundidadeMm > DEPTH_MAX_MM)
                    {
                        continue;
                    }

                    double larguraPixelMm = (2 * profundidadeMm * Math.Tan(fovHorizontal / 2)) / largura;
                    double alturaPixelMm = (2 * profundidadeMm * Math.Tan(fovVertical / 2)) / altura;

                    double alturaUtilMm = Math.Min(profundidadeMm, ALTURA_MAXIMA_OBJETO_MM);

                    volumeTotalMm3 += alturaUtilMm * larguraPixelMm * alturaPixelMm;
                    pontosValidos++;
                }
            }

            if (pontosValidos < PONTOS_MINIMOS_VOLUME)
            {
                LoggerService.LogWarning("Volume de referencia descartado: poucos pontos validos.");
                return 0;
            }

            double volumeCm3 = volumeTotalMm3 / 1000.0;

            LoggerService.Info($"Volume de referencia calculado: {volumeCm3:F0} cm3 | Pontos validos: {pontosValidos}");

            return Math.Round(volumeCm3, 0);
        }
    }
}