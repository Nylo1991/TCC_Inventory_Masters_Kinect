using Microsoft.Kinect;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TCC_Inventory_Masters_Kinect.Logs;

namespace TCC_Inventory_Masters_Kinect.Service
{
    public class KinectService
    {
        private KinectSensor _sensor;
        private DepthImageFrame _lastDepthFrame;
        private Skeleton[] _skeletons = new Skeleton[6];

        public bool IsConnected => _sensor != null && _sensor.Status == KinectStatus.Connected;

        public KinectService()
        {
            _sensor = KinectSensor.KinectSensors[0];
        }

        public void Start()
        {
            if (_sensor == null || _sensor.Status != KinectStatus.Connected)
                throw new Exception("Kinect v1 não encontrado ou não conectado.");

            _sensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
            _sensor.SkeletonStream.Enable();

            _sensor.Start();
        }

        public void Stop()
        {
            _sensor?.Stop();
        }

        // Captura simples de volume (versão básica para v1)
        public async Task<double> MeasureCurrentVolumeAsync(CancellationToken token)
        {
            if (!IsConnected)
                return 0;

            try
            {
                using (var frame = _sensor.DepthStream.OpenNextFrame(1000))
                {
                    if (frame == null) return 0;

                    short[] depthData = new short[frame.PixelDataLength];
                    frame.CopyPixelDataTo(depthData);

                    // Cálculo simples de volume baseado na profundidade
                    double volume = 0;
                    int validPoints = 0;

                    for (int i = 0; i < depthData.Length; i++)
                    {
                        int depth = depthData[i] >> 3; // Remove player index
                        if (depth > 0 && depth < 4000)
                        {
                            volume += depth;
                            validPoints++;
                        }
                    }

                    if (validPoints == 0) return 0;

                    double averageDepth = volume / validPoints;
                    return averageDepth * 100; // Convertendo para cm³ aproximado
                }
            }
            catch (Exception ex)
            {
                LoggerService.Erro("Erro ao medir volume no Kinect v1.", ex);
                return 0;
            }
        }

        // Calibração básica do chão (versão simplificada)
        public async Task<CalibrationResult> CalibrateAsync(IProgress<CalibrationProgress> progress, CancellationToken token)
        {
            progress?.Report(new CalibrationProgress { Status = "Calibrando Kinect v1..." });

            // Simulação de calibração (pode ser melhorada depois)
            await Task.Delay(1500, token);

            return new CalibrationResult
            {
                MaxVolume = 500000,
                TotalPointsFound = 25000
            };
        }

        internal async Task CalibrateAsync(Progress<Model.CalibrationProgress> progress, CancellationToken none)
        {
            throw new NotImplementedException();
        }
    }

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
