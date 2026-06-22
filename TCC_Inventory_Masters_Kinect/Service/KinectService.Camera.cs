using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Kinect;
using TCC_Inventory_Masters_Kinect.Logs;

namespace TCC_Inventory_Masters_Kinect.Service
{
    /// <summary>
    /// Parte do KinectService responsável pela captura da câmera RGB e da imagem de profundidade colorida.
    /// </summary>
    public partial class KinectService
    {
        /// <summary>
        /// Evento interno responsável por capturar o frame RGB em tempo real.
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
        /// Captura o mapa de profundidade do Kinect e gera uma imagem colorida.
        /// </summary>
        public BitmapSource CapturarDepthColorido()
        {
            if (!IsConnected || _sensor.DepthStream == null)
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

                        if (depth <= DEPTH_MIN_MM || depth >= DEPTH_MAX_MM)
                        {
                            intensity = 0;
                        }
                        else
                        {
                            double normalizado = (depth - DEPTH_MIN_MM) / (double)(DEPTH_MAX_MM - DEPTH_MIN_MM);
                            intensity = (byte)(255 - System.Math.Min(255, System.Math.Max(0, normalizado * 255)));
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
    }
}