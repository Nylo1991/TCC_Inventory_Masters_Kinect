using System;
using System.Collections.ObjectModel;
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

        private double _ultimoVolumeAtual;
        private double _volumeMaximoCm3;

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

        private string _usuarioLogado;
        public string UsuarioLogado
        {
            get => _usuarioLogado;
            set => SetProperty(ref _usuarioLogado, value);
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

        private string _nomeEspaco;
        public string NomeEspaco
        {
            get => _nomeEspaco;
            set => SetProperty(ref _nomeEspaco, value);
        }

        private string _percentualAlerta;
        public string PercentualAlerta
        {
            get => _percentualAlerta;
            set => SetProperty(ref _percentualAlerta, value);
        }

        private bool _isCalibrating;
        public bool IsCalibrating
        {
            get => _isCalibrating;
            set => SetProperty(ref _isCalibrating, value);
        }

        private bool _espacoSalvo;
        public bool EspacoSalvo
        {
            get => _espacoSalvo;
            set => SetProperty(ref _espacoSalvo, value);
        }

        private string _mensagemEspaco;
        public string MensagemEspaco
        {
            get => _mensagemEspaco;
            set => SetProperty(ref _mensagemEspaco, value);
        }

        private ObservableCollection<MedicaoVolume> _historicoMedicoes;
        public ObservableCollection<MedicaoVolume> HistoricoMedicoes
        {
            get => _historicoMedicoes;
            set => SetProperty(ref _historicoMedicoes, value);
        }

        public ICommand LigarKinectCommand { get; }
        public ICommand DesligarKinectCommand { get; }
        public ICommand CalibrarCommand { get; }
        public ICommand MedirCommand { get; }
        public ICommand SalvarEspacoCommand { get; }

        public MainViewModel()
            : this("Administrador")
        {
        }

        public MainViewModel(string usuarioLogado)
        {
            UsuarioLogado = usuarioLogado;

            _kinectService = new KinectService();
            _signalRService = new SignalRService();
            _repository = new KinectRepository();

            _signalRService.StatusSignalRAtualizado += status => StatusSignalR = status;

            LigarKinectCommand = new RelayCommand(LigarKinectAsync);
            DesligarKinectCommand = new RelayCommand(DesligarKinect);
            CalibrarCommand = new RelayCommand(ExecutarCalibracaoAsync);
            MedirCommand = new RelayCommand(ExecutarMedicaoAsync);
            SalvarEspacoCommand = new RelayCommand(SalvarEspaco);

            StatusMessage = "Pronto";
            StatusKinect = "Kinect desligado";
            StatusSignalR = "SignalR: Desconectado";
            StatusSQLite = "SQLite: Aguardando";
            VolumeTexto = "0.000 m3";
            VolumeMaximo = "0.000 m3";
            PercentualOcupacaoTexto = "Ocupação: 0%";
            EspacoLivreTexto = "0.000 m3";
            MensagemEnvioAplicacao = "Aguardando envio.";
            MensagemEspaco = "Calibre o espaço antes de salvar.";

            CarregarHistoricoMedicoes();
        }

        private void AtualizarCameraRgb(BitmapSource imagem)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                CameraImage = imagem;
            }));
        }

        private void SalvarEspaco()
        {
            if (string.IsNullOrWhiteSpace(NomeEspaco))
            {
                MensagemEspaco = "Informe o nome do espaço.";
                LoggerService.LogWarning("Tentativa de salvar espaço sem nome.");
                return;
            }

            if (string.IsNullOrWhiteSpace(PercentualAlerta))
            {
                MensagemEspaco = "Informe o limite de ocupação.";
                LoggerService.LogWarning("Tentativa de salvar espaço sem limite de ocupação.");
                return;
            }

            if (_volumeMaximoCm3 <= 0)
            {
                MensagemEspaco = "Calibre o espaço antes de salvar.";
                LoggerService.LogWarning("Tentativa de salvar espaço sem calibração.");
                return;
            }

            EspacoSalvo = true;
            MensagemEspaco = "Espaço salvo. Histórico e medição automática liberados.";
            StatusMessage = "Espaço salvo com sucesso.";

            IniciarTimerVolume();

            LoggerService.Info($"Espaço salvo: {NomeEspaco}");
        }

        private async Task LigarKinectAsync()
        {
            try
            {
                _kinectService.CameraFrameAtualizado -= AtualizarCameraRgb;
                _kinectService.CameraFrameAtualizado += AtualizarCameraRgb;

                _kinectService.Start();

                StatusKinect = "Kinect conectado";
                StatusMessage = "Kinect iniciado com sucesso";

                StatusSignalR = "SignalR: Conectando...";
                await _signalRService.ConectarAsync();

                StatusSignalR = _signalRService.EstaConectado
                    ? "SignalR: Conectado"
                    : "SignalR: Sem conexão";

                IniciarTimerFrames();

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
                Interval = TimeSpan.FromMilliseconds(100)
            };

            _frameTimer.Tick += (s, e) =>
            {
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
                Interval = TimeSpan.FromSeconds(15)
            };

            _volumeTimer.Tick += async (s, e) =>
            {
                await MedirSalvarEEnviarAsync("Medição automática");
            };

            _volumeTimer.Start();

            LoggerService.Info("Timer único de medição automática iniciado.");
        }

        private void DesligarKinect()
        {
            _frameTimer?.Stop();
            _frameTimer = null;

            _volumeTimer?.Stop();
            _volumeTimer = null;

            _kinectService.CameraFrameAtualizado -= AtualizarCameraRgb;
            _kinectService.Stop();

            CameraImage = null;
            DepthImage = null;

            StatusKinect = "Kinect desligado";
            StatusMessage = "Kinect encerrado pelo usuário";

            LoggerService.Info("Kinect desligado pela MainViewModel.");
        }

        private async Task ExecutarCalibracaoAsync()
        {
            try
            {
                IsCalibrating = true;
                EspacoSalvo = false;

                _volumeTimer?.Stop();

                StatusMessage = "Calibrando ambiente vazio...";

                var resultado = await _kinectService.CalibrateAsync(CancellationToken.None);

                _volumeMaximoCm3 = resultado.MaxVolume;

                VolumeMaximo = FormatarVolumeM3(resultado.MaxVolume);
                QuantidadePontosDepth = resultado.TotalPointsFound.ToString();

                VolumeTexto = "0.000 m3";
                PercentualOcupacaoTexto = "Ocupação: 0%";
                EspacoLivreTexto = FormatarVolumeM3(resultado.MaxVolume);

                StatusMessage = $"Calibração concluída. Volume máximo: {FormatarVolumeM3(resultado.MaxVolume)}";
                MensagemEspaco = "Calibração concluída. Salve o espaço para liberar medições.";

                LoggerService.Info($"Calibração concluída. Volume máximo: {resultado.MaxVolume:F0} cm3");
            }
            catch
            {
                StatusMessage = "Erro na calibração";
                LoggerService.Erro("Erro na calibração pela MainViewModel.");
            }
            finally
            {
                IsCalibrating = false;
            }
        }

        private async Task ExecutarMedicaoAsync()
        {
            await MedirSalvarEEnviarAsync("Medição manual");
        }

        private async Task MedirSalvarEEnviarAsync(string statusMedicao)
        {
            try
            {
                if (!_kinectService.IsConnected)
                {
                    StatusMessage = "Kinect não está conectado.";
                    LoggerService.LogWarning("Tentativa de medição com Kinect desconectado.");
                    return;
                }

                if (_volumeMaximoCm3 <= 0)
                {
                    StatusMessage = "Calibre o espaço antes de medir.";
                    LoggerService.LogWarning("Tentativa de medição sem calibração.");
                    return;
                }

                if (!EspacoSalvo)
                {
                    StatusMessage = "Salve o espaço antes de medir.";
                    LoggerService.LogWarning("Tentativa de medição antes de salvar o espaço.");
                    return;
                }

                double volumeAtualCm3 = _kinectService.CalcularVolumeAtualCm3();

                if (volumeAtualCm3 <= 0)
                {
                    StatusMessage = "Nenhum volume detectado.";
                    LoggerService.LogWarning("Nenhum volume detectado na medição.");
                    return;
                }

                _ultimoVolumeAtual = volumeAtualCm3;

                AtualizarIndicadoresVolume(volumeAtualCm3);

                var medicao = new MedicaoVolume
                {
                    VolumeCm3 = volumeAtualCm3,
                    DataHora = DateTime.Now,
                    KinectLigado = _kinectService.IsConnected,
                    Calibrado = true,
                    Status = statusMedicao
                };

                _repository.SalvarMedicao(medicao);
                CarregarHistoricoMedicoes();

                StatusSQLite = "SQLite: Medição salva";
                StatusMessage = $"Medido: {FormatarVolumeM3(volumeAtualCm3)}";

                if (_signalRService.EstaConectado)
                {
                    await _signalRService.EnviarVolumeAsync(volumeAtualCm3);
                    MensagemEnvioAplicacao = $"Volume enviado: {FormatarVolumeM3(volumeAtualCm3)}";
                }
                else
                {
                    MensagemEnvioAplicacao = "SignalR não está conectado.";
                }

                LoggerService.Info($"{statusMedicao}. Volume: {volumeAtualCm3:F0} cm3");
            }
            catch
            {
                StatusMessage = "Erro na medição";
                MensagemEnvioAplicacao = "Erro na medição";
                LoggerService.Erro("Erro na medição pela MainViewModel.");
            }
        }

        private void AtualizarIndicadoresVolume(double volumeAtualCm3)
        {
            VolumeTexto = FormatarVolumeM3(volumeAtualCm3);

            if (_volumeMaximoCm3 <= 0)
            {
                PercentualOcupacaoTexto = "Ocupação: 0%";
                EspacoLivreTexto = "0.000 m3";
                return;
            }

            double percentual = (volumeAtualCm3 / _volumeMaximoCm3) * 100.0;
            percentual = Math.Max(0, Math.Min(100, percentual));

            double espacoLivreCm3 = _volumeMaximoCm3 - volumeAtualCm3;
            espacoLivreCm3 = Math.Max(0, espacoLivreCm3);

            PercentualOcupacaoTexto = $"Ocupação: {percentual:F1}%";
            EspacoLivreTexto = FormatarVolumeM3(espacoLivreCm3);
        }

        public void CarregarHistoricoMedicoes()
        {
            var medicoes = _repository.ObterMedicoesEmOrdemCrescente(100);
            HistoricoMedicoes = new ObservableCollection<MedicaoVolume>(medicoes);
        }

        private string FormatarVolumeM3(double volumeCm3)
        {
            double volumeM3 = volumeCm3 / 1000000.0;
            return $"{volumeM3:F3} m3";
        }
    }
}