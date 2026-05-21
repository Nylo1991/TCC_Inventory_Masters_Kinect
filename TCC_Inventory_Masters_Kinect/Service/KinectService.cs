using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCC_Inventory_Masters_Kinect.Service
{
    public class KinectService
    {
        public KinectSensor Sensor { get; private set; }

        private DepthImagePixel[] _depthPixels;

        public event Action<double> MedidaAtualizada;
        public event Action<string> StatusAtualizado;

        public bool InicializarKinect()
        {
            try
            {
                Sensor = KinectSensor.KinectSensors
                    .FirstOrDefault(s => s.Status == KinectStatus.Connected);

                if (Sensor == null)
                {
                    StatusAtualizado?.Invoke("Nenhum Kinect conectado.");
                    return false;
                }

                Sensor.ColorStream.Enable(
                    ColorImageFormat.RgbResolution640x480Fps30);

                Sensor.DepthStream.Enable(
                    DepthImageFormat.Resolution320x240Fps30);

                _depthPixels = new DepthImagePixel[Sensor.DepthStream.FramePixelDataLength];

                Sensor.DepthFrameReady += Sensor_DepthFrameReady;

                Sensor.Start();

                if (Sensor.IsRunning)
                {
                    StatusAtualizado?.Invoke("Kinect ligado e capturando dados.");
                    return true;
                }

                StatusAtualizado?.Invoke("Kinect encontrado, mas não iniciou.");
                return false;
            }
            catch (Exception ex)
            {
                StatusAtualizado?.Invoke("Erro ao ligar Kinect: " + ex.Message);
                return false;
            }
        }

        private void Sensor_DepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            try
            {
                using (DepthImageFrame frame = e.OpenDepthImageFrame())
                {
                    if (frame == null)
                    {
                        return;
                    }

                    frame.CopyDepthImagePixelDataTo(_depthPixels);

                    double somaDistancias = 0;
                    int quantidadeValidos = 0;

                    foreach (var pixel in _depthPixels)
                    {
                        int profundidade = pixel.Depth;

                        if (profundidade > 0)
                        {
                            somaDistancias += profundidade;
                            quantidadeValidos++;
                        }
                    }

                    if (quantidadeValidos == 0)
                    {
                        return;
                    }

                    double mediaMm = somaDistancias / quantidadeValidos;

                    MedidaAtualizada?.Invoke(mediaMm);
                }
            }
            catch (Exception ex)
            {
                StatusAtualizado?.Invoke("Erro ao ler profundidade: " + ex.Message);
            }
        }

        public void DesligarKinect()
        {
            try
            {
                if (Sensor != null)
                {
                    Sensor.DepthFrameReady -= Sensor_DepthFrameReady;

                    if (Sensor.IsRunning)
                    {
                        Sensor.Stop();
                    }

                    Sensor = null;
                }

                StatusAtualizado?.Invoke("Kinect desligado.");
            }
            catch (Exception ex)
            {
                StatusAtualizado?.Invoke("Erro ao desligar Kinect: " + ex.Message);
            }
        }
    }
}
