using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TCC_Inventory_Masters_Kinect.Logs;

namespace TCC_Inventory_Masters_Kinect.Service
{
    public class KinectService
    {
        // ==========================================
        // SENSOR
        // ==========================================

        public KinectSensor Sensor
        {
            get;
            private set;
        }

        // ==========================================
        // DEPTH
        // ==========================================

        private DepthImagePixel[] _depthPixels;

        // ==========================================
        // RGB
        // ==========================================

        private byte[] _colorPixels;

        private WriteableBitmap _colorBitmap;

        // ==========================================
        // CALIBRAÇÃO
        // ==========================================

        private int[] _referenciaChao;

        private bool _calibrado;

        // ==========================================
        // SUAVIZAÇÃO
        // ==========================================

        private Queue<double> _historicoVolume =
            new Queue<double>();

        private double _ultimoVolume;

        // Controla o intervalo de registro do volume no arquivo de log.
        // Evita gravar uma linha de log a cada frame do Kinect.
        private DateTime _proximoLogVolume =
            DateTime.MinValue;

        // ==========================================
        // EVENTOS
        // ==========================================

        public event Action<double>
            MedidaAtualizada;

        public event Action<string>
            StatusAtualizado;

        public event Action<ImageSource>
            CameraAtualizada;

        // ==========================================
        // INICIALIZAR
        // ==========================================

        public bool InicializarKinect()
        {
            try
            {
                LoggerService.Info("Iniciando busca pelo Kinect.");

                StatusAtualizado?.Invoke(
                    "Procurando Kinect...");

                Sensor =
                    KinectSensor.KinectSensors
                    .FirstOrDefault(
                        s => s.Status ==
                             KinectStatus.Connected);

                if (Sensor == null)
                {
                    LoggerService.Info("Nenhum Kinect encontrado.");

                    StatusAtualizado?.Invoke(
                        "Nenhum Kinect encontrado.");

                    return false;
                }

                LoggerService.Info("Kinect encontrado. Habilitando streams RGB e Depth.");

                // RGB

                Sensor.ColorStream.Enable(
                    ColorImageFormat
                    .RgbResolution640x480Fps30);

                LoggerService.Info("Stream RGB habilitado.");

                // DEPTH

                Sensor.DepthStream.Enable(
                    DepthImageFormat
                    .Resolution320x240Fps30);

                LoggerService.Info("Stream de profundidade habilitado.");

                // ARRAYS

                _depthPixels =
                    new DepthImagePixel[
                        Sensor.DepthStream
                            .FramePixelDataLength];

                _colorPixels =
                    new byte[
                        Sensor.ColorStream
                            .FramePixelDataLength];

                LoggerService.Info("Arrays de captura inicializados.");

                // BITMAP

                _colorBitmap =
                    new WriteableBitmap(
                        Sensor.ColorStream.FrameWidth,
                        Sensor.ColorStream.FrameHeight,
                        96,
                        96,
                        PixelFormats.Bgr32,
                        null);

                LoggerService.Info("Bitmap da câmera inicializado.");

                // EVENTOS

                Sensor.ColorFrameReady +=
                    Sensor_ColorFrameReady;

                Sensor.DepthFrameReady +=
                    Sensor_DepthFrameReady;

                LoggerService.Info("Eventos RGB e Depth registrados.");

                // START

                Sensor.Start();

                LoggerService.Info("Kinect iniciado com sucesso.");

                StatusAtualizado?.Invoke(
                    "Kinect iniciado.");

                return true;
            }
            catch (Exception ex)
            {
                LoggerService.Erro("Erro ao inicializar Kinect.", ex);

                StatusAtualizado?.Invoke(
                    "Erro ao inicializar Kinect: " + ex.Message);

                return false;
            }
        }

        // ==========================================
        // RGB
        // ==========================================

        private void Sensor_ColorFrameReady(
            object sender,
            ColorImageFrameReadyEventArgs e)
        {
            try
            {
                using (var frame =
                    e.OpenColorImageFrame())
                {
                    if (frame == null)
                        return;

                    frame.CopyPixelDataTo(
                        _colorPixels);

                    _colorBitmap.WritePixels(
                        new Int32Rect(
                            0,
                            0,
                            frame.Width,
                            frame.Height),

                        _colorPixels,

                        frame.Width * 4,

                        0);

                    CameraAtualizada?.Invoke(
                        _colorBitmap);
                }
            }
            catch (Exception ex)
            {
                LoggerService.Erro("Erro ao processar frame RGB.", ex);

                StatusAtualizado?.Invoke(
                    "Erro ao processar imagem RGB.");
            }
        }

        // ==========================================
        // DEPTH
        // ==========================================

        private void Sensor_DepthFrameReady(
            object sender,
            DepthImageFrameReadyEventArgs e)
        {
            try
            {
                using (DepthImageFrame frame =
                    e.OpenDepthImageFrame())
                {
                    if (frame == null)
                        return;

                    frame.CopyDepthImagePixelDataTo(
                        _depthPixels);

                    if (_calibrado)
                    {
                        CalcularVolume();
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerService.Erro("Erro ao processar frame de profundidade.", ex);

                StatusAtualizado?.Invoke(
                    "Erro ao processar profundidade.");
            }
        }

        // ==========================================
        // CALIBRAR CHÃO
        // ==========================================

        public void CalibrarChao()
        {
            try
            {
                LoggerService.Info("Tentativa de calibração do chão iniciada.");

                if (_depthPixels == null)
                {
                    LoggerService.Info("Falha na calibração: dados de profundidade indisponíveis.");

                    StatusAtualizado?.Invoke(
                        "Não há dados de profundidade para calibrar.");

                    return;
                }

                _referenciaChao =
                    new int[_depthPixels.Length];

                for (int i = 0;
                     i < _depthPixels.Length;
                     i++)
                {
                    _referenciaChao[i] =
                        _depthPixels[i].Depth;
                }

                _calibrado = true;

                _historicoVolume.Clear();

                _ultimoVolume = 0;

                LoggerService.Info("Chão calibrado com sucesso.");

                StatusAtualizado?.Invoke(
                    "Chão calibrado.");
            }
            catch (Exception ex)
            {
                LoggerService.Erro("Erro ao calibrar chão.", ex);

                StatusAtualizado?.Invoke(
                    "Erro ao calibrar chão: " + ex.Message);
            }
        }

        // ==========================================
        // CALCULAR VOLUME
        // ==========================================

        private void CalcularVolume()
        {
            try
            {
                if (Sensor == null ||
                    _depthPixels == null ||
                    _referenciaChao == null)
                {
                    LoggerService.Info("Cálculo de volume ignorado: dados insuficientes.");

                    return;
                }

                double horizontalFOV =
                    57 * Math.PI / 180.0;

                double verticalFOV =
                    43 * Math.PI / 180.0;

                int width =
                    Sensor.DepthStream.FrameWidth;

                int height =
                    Sensor.DepthStream.FrameHeight;

                double volumeTotal = 0;

                for (int i = 0;
                     i < _depthPixels.Length;
                     i++)
                {
                    int current =
                        _depthPixels[i].Depth;

                    int reference =
                        _referenciaChao[i];

                    if (current <= 0 ||
                        reference <= 0 ||
                        current >= reference)
                        continue;

                    int delta =
                        reference - current;

                    if (delta < 30)
                        continue;

                    double altura =
                        delta / 1000.0;

                    double distancia =
                        current / 1000.0;

                    double pixelWidth =
                        2 *
                        distancia *
                        Math.Tan(horizontalFOV / 2)
                        / width;

                    double pixelHeight =
                        2 *
                        distancia *
                        Math.Tan(verticalFOV / 2)
                        / height;

                    double pixelArea =
                        pixelWidth *
                        pixelHeight;

                    volumeTotal +=
                        altura *
                        pixelArea;
                }

                double volumeCm3 =
                    volumeTotal * 1000000;

                _historicoVolume.Enqueue(
                    volumeCm3);

                if (_historicoVolume.Count > 30)
                {
                    _historicoVolume.Dequeue();
                }

                double media =
                    _historicoVolume.Average();

                double suavizado =
                    (_ultimoVolume * 0.7)
                    + (media * 0.3);

                _ultimoVolume =
                    suavizado;

                // Registra o volume no log apenas a cada 5 segundos.
                // Isso evita criar um arquivo de log muito grande.
                if (DateTime.Now >= _proximoLogVolume)
                {
                    _proximoLogVolume =
                        DateTime.Now.AddSeconds(5);

                    LoggerService.Info(
                        $"Volume calculado: {suavizado:F2} cm³.");
                }

                MedidaAtualizada?.Invoke(
                    suavizado);
            }
            catch (Exception ex)
            {
                LoggerService.Erro("Erro ao calcular volume.", ex);

                StatusAtualizado?.Invoke(
                    "Erro ao calcular volume: " + ex.Message);
            }
        }

        // ==========================================
        // DESLIGAR
        // ==========================================

        public void DesligarKinect()
        {
            try
            {
                LoggerService.Info("Desligamento do Kinect solicitado.");

                if (Sensor != null)
                {
                    Sensor.ColorFrameReady -=
                        Sensor_ColorFrameReady;

                    Sensor.DepthFrameReady -=
                        Sensor_DepthFrameReady;

                    if (Sensor.IsRunning)
                    {
                        Sensor.Stop();

                        LoggerService.Info("Sensor Kinect parado.");
                    }

                    Sensor = null;
                }

                LoggerService.Info("Kinect desligado com sucesso.");

                StatusAtualizado?.Invoke(
                    "Kinect desligado.");
            }
            catch (Exception ex)
            {
                LoggerService.Erro("Erro ao desligar Kinect.", ex);

                StatusAtualizado?.Invoke(
                    "Erro ao desligar Kinect: " + ex.Message);
            }
        }
    }
}