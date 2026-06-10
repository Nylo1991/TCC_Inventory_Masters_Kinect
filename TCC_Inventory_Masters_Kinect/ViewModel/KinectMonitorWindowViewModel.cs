using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using TCC_Inventory_Masters_Kinect.Command;
using TCC_Inventory_Masters_Kinect.Service;

namespace TCC_Inventory_Masters_Kinect.ViewModel
{
    public class KinectMonitorWindowViewModel : BaseViewModel
    {
        private readonly KinectService _kinectService;
        private DispatcherTimer _frameTimer;

        // ─── PROPRIEDADES DE IMAGEM ─────────────────────────────────────
        private BitmapSource _cameraImage;
        public BitmapSource CameraImage
        {
            get => _cameraImage;
            set => SetProperty(ref _cameraImage, value);
        }

        private BitmapSource _depthImage;
        public BitmapSource DepthImage
        {
            get => _depthImage;
            set => SetProperty(ref _depthImage, value);
        }

        // ─── STATUS ─────────────────────────────────────────────────────
        private string _statusKinect = "Kinect: Desconectado";
        public string StatusKinect
        {
            get => _statusKinect;
            set => SetProperty(ref _statusKinect, value);
        }

        private string _status = "Pronto.";
        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        // ─── OUTRAS PROPRIEDADES (mantidas) ─────────────────────────────
        public string StatusSQLite { get; set; } = "SQLite: Aguardando...";
        public string StatusSignalR { get; set; } = "SignalR: Desconectado";
        public string StatusMvcFirebase { get; set; } = "MVC/Firebase: Aguardando...";

        public string VolumeTexto { get; set; } = "Volume: --";
        public string PercentualOcupacaoTexto { get; set; } = "Ocupacao: --%";
        public string EspacoLivreTexto { get; set; } = "Espaco livre: --";
        public string QuantidadePontos3D { get; set; } = "Pontos 3D: 0";
        public string UltimoSnapshot { get; set; } = "Ultimo snapshot: --";

        public string NomeEspaco { get; set; } = "Estoque Principal";
        public string PercentualAlerta { get; set; } = "80";
        public string VolumeMaximo { get; set; } = "0";
        public string MensagemEnvioAplicacao { get; set; } = "Nenhum envio realizado.";

        // ─── COMANDOS ───────────────────────────────────────────────────
        public ICommand LigarKinectCommand { get; }
        public ICommand DesligarKinectCommand { get; }
        public ICommand CalibrarCommand { get; }
        public ICommand CalibrarEspacoCommand { get; }

        public KinectMonitorWindowViewModel()
        {
            _kinectService = new KinectService();

            LigarKinectCommand = new RelayCommand(LigarKinect);
            DesligarKinectCommand = new RelayCommand(DesligarKinect);
            CalibrarCommand = new RelayCommand(CalibrarChao);
            CalibrarEspacoCommand = new RelayCommand(CalibrarEspaco);
        }

        // ─── LIGAR KINECT COM TIMER ─────────────────────────────────────
        private void LigarKinect()
        {
            try
            {
                _kinectService.Start();
                StatusKinect = "Kinect: Conectado";
                Status = "Kinect ligado com sucesso.";

                // Inicia o timer para capturar frames
                IniciarTimerFrames();
            }
            catch (Exception ex)
            {
                StatusKinect = "Kinect: Erro ao conectar";
                Status = $"Erro: {ex.Message}";
            }
        }

        private void IniciarTimerFrames()
        {
            _frameTimer?.Stop();

            _frameTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(33) // ~30 FPS
            };

            _frameTimer.Tick += (s, e) =>
            {
                // Atualiza Camera RGB
                var cameraFrame = _kinectService.CapturarFrameCamera();
                if (cameraFrame != null)
                    CameraImage = cameraFrame;

                // Atualiza Depth (nuvem de pontos)
                var depthFrame = _kinectService.CapturarDepthColorido();
                if (depthFrame != null)
                    DepthImage = depthFrame;
            };

            _frameTimer.Start();
        }

        private void DesligarKinect()
        {
            _frameTimer?.Stop();
            _frameTimer = null;

            _kinectService.Stop();
            StatusKinect = "Kinect: Desconectado";
            CameraImage = null;
            DepthImage = null;
            Status = "Kinect desligado.";
        }

        private async void CalibrarChao()
        {
            try
            {
                Status = "Calibrando chao... aguarde.";
                var resultado = await _kinectService.CalibrateAsync(CancellationToken.None);
                Status = $"Chao calibrado! Pontos: {resultado.TotalPointsFound}";
                MessageBox.Show($"Calibracao concluida!\nPontos: {resultado.TotalPointsFound}", "Calibracao OK");
            }
            catch (Exception ex)
            {
                Status = $"Erro na calibracao: {ex.Message}";
                MessageBox.Show(ex.Message, "Erro");
            }
        }

        private async void CalibrarEspaco()
        {
            try
            {
                Status = "Calibrando espaco... aguarde.";
                var resultado = await _kinectService.CalibrateAsync(CancellationToken.None);
                VolumeMaximo = resultado.MaxVolume.ToString("F0");
                Status = $"Espaco calibrado! Volume maximo: {resultado.MaxVolume:F0} cm³";
                MessageBox.Show($"Volume maximo definido: {resultado.MaxVolume:F0} cm³", "Espaco OK");
            }
            catch (Exception ex)
            {
                Status = $"Erro: {ex.Message}";
                MessageBox.Show(ex.Message, "Erro");
            }
        }

        public void Dispose()
        {
            _frameTimer?.Stop();
            _kinectService?.Stop();
        }
    }
}
