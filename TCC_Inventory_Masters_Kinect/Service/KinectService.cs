using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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
                StatusAtualizado?.Invoke(
                    "Procurando Kinect...");

                Sensor =
                    KinectSensor.KinectSensors
                    .FirstOrDefault(
                        s => s.Status ==
                             KinectStatus.Connected);

                if (Sensor == null)
                {
                    StatusAtualizado?.Invoke(
                        "Nenhum Kinect encontrado.");

                    return false;
                }

                // RGB

                Sensor.ColorStream.Enable(
                    ColorImageFormat
                    .RgbResolution640x480Fps30);

                // DEPTH

                Sensor.DepthStream.Enable(
                    DepthImageFormat
                    .Resolution320x240Fps30);

                // ARRAYS

                _depthPixels =
                    new DepthImagePixel[
                        Sensor.DepthStream
                            .FramePixelDataLength];

                _colorPixels =
                    new byte[
                        Sensor.ColorStream
                            .FramePixelDataLength];

                // BITMAP

                _colorBitmap =
                    new WriteableBitmap(
                        Sensor.ColorStream.FrameWidth,
                        Sensor.ColorStream.FrameHeight,
                        96,
                        96,
                        PixelFormats.Bgr32,
                        null);

                // EVENTOS

                Sensor.ColorFrameReady +=
                    Sensor_ColorFrameReady;

                Sensor.DepthFrameReady +=
                    Sensor_DepthFrameReady;

                // START

                Sensor.Start();

                StatusAtualizado?.Invoke(
                    "Kinect iniciado.");

                return true;
            }
            catch (Exception ex)
            {
                StatusAtualizado?.Invoke(
                    ex.Message);

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

        // ==========================================
        // DEPTH
        // ==========================================

        private void Sensor_DepthFrameReady(
            object sender,
            DepthImageFrameReadyEventArgs e)
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

        // ==========================================
        // CALIBRAR CHÃO
        // ==========================================

        public void CalibrarChao()
        {
            if (_depthPixels == null)
                return;

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

            StatusAtualizado?.Invoke(
                "Chão calibrado.");
        }

        // ==========================================
        // CALCULAR VOLUME
        // ==========================================

        private void CalcularVolume()
        {
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

            MedidaAtualizada?.Invoke(
                suavizado);
        }

        // ==========================================
        // DESLIGAR
        // ==========================================

        public void DesligarKinect()
        {
            if (Sensor != null)
            {
                Sensor.ColorFrameReady -=
                    Sensor_ColorFrameReady;

                Sensor.DepthFrameReady -=
                    Sensor_DepthFrameReady;

                if (Sensor.IsRunning)
                {
                    Sensor.Stop();
                }

                Sensor = null;
            }

            StatusAtualizado?.Invoke(
                "Kinect desligado.");
        }
    }
}