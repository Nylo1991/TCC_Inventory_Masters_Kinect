using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TCC_Inventory_Masters_Kinect.Logs;
using TCC_Inventory_Masters_Kinect.Model;

namespace TCC_Inventory_Masters_Kinect.Service
{
    /// <summary>
    /// Parte do KinectService responsável pela calibração do ambiente vazio.
    /// </summary>
    public partial class KinectService
    {
        /// <summary>
        /// Realiza a calibração volumétrica do ambiente vazio.
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

            int anguloOriginal = 0;

            try
            {
                anguloOriginal = _sensor.ElevationAngle;
            }
            catch
            {
                LoggerService.LogWarning("Nao foi possivel ler o angulo original do Kinect. Usando 0 graus como referencia.");
            }

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
                        Status = $"Estabilizando Kinect em {angulo} graus. Mantenha a area vazia...",
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
                    Status = "Mantenha a area vazia. Capturando referencia do ambiente...",
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

                if (volumeMaximo <= 0)
                {
                    _calibrado = false;

                    LoggerService.Erro("Calibracao interrompida: volume de referencia invalido.");

                    return new CalibrationResult
                    {
                        MaxVolume = 0,
                        TotalPointsFound = resultadoChao.TotalPontos,
                        CalibratedAt = DateTime.Now
                    };
                }

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
        /// Move o motor vertical do Kinect para o ângulo informado.
        /// </summary>
        private async Task MoverMotorAsync(
            int angulo,
            CancellationToken token,
            IProgress<CalibrationProgress> progress)
        {
            int anguloSeguro = Math.Max(ANGULO_MIN, Math.Min(ANGULO_MAX, angulo));

            if (_sensor == null)
            {
                LoggerService.Erro("Nao foi possivel mover o motor: Kinect nao inicializado.");
                return;
            }

            try
            {
                _sensor.ElevationAngle = anguloSeguro;

                progress?.Report(new CalibrationProgress
                {
                    Status = $"Movendo motor para {anguloSeguro} graus..."
                });

                await Task.Delay(300, token);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch
            {
                LoggerService.LogWarning($"Motor ocupado ou indisponivel ao mover para {anguloSeguro} graus.");

                try
                {
                    await Task.Delay(2000, token);
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
        /// </summary>
        private async Task<(double MediaDepth, int TotalPontos)> CapturarMediaDepthAsync(
            int quantidadeFrames,
            CancellationToken token)
        {
            if (_sensor == null || _sensor.DepthStream == null)
            {
                LoggerService.Erro("Nao foi possivel capturar media depth: Kinect ou DepthStream indisponivel.");
                return (0, 0);
            }

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
                            await Task.Delay(30, token);
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
        /// </summary>
        private (double DistanciaChaoMm, int AnguloDetectado, int TotalPontos) DetectarChao(
            List<(int Angulo, double MediaDepth, int Pontos)> leituras)
        {
            try
            {
                if (leituras == null || leituras.Count == 0)
                {
                    LoggerService.Erro("Nao foi possivel detectar referencia angular: nenhuma leitura disponivel.");
                    return (0, 0, 0);
                }

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
            catch
            {
                LoggerService.Erro("Erro ao detectar referencia angular durante calibracao.");
                return (0, 0, 0);
            }
        }

        /// <summary>
        /// Captura o mapa médio de profundidade do ambiente vazio.
        /// </summary>
        private bool CapturarMapaDepthCalibrado()
        {
            if (!IsConnected || _sensor.DepthStream == null)
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
        /// Calcula o volume máximo de referência do espaço escaneado.
        /// </summary>
        private double CalcularVolumeReferenciaCm3(short[] depthCalibrado, int largura, int altura)
        {
            try
            {
                if (depthCalibrado == null || largura <= 0 || altura <= 0)
                {
                    LoggerService.Erro("Mapa calibrado invalido para calcular volume de referencia.");
                    return 0;
                }

                if (depthCalibrado.Length < largura * altura)
                {
                    LoggerService.Erro("Mapa calibrado menor que as dimensoes informadas.");
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
            catch
            {
                LoggerService.Erro("Erro ao calcular volume de referencia do ambiente.");
                return 0;
            }
        }
    }
}