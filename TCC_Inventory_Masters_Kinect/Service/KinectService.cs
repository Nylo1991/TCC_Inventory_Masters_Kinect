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
        // ESPAÇO MAPEADO
        // ==========================================

        private EspacoMapeado _espacoAtual;

        // ==========================================
        // POINT CLOUD
        // ==========================================

        private List<Point3DData> _pontos3D =
            new List<Point3DData>();

        // ==========================================
        // SNAPSHOT
        // ==========================================

        private DateTime _proximoSnapshot =
            DateTime.MinValue;

        // ==========================================
        // SUAVIZAÇÃO
        // ==========================================

        private Queue<double> _historicoVolume =
            new Queue<double>();

        private double _ultimoVolume;

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

        public event Action<List<Point3DData>>
            PointCloudAtualizada;

        public event Action<SnapshotEspacial>
            SnapshotCriado;

        // ==========================================
        // INICIALIZAR
        // ==========================================

        public bool InicializarKinect()
        {
            try
            {
                LoggerService.Info(
                    "Iniciando busca pelo Kinect.");

                StatusAtualizado?.Invoke(
                    "Procurando Kinect...");

                Sensor =
                    KinectSensor.KinectSensors
                    .FirstOrDefault(
                        s => s.Status ==
                             KinectStatus.Connected);

                if (Sensor == null)
                {
                    LoggerService.Info(
                        "Nenhum Kinect encontrado.");

                    StatusAtualizado?.Invoke(
                        "Nenhum Kinect encontrado.");

                    return false;
                }

                Sensor.ColorStream.Enable(
                    ColorImageFormat
                    .RgbResolution640x480Fps30);

                Sensor.DepthStream.Enable(
                    DepthImageFormat
                    .Resolution320x240Fps30);

                _depthPixels =
                    new DepthImagePixel[
                        Sensor.DepthStream
                            .FramePixelDataLength];

                _colorPixels =
                    new byte[
                        Sensor.ColorStream
                            .FramePixelDataLength];

                _colorBitmap =
                    new WriteableBitmap(
                        Sensor.ColorStream.FrameWidth,
                        Sensor.ColorStream.FrameHeight,
                        96,
                        96,
                        PixelFormats.Bgr32,
                        null);

                Sensor.ColorFrameReady +=
                    Sensor_ColorFrameReady;

                Sensor.DepthFrameReady +=
                    Sensor_DepthFrameReady;

                Sensor.Start();

                LoggerService.Info(
                    "Kinect iniciado com sucesso.");

                StatusAtualizado?.Invoke(
                    "Kinect iniciado.");

                return true;
            }
            catch (Exception ex)
            {
                LoggerService.Erro(
                    "Erro ao inicializar Kinect.",
                    ex);

                StatusAtualizado?.Invoke(
                    "Erro ao inicializar Kinect: " + ex.Message);

                return false;
            }
        }

        // ==========================================
        // DEFINIR ESPAÇO
        // ==========================================

        public void DefinirEspaco(
            EspacoMapeado espaco)
        {
            _espacoAtual = espaco;

            LoggerService.Info(
                $"Espaço definido: {espaco.NomeEspaco}");
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

                    if (_colorPixels == null ||
                        _colorBitmap == null)
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
                LoggerService.Erro(
                    "Erro ao processar RGB.",
                    ex);

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

                    if (Sensor == null)
                    {
                        LoggerService.Info(
                            "Frame depth ignorado: Sensor está nulo.");

                        return;
                    }

                    if (_depthPixels == null)
                    {
                        LoggerService.Info(
                            "Frame depth ignorado: array de profundidade está nulo.");

                        return;
                    }

                    frame.CopyDepthImagePixelDataTo(
                        _depthPixels);

                    if (_calibrado)
                    {
                        GerarPointCloud(
                            frame.Width,
                            frame.Height);

                        CalcularVolume(
                            frame.Width,
                            frame.Height);

                        GerarSnapshotAutomatico();
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerService.Erro(
                    "Erro ao processar depth.",
                    ex);

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
                if (_depthPixels == null)
                {
                    LoggerService.Info(
                        "Falha na calibração: dados de profundidade indisponíveis.");

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

                LoggerService.Info(
                    "Chão calibrado.");

                StatusAtualizado?.Invoke(
                    "Chão calibrado.");
            }
            catch (Exception ex)
            {
                LoggerService.Erro(
                    "Erro ao calibrar chão.",
                    ex);

                StatusAtualizado?.Invoke(
                    "Erro ao calibrar chão: " + ex.Message);
            }
        }

        // ==========================================
        // GERAR POINT CLOUD
        // ==========================================

        private void GerarPointCloud(
            int width,
            int height)
        {
            try
            {
                if (_depthPixels == null)
                {
                    LoggerService.Info(
                        "Point Cloud não gerada: dados de profundidade indisponíveis.");

                    return;
                }

                _pontos3D.Clear();

                for (int i = 0;
                     i < _depthPixels.Length;
                     i++)
                {
                    int depth =
                        _depthPixels[i].Depth;

                    if (depth <= 0)
                        continue;

                    int x =
                        i % width;

                    int y =
                        i / width;

                    Point3DData ponto =
                        new Point3DData
                        {
                            EspacoMapeadoId =
                                _espacoAtual?.Id ?? 0,

                            X =
                                x,

                            Y =
                                y,

                            Z =
                                depth,

                            Distancia =
                                depth,

                            PixelX =
                                x,

                            PixelY =
                                y,

                            TipoObjeto =
                                "Objeto",

                            DataCaptura =
                                DateTime.Now
                        };

                    _pontos3D.Add(
                        ponto);

                    if (_pontos3D.Count >=
                        KinectConfig.MaxPontos3D)
                    {
                        break;
                    }
                }

                PointCloudAtualizada?.Invoke(
                    _pontos3D);
            }
            catch (Exception ex)
            {
                LoggerService.Erro(
                    "Erro ao gerar Point Cloud.",
                    ex);
            }
        }

        // ==========================================
        // CALCULAR VOLUME
        // ==========================================

        private void CalcularVolume(
            int width,
            int height)
        {
            try
            {
                if (_depthPixels == null ||
                    _referenciaChao == null)
                {
                    LoggerService.Info(
                        "Cálculo de volume ignorado: dados insuficientes.");

                    return;
                }

                double horizontalFOV =
                    KinectConfig.HorizontalFovGraus
                    * Math.PI / 180.0;

                double verticalFOV =
                    KinectConfig.VerticalFovGraus
                    * Math.PI / 180.0;

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

                    if (delta <
                        KinectConfig
                            .LimiteMinimoAlturaMm)
                        continue;

                    if (current <
                        KinectConfig
                            .DistanciaMinimaMm)
                        continue;

                    if (current >
                        KinectConfig
                            .DistanciaMaximaMm)
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

                if (_historicoVolume.Count >
                    KinectConfig.MaxHistoricoVolume)
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

                if (_espacoAtual != null)
                {
                    _espacoAtual.VolumeAtualCm3 =
                        suavizado;

                    _espacoAtual.EspacoLivreCm3 =
                        _espacoAtual
                            .VolumeMaximoPermitidoCm3
                        - suavizado;

                    if (_espacoAtual
                            .VolumeMaximoPermitidoCm3 > 0)
                    {
                        _espacoAtual.PercentualOcupacao =
                            (suavizado /
                             _espacoAtual
                                 .VolumeMaximoPermitidoCm3)
                            * 100.0;
                    }
                    else
                    {
                        _espacoAtual.PercentualOcupacao =
                            0;
                    }

                    _espacoAtual.DataUltimaAtualizacao =
                        DateTime.Now;
                }

                if (DateTime.Now >=
                    _proximoLogVolume)
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
                LoggerService.Erro(
                    "Erro ao calcular volume.",
                    ex);

                StatusAtualizado?.Invoke(
                    "Erro ao calcular volume: " + ex.Message);
            }
        }

        // ==========================================
        // SNAPSHOT AUTOMÁTICO
        // ==========================================

        private void GerarSnapshotAutomatico()
        {
            try
            {
                if (DateTime.Now <
                    _proximoSnapshot)
                    return;

                _proximoSnapshot =
                    DateTime.Now.AddSeconds(
                        KinectConfig
                            .IntervaloSnapshotSegundos);

                SnapshotEspacial snapshot =
                    new SnapshotEspacial
                    {
                        EspacoMapeadoId =
                            _espacoAtual?.Id ?? 0,

                        NomeSnapshot =
                            "Snapshot_" +
                            DateTime.Now
                                .ToString("yyyyMMdd_HHmmss"),

                        VolumeAtualCm3 =
                            _espacoAtual
                                ?.VolumeAtualCm3 ?? 0,

                        VolumeMaximoCm3 =
                            _espacoAtual
                                ?.VolumeMaximoPermitidoCm3 ?? 0,

                        PercentualOcupacao =
                            _espacoAtual
                                ?.PercentualOcupacao ?? 0,

                        EspacoLivreCm3 =
                            _espacoAtual
                                ?.EspacoLivreCm3 ?? 0,

                        Status =
                            "Snapshot automático",

                        DataCaptura =
                            DateTime.Now
                    };

                SnapshotCriado?.Invoke(
                    snapshot);

                LoggerService.Info(
                    "Snapshot espacial criado.");
            }
            catch (Exception ex)
            {
                LoggerService.Erro(
                    "Erro ao gerar snapshot.",
                    ex);
            }
        }

        // ==========================================
        // DESLIGAR
        // ==========================================

        public void DesligarKinect()
        {
            try
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

                _calibrado = false;

                _referenciaChao = null;

                _historicoVolume.Clear();

                _ultimoVolume = 0;

                LoggerService.Info(
                    "Kinect desligado.");

                StatusAtualizado?.Invoke(
                    "Kinect desligado.");
            }
            catch (Exception ex)
            {
                LoggerService.Erro(
                    "Erro ao desligar Kinect.",
                    ex);

                StatusAtualizado?.Invoke(
                    "Erro ao desligar Kinect: " + ex.Message);
            }
        }
    }
}