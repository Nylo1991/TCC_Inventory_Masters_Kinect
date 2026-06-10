using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Kinect;
using TCC_Inventory_Masters_Kinect.Logs; // Mantenha se usar LoggerService

namespace TCC_Inventory_Masters_Kinect.Service
{
    public class KinectService
    {
        private KinectSensor _sensor;

        public bool IsConnected => _sensor != null && _sensor.Status == KinectStatus.Connected;

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
                throw new Exception("Nenhum Kinect foi encontrado no computador.");

            if (_sensor.Status != KinectStatus.Connected)
                throw new Exception("Kinect v1 não está conectado.");

            // Habilita os streams
            _sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
            _sensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
            _sensor.SkeletonStream.Enable();

            // Inicia o sensor (ESSENCIAL)
            _sensor.Start();
        }

        public void Stop()
        {
            _sensor?.Stop();
        }

        // ==================== CÂMERA RGB ====================
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
                        frame.Width,
                        frame.Height,
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
                Console.WriteLine($"Erro ao capturar câmera: {ex.Message}");
                return null;
            }
        }

        // ==================== DEPTH COLORIDO ====================
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
                        byte intensity = (depth == 0) ? (byte)0 : (byte)(255 - Math.Min(255, depth * 255 / 4000));

                        pixels[index++] = intensity;               // B
                        pixels[index++] = (byte)(intensity * 0.7); // G
                        pixels[index++] = (byte)(intensity * 0.4); // R
                        pixels[index++] = 255;                     // A
                    }

                    return BitmapSource.Create(
                        frame.Width,
                        frame.Height,
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
                Console.WriteLine($"Erro ao capturar depth: {ex.Message}");
                return null;
            }
        }

        // ==================== MEDIÇÃO DE VOLUME ====================
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
                        if (depth > 0 && depth < 4000)
                        {
                            soma += depth;
                            validos++;
                        }
                    }

                    if (validos == 0) return 0;

                    double media = soma / validos;
                    return media * 100; // aproximação em cm³
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao medir volume: {ex.Message}");
                return 0;
            }
        }

        // ==================== CALIBRAÇÃO ====================
        public async Task<CalibrationResult> CalibrateAsync(CancellationToken token, IProgress<CalibrationProgress> progress = null)
        {
            progress?.Report(new CalibrationProgress { Status = "Calibrando Kinect..." });

            await Task.Delay(1500, token); // Simulação (substitua pela lógica real depois)

            return new CalibrationResult
            {
                MaxVolume = 500000,
                TotalPointsFound = 25000
            };
        }
    }

    // ==================== CLASSES AUXILIARES ====================
    public class CalibrationResult
    {
        public double MaxVolume { get; set; }
        public int TotalPointsFound { get; set; }
    }

    public class CalibrationProgress
    {
        public string Status { get; set; }
    }
}
