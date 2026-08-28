using System;
using TCC_Inventory_Masters_Kinect.Logs;

namespace TCC_Inventory_Masters_Kinect.Service
{
    /// <summary>
    /// Parte do KinectService responsável por calcular e estabilizar o volume.
    /// </summary>
    public partial class KinectService
    {
        /// <summary>
        /// Aplica média móvel e suavização ponderada ao volume atual.
        /// </summary>
        private double EstabilizarVolume(double volumeAtual)
        {
            try
            {
                if (volumeAtual <= 0)
                {
                    _historicoVolumes.Clear();
                    _ultimoVolumeSuavizado = 0;
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
            catch
            {
                LoggerService.Erro("Erro ao estabilizar volume calculado.");
                return volumeAtual;
            }
        }

        /// <summary>
        /// Calcula o volume atual detectado pelo Kinect em centímetros cúbicos.
        /// </summary>
        public double CalcularVolumeAtualCm3()
        {
            if (!IsConnected || _sensor.DepthStream == null)
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
        /// Calcula o volume real do objeto comparando o mapa calibrado com o mapa atual.
        /// </summary>
        private double CalcularVolumeRealCm3(short[] depthCalibrado, short[] depthAtual, int largura, int altura)
        {
            try
            {
                if (depthCalibrado == null || depthAtual == null)
                {
                    LoggerService.Erro("Mapa de profundidade invalido para calculo de volume.");
                    return 0;
                }

                if (largura <= 0 || altura <= 0)
                {
                    LoggerService.Erro("Dimensoes invalidas para calculo de volume.");
                    return 0;
                }

                if (depthCalibrado.Length != depthAtual.Length)
                {
                    LoggerService.Erro("Mapa calibrado e mapa atual possuem tamanhos diferentes.");
                    return 0;
                }

                if (depthAtual.Length < largura * altura)
                {
                    LoggerService.Erro("Tamanho do frame de profundidade menor que as dimensoes informadas.");
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

                        // O pixel representa uma área do espaço calibrado. Usar a
                        // profundidade atual fazia objetos próximos ocuparem uma área
                        // artificialmente menor e impedia o volume de chegar ao máximo.
                        double larguraPixelMm = (2 * profundidadeBaseMm * Math.Tan(fovHorizontal / 2)) / largura;
                        double alturaPixelMm = (2 * profundidadeBaseMm * Math.Tan(fovVertical / 2)) / altura;

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
            catch
            {
                LoggerService.Erro("Erro ao calcular volume real pelo mapa de profundidade.");
                return 0;
            }
        }
    }
}
