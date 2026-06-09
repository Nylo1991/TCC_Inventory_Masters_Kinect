using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TCC_Inventory_Masters_Kinect.ConfigKinect;
using TCC_Inventory_Masters_Kinect.Logs;
using TCC_Inventory_Masters_Kinect.Model;

namespace TCC_Inventory_Masters_Kinect.Service
{
    public class KinectService
    {
        public KinectSensor Sensor { get; private set; }
        private bool _encerrando = false;
        private DepthImagePixel[] _depthPixels;
        private byte[] _colorPixels;
        private WriteableBitmap _colorBitmap;
        private int[] _referenciaChao;
        private bool _calibrado;
        private EspacoMapeado _espacoAtual;
        private List<Point3DData> _pontos3D = new List<Point3DData>();
        private DateTime _proximoSnapshot = DateTime.MinValue;
        private Queue<double> _historicoVolume = new Queue<double>();
        private double _ultimoVolume;
        private DateTime _proximoLogVolume = DateTime.MinValue;

        // ==================== NOVO: Calibração de Espaço ====================
        public double VolumeMaximo { get; private set; }
        public bool EstaCalibrado => _calibrado && VolumeMaximo > 0;

        public event Action<double> MedidaAtualizada;
        public event Action<string> StatusAtualizado;
        public event Action<ImageSource> CameraAtualizada;
        public event Action<List<Point3DData>> PointCloudAtualizada;
        public event Action<SnapshotEspacial> SnapshotCriado;
        public event Action<double> CalibracaoConcluida; // Novo evento

        public bool InicializarKinect()
        {
            try
            {
                _encerrando = false;

                var sensor = KinectSensor.KinectSensors.FirstOrDefault(s => s.Status == KinectStatus.Connected);
                if (sensor == null)
                {
                    StatusAtualizado?.Invoke("Nenhum Kinect encontrado.");
                    return false;
                }

                Sensor = sensor;

                Sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
                Sensor.DepthStream.Enable(DepthImageFormat.Resolution320x240Fps30);

                _depthPixels = new DepthImagePixel[Sensor.DepthStream.FramePixelDataLength];
                _colorPixels = new byte[Sensor.ColorStream.FramePixelDataLength];
                _colorBitmap = new WriteableBitmap(640, 480, 96, 96, PixelFormats.Bgr32, null);

                Sensor.ColorFrameReady += Sensor_ColorFrameReady;
                Sensor.DepthFrameReady += Sensor_DepthFrameReady;

                Sensor.Start();
                StatusAtualizado?.Invoke("Kinect iniciado.");
                return true;
            }
            catch (Exception ex)
            {
                LoggerService.Erro("Erro ao inicializar Kinect.", ex);
                return false;
            }
        }

        private void Sensor_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            if (_encerrando || Sensor == null || _colorPixels == null || _colorBitmap == null) return;

            try
            {
                using (var frame = e.OpenColorImageFrame())
                {
                    if (frame == null) return;
                    frame.CopyPixelDataTo(_colorPixels);
                    _colorBitmap.WritePixels(new Int32Rect(0, 0, frame.Width, frame.Height), _colorPixels, frame.Width * 4, 0);
                    CameraAtualizada?.Invoke(_colorBitmap);
                }
            }
            catch { }
        }

        private void Sensor_DepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            if (_encerrando || Sensor == null || _depthPixels == null) return;

            try
            {
                using (var frame = e.OpenDepthImageFrame())
                {
                    if (frame == null) return;
                    frame.CopyDepthImagePixelDataTo(_depthPixels);

                    if (_calibrado)
                    {
                        GerarPointCloud(frame.Width, frame.Height);
                        CalcularVolume(frame.Width, frame.Height);
                        GerarSnapshotAutomatico();
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerService.Erro("Erro no DepthFrameReady", ex);
            }
        }

        public void DefinirEspaco(EspacoMapeado espaco) => _espacoAtual = espaco;

        public void CalibrarChao()
        {
            if (_depthPixels == null) return;

            _referenciaChao = new int[_depthPixels.Length];
            for (int i = 0; i < _depthPixels.Length; i++)
                _referenciaChao[i] = _depthPixels[i].Depth;

            _calibrado = true;
            _historicoVolume.Clear();
            _ultimoVolume = 0;
            VolumeMaximo = 0; // Reset ao recalibrar o chão

            StatusAtualizado?.Invoke("Chão calibrado. Agora clique em 'Calibrar Espaço'.");
        }

        // ==================== NOVO: Método de Calibração do Espaço ====================
        public void CalibrarEspaco()
        {
            if (!_calibrado || _referenciaChao == null)
            {
                StatusAtualizado?.Invoke("Calibre o chão primeiro antes de calibrar o espaço.");
                return;
            }

            // Define o volume atual como o volume máximo do espaço
            VolumeMaximo = _ultimoVolume > 0 ? _ultimoVolume : 1.0;

            StatusAtualizado?.Invoke($"Espaço calibrado. Volume máximo: {VolumeMaximo:F2} m³");
            CalibracaoConcluida?.Invoke(VolumeMaximo);
        }

        private void GerarPointCloud(int width, int height)
        {
            if (_depthPixels == null) return;
            _pontos3D.Clear();

            for (int i = 0; i < _depthPixels.Length; i++)
            {
                int depth = _depthPixels[i].Depth;
                if (depth <= 0) continue;

                _pontos3D.Add(new Point3DData
                {
                    X = i % width,
                    Y = i / width,
                    Z = depth,
                    DataCaptura = DateTime.Now
                });

                if (_pontos3D.Count >= KinectConfig.MaxPontos3D) break;
            }

            PointCloudAtualizada?.Invoke(_pontos3D);
        }

        private void CalcularVolume(int width, int height)
        {
            if (_depthPixels == null || _referenciaChao == null) return;

            double volumeTotal = 0;
            for (int i = 0; i < _depthPixels.Length; i++)
            {
                int current = _depthPixels[i].Depth;
                int reference = _referenciaChao[i];
                if (current <= 0 || reference <= 0 || current >= reference) continue;
                volumeTotal += (reference - current);
            }

            double suavizado = (_ultimoVolume * 0.7) + ((volumeTotal / 1000.0) * 0.3);
            _ultimoVolume = suavizado;

            MedidaAtualizada?.Invoke(suavizado);
        }

        private void GerarSnapshotAutomatico()
        {
            if (DateTime.Now < _proximoSnapshot) return;
            _proximoSnapshot = DateTime.Now.AddSeconds(KinectConfig.IntervaloSnapshotSegundos);
            SnapshotCriado?.Invoke(new SnapshotEspacial { DataCaptura = DateTime.Now });
        }

        public void DesligarKinect()
        {
            _encerrando = true;

            if (Sensor != null)
            {
                Sensor.ColorFrameReady -= Sensor_ColorFrameReady;
                Sensor.DepthFrameReady -= Sensor_DepthFrameReady;
                if (Sensor.IsRunning) Sensor.Stop();
                Sensor = null;
            }

            _depthPixels = null;
            _colorPixels = null;
            _colorBitmap = null;
            _calibrado = false;
            StatusAtualizado?.Invoke("Kinect desligado.");
        }
    }
}
