
using TCC_Inventory_Masters_Kinect.Model;
using TCC_Inventory_Masters_Kinect.Repository.Interface;
using Microsoft.Kinect;
using System;
using System.Linq;

namespace TCC_Inventory_Masters_Kinect.Service
{
    public class KinectService
    {
        // ==========================================
        // 1. SENSOR
        // ==========================================

        public KinectSensor Sensor { get; private set; }

        // ==========================================
        // 2. ARRAY DE PROFUNDIDADE
        // ==========================================

        private DepthImagePixel[] _depthPixels;

        // ==========================================
        // 3. EVENTOS
        // ==========================================

        public event Action<double> MedidaAtualizada;

        public event Action<string> StatusAtualizado;

        // ==========================================
        // 4. INICIALIZAR KINECT
        // ==========================================

        public bool InicializarKinect()
        {
            try
            {
                StatusAtualizado?.Invoke("Procurando Kinect...");

                Sensor = KinectSensor.KinectSensors
                    .FirstOrDefault(s => s.Status == KinectStatus.Connected);

                if (Sensor == null)
                {
                    StatusAtualizado?.Invoke("Nenhum Kinect conectado.");
                    return false;
                }

                // ==========================================
                // HABILITA CÂMERA RGB
                // ==========================================

                Sensor.ColorStream.Enable(
                    ColorImageFormat.RgbResolution640x480Fps30);

                // ==========================================
                // HABILITA SENSOR DE PROFUNDIDADE
                // ==========================================

                Sensor.DepthStream.Enable(
                    DepthImageFormat.Resolution320x240Fps30);

                // ==========================================
                // CRIA ARRAY DE PIXELS
                // ==========================================

                _depthPixels = new DepthImagePixel[
                    Sensor.DepthStream.FramePixelDataLength];

                // ==========================================
                // EVENTO DE LEITURA
                // ==========================================

                Sensor.DepthFrameReady += Sensor_DepthFrameReady;

                // ==========================================
                // INICIA SENSOR
                // ==========================================

                Sensor.Start();

                if (Sensor.IsRunning)
                {
                    StatusAtualizado?.Invoke("Kinect ligado com sucesso.");
                    return true;
                }

                StatusAtualizado?.Invoke("Kinect encontrado, mas não iniciou.");

                return false;
            }
            catch (Exception ex)
            {
                StatusAtualizado?.Invoke(
                    "Erro ao inicializar Kinect: " + ex.Message);

                return false;
            }
        }

        // ==========================================
        // 5. LEITURA DA PROFUNDIDADE
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
                    {
                        StatusAtualizado?.Invoke(
                            "Frame de profundidade vazio.");

                        return;
                    }

                    // ==========================================
                    // COPIA DADOS
                    // ==========================================

                    frame.CopyDepthImagePixelDataTo(_depthPixels);

                    double somaDistancias = 0;

                    int quantidadeValidos = 0;

                    // ==========================================
                    // PERCORRE TODOS PIXELS
                    // ==========================================

                    foreach (var pixel in _depthPixels)
                    {
                        int profundidade = pixel.Depth;

                        // Ignora pixels inválidos
                        if (profundidade > 0)
                        {
                            somaDistancias += profundidade;

                            quantidadeValidos++;
                        }
                    }

                    // ==========================================
                    // EVITA DIVISÃO POR ZERO
                    // ==========================================

                    if (quantidadeValidos == 0)
                    {
                        StatusAtualizado?.Invoke(
                            "Nenhum pixel válido detectado.");

                        return;
                    }

                    // ==========================================
                    // MÉDIA DA PROFUNDIDADE
                    // ==========================================

                    double mediaMm =
                        somaDistancias / quantidadeValidos;

                    // ==========================================
                    // ENVIA MEDIDA PARA VIEWMODEL
                    // ==========================================

                    MedidaAtualizada?.Invoke(mediaMm);
                }
            }
            catch (Exception ex)
            {
                StatusAtualizado?.Invoke(
                    "Erro ao ler profundidade: " + ex.Message);
            }
        }

        // ==========================================
        // 6. DESLIGAR KINECT
        // ==========================================

        public void DesligarKinect()
        {
            try
            {
                if (Sensor != null)
                {
                    // Remove evento
                    Sensor.DepthFrameReady -= Sensor_DepthFrameReady;

                    // Para Kinect
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
                StatusAtualizado?.Invoke(
                    "Erro ao desligar Kinect: " + ex.Message);
            }
        }
    }
}