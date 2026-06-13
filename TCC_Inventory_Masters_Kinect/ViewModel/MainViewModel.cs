using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using TCC_Inventory_Masters_Kinect.Command;
using TCC_Inventory_Masters_Kinect.Logs;
using TCC_Inventory_Masters_Kinect.Model;
using TCC_Inventory_Masters_Kinect.Repository;
using TCC_Inventory_Masters_Kinect.Service;

namespace TCC_Inventory_Masters_Kinect.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        private readonly KinectService _kinectService;
        private readonly SignalRService _signalRService;
        private readonly KinectRepository _repository;

        private DispatcherTimer _frameTimer;
        private DispatcherTimer _volumeTimer;
        private DispatcherTimer _envioVolumeTimer;

        private double _ultimoVolumeAtual;

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

        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        private string _statusKinect;
        public string StatusKinect
        {
            get => _statusKinect;
            set => SetProperty(ref _statusKinect, value);
        }

        private string _statusSignalR;
        public string StatusSignalR
        {
            get => _statusSignalR;
            set => SetProperty(ref _statusSignalR, value);
        }

        private string _statusSQLite;
        public string StatusSQLite
        {
            get => _statusSQLite;
            set => SetProperty(ref _statusSQLite, value);
        }

        private string _volumeTexto;
        public string VolumeTexto
        {
            get => _volumeTexto;
            set => SetProperty(ref _volumeTexto, value);
        }

        private string _volumeMaximo;
        public string VolumeMaximo
        {
            get => _volumeMaximo;
            set => SetProperty(ref _volumeMaximo, value);
        }

        private string _percentualOcupacaoTexto;
        public string PercentualOcupacaoTexto
        {
            get => _percentualOcupacaoTexto;
            set => SetProperty(ref _percentualOcupacaoTexto, value);
        }

        private string _espacoLivreTexto;
        public string EspacoLivreTexto
        {
            get => _espacoLivreTexto;
            set => SetProperty(ref _espacoLivreTexto, value);
        }

        private string _quantidadePontosDepth;
        public string QuantidadePontosDepth
        {
            get => _quantidadePontosDepth;
            set => SetProperty(ref _quantidadePontosDepth, value);
        }

        private string _mensagemEnvioAplicacao;
        public string MensagemEnvioAplicacao
        {
            get => _mensagemEnvioAplicacao;
            set => SetProperty(ref _mensagemEnvioAplicacao, value);
        }

        private bool _isCalibrating;
        public bool IsCalibrating
        {
            get => _isCalibrating;
            set => SetProperty(ref _isCalibrating, value);
        }

        public ICommand LigarKinectCommand { get; }
        public ICommand DesligarKinectCommand { get; }
        public ICommand CalibrarCommand { get; }
        public ICommand MedirCommand { get; }

        public MainViewModel()
        {
            _kinectService = new KinectService();
            _signalRService = new SignalRService();
            _repository = new KinectRepository();

            _signalRService.StatusSignalRAtualizado += status => StatusSignalR = status;

            LigarKinectCommand = new RelayCommand(LigarKinectAsync);
            DesligarKinectCommand = new RelayCommand(DesligarKinect);
            CalibrarCommand = new RelayCommand(ExecutarCalibracaoAsync);
            MedirCommand = new RelayCommand(ExecutarMedicaoAsync);

            StatusMessage = "Pronto";
            StatusKinect = "Kinect desligado";
            StatusSignalR = "SignalR: Desconectado";
            StatusSQLite = "SQLite: Aguardando";
            VolumeTexto = "0 cm3";
        }

        private async Task LigarKinectAsync()
        {
            try
            {
                _kinectService.Start();

                StatusKinect = "Kinect conectado";
                StatusMessage = "Kinect iniciado com sucesso";

                StatusSignalR = "SignalR: Conectando...";
                await _signalRService.ConectarAsync();

                StatusSignalR = _signalRService.EstaConectado
                    ? "SignalR: Conectado"
                    : "SignalR: Sem conexao";

                IniciarTimerFrames();
                IniciarTimerVolume();
                IniciarEnvioPeriodicoDeVolume();

                LoggerService.Info("Kinect iniciado pela MainViewModel.");
            }
            catch
            {
                StatusKinect = "Kinect: erro ao conectar";
                StatusMessage = "Erro ao iniciar Kinect";
                LoggerService.Erro("Erro ao iniciar Kinect pela MainViewModel.");
            }
        }

        private void IniciarTimerFrames()
        {
            _frameTimer?.Stop();

            _frameTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(33)
            };

            _frameTimer.Tick += (s, e) =>
            {
                var cameraFrame = _kinectService.CapturarFrameCamera();

                if (cameraFrame != null)
                {
                    CameraImage = cameraFrame;
                }

                var depthFrame = _kinectService.CapturarDepthColorido();

                if (depthFrame != null)
                {
                    DepthImage = depthFrame;
                }
            };

            _frameTimer.Start();
        }

        private void IniciarTimerVolume()
        {
            _volumeTimer?.Stop();

            _volumeTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(500)
            };

            _volumeTimer.Tick += (s, e) =>
            {
                double volumeAtual = _kinectService.CalcularVolumeAtualCm3();

                if (volumeAtual <= 0)
                {
                    return;
                }

                _ultimoVolumeAtual = volumeAtual;
                VolumeTexto = $"{volumeAtual:F0} cm3";
            };

            _volumeTimer.Start();
        }

        private void IniciarEnvioPeriodicoDeVolume()
        {
            _envioVolumeTimer?.Stop();

            _envioVolumeTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(15)
            };

            _envioVolumeTimer.Tick += async (s, e) =>
            {
                if (_signalRService.EstaConectado && _ultimoVolumeAtual > 0)
                {
                    await _signalRService.EnviarVolumeAsync(_ultimoVolumeAtual);
                }
            };

            _envioVolumeTimer.Start();
        }

        private void DesligarKinect()
        {
            _frameTimer?.Stop();
            _frameTimer = null;

            _volumeTimer?.Stop();
            _volumeTimer = null;

            _envioVolumeTimer?.Stop();
            _envioVolumeTimer = null;

            _kinectService.Stop();

            CameraImage = null;
            DepthImage = null;

            StatusKinect = "Kinect desligado";
            StatusMessage = "Kinect encerrado pelo usuario";

            LoggerService.Info("Kinect desligado pela MainViewModel.");
        }

        private async Task ExecutarCalibracaoAsync()
        {
            try
            {
                IsCalibrating = true;
                StatusMessage = "Calibrando...";

                var resultado = await _kinectService.CalibrateAsync(CancellationToken.None);

                VolumeMaximo = resultado.MaxVolume.ToString("F0");
                QuantidadePontosDepth = resultado.TotalPointsFound.ToString();

                StatusMessage = $"Calibracao concluida. Volume maximo: {resultado.MaxVolume:F0} cm3";

                LoggerService.Info($"Calibracao concluida. Volume maximo: {resultado.MaxVolume:F0} cm3");
            }
            catch
            {
                StatusMessage = "Erro na calibracao";
                LoggerService.Erro("Erro na calibracao pela MainViewModel.");
            }
            finally
            {
                IsCalibrating = false;
            }
        }

        private async Task ExecutarMedicaoAsync()
        {
            try
            {
                StatusMessage = "Medindo...";

                double volume = _kinectService.CalcularVolumeAtualCm3();

                await Task.CompletedTask;

                if (volume > 0)
                {
                    _ultimoVolumeAtual = volume;
                    VolumeTexto = $"{volume:F0} cm3";

                    var medicao = new MedicaoVolume
                    {
                        VolumeCm3 = volume,
                        DataHora = DateTime.Now,
                        KinectLigado = _kinectService.IsConnected,
                        Calibrado = true,
                        Status = "Medicao realizada"
                    };

                    _repository.SalvarMedicao(medicao);

                    StatusSQLite = "SQLite: Medicao salva";
                    StatusMessage = $"Medido: {volume:F0} cm3";

                    if (_signalRService.EstaConectado)
                    {
                        await _signalRService.EnviarVolumeAsync(volume);
                        MensagemEnvioAplicacao = $"Volume enviado com sucesso: {volume:F0} cm3";
                    }
                    else
                    {
                        MensagemEnvioAplicacao = "SignalR nao esta conectado.";
                    }

                    LoggerService.Info($"Medicao realizada. Volume: {volume:F0} cm3");
                }
                else
                {
                    StatusMessage = "Nenhum volume detectado";
                    LoggerService.LogWarning("Nenhum volume detectado na medicao.");
                }
            }
            catch
            {
                StatusMessage = "Erro na medicao";
                MensagemEnvioAplicacao = "Erro na medicao";
                LoggerService.Erro("Erro na medicao pela MainViewModel.");
            }
        }
    }
}