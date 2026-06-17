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
    /// <summary>
    ///  ViewModel principal da aplicação, responsável por gerenciar o estado do Kinect, 
    ///  exibir as imagens, realizar calibração e medições de volume, e comunicar com o SignalR e o banco de dados SQLite.
    /// </summary>
    public class MainViewModel : BaseViewModel
    {

        /// <summary>
        /// serviço resposnaevl por gerenciar a interação de comunicação com o Kinect, 
        /// incluindo captura de frames, calibração e cálculo de volume.
        /// </summary>
        private readonly KinectService _kinectService;
        private readonly SignalRService _signalRService;
        private readonly KinectRepository _repository;
        private readonly SessaoUsuario _sessao;


        private DispatcherTimer _frameTimer;
        private DispatcherTimer _volumeTimer;

        private double _ultimoVolumeAtual;
        private double _volumeMaximoCm3;

        /// <summary>
        /// eventos respnsaveis por atualizar as mudanças como : imagens da câmera RGB e Depth, 
        /// os indicadores de volume, status de conexão e mensagens para o usuário.
        /// são responsaveis por atualizar a interface do usuário em tempo real, 
        /// garantindo que as informações exibidas estejam sempre atualizadas com o estado atual do Kinect e das medições.
        /// </summary>

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

        private string _empresaLogada;
        public string EmpresaLogada
        {
            get => _empresaLogada;
            set => SetProperty(ref _empresaLogada, value);
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

        private string _statusAlertaTexto;
        public string StatusAlertaTexto
        {
            get => _statusAlertaTexto;
            set => SetProperty(ref _statusAlertaTexto, value);
        }

        private string _statusCalibracao;

        public string StatusCalibracao
        {
            get => _statusCalibracao;
            set
            {
                _statusCalibracao = value;
                OnPropertyChanged(nameof(StatusCalibracao));
            }
        }

        /// <summary>
        /// eventos responsáveis por acionar as ações de ligar/desligar o Kinect,
        /// calibrar o ambiente, realizar medições e salvar o espaço.
        /// </summary>

        public event Action CalibracaoFinalizada;
        public ICommand LigarKinectCommand { get; }
        public ICommand DesligarKinectCommand { get; }
        public ICommand CalibrarCommand { get; }
        public ICommand MedirCommand { get; }
        public ICommand SalvarEspacoCommand { get; }

        public MainViewModel()
     : this(new SessaoUsuario
     {
         Usuario = "Administrador",
         Empresa = "Empresa Teste",
         Email = "teste@inventorymasters.com",
         Token = "DEV"
     })
        {
        }

        public MainViewModel(string usuarioLogado)
            : this(new SessaoUsuario
            {
                Usuario = usuarioLogado,
                Empresa = "Empresa Teste",
                Email = "teste@inventorymasters.com",
                Token = "DEV"
            })
        {
        }

        public MainViewModel(SessaoUsuario sessao)
        {
            _sessao = sessao;

            /// instancia dos serviços pilares do sistema - KinectService para gerenciar o Kinect,
            /// SignalRService para comunicação em tempo real,
            /// e KinectRepository para acesso ao banco de dados SQLite.
            UsuarioLogado = sessao.Usuario;
            EmpresaLogada = sessao.Empresa;

            _kinectService = new KinectService();
            _signalRService = new SignalRService();
            _repository = new KinectRepository(sessao.Empresa);

            /// conexão  direta entre os eventos do KinectService e SignalRService com as propriedades do ViewModel,
            /// para exibir mudança no status de conexão , atualizações de frames e resultados de calibração e medições.
            ///
            _signalRService.StatusSignalRAtualizado += status => StatusSignalR = status;

            LigarKinectCommand = new RelayCommand(LigarKinectAsync);
            DesligarKinectCommand = new RelayCommand(DesligarKinect);
            CalibrarCommand = new RelayCommand(ExecutarCalibracaoAsync);
            MedirCommand = new RelayCommand(ExecutarMedicaoAsync);
            SalvarEspacoCommand = new RelayCommand(SalvarEspaco);

            // inicialização dos estados iniciais das propriedades,
            // garantindo que a interface do usuário comece com informações claras e consistentes.

            StatusMessage = "Pronto";
            StatusKinect = "Kinect desligado";
            StatusSignalR = "SignalR: Desconectado";
            StatusSQLite = "SQLite: Aguardando";
            VolumeTexto = "0.000 m3";
            VolumeMaximo = "0.000 m3";
            PercentualOcupacaoTexto = "0%";
            EspacoLivreTexto = "0.000 m3";
            MensagemEnvioAplicacao = "Aguardando envio.";
            MensagemEspaco = "Calibre o espaço antes de salvar.";
            StatusAlertaTexto = "OK";

            CarregarHistoricoMedicoes();
        }

        /// <summary>
        /// método responsável por atualizar a imagem da câmera RGB exibida na interface do usuário.
        /// </summary>
        /// <param name="imagem"></param>
        private void AtualizarCameraRgb(BitmapSource imagem)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                CameraImage = imagem;
            }));
        }

        /// <summary>
        /// método responsável por validar as informações do espaço, como nome e limite de ocupação,
        /// e salvar essas informações no sistema.
        /// </summary>
        private void SalvarEspaco()
        {
            if (string.IsNullOrWhiteSpace(PercentualAlerta))
            {
                MensagemEspaco = "Informe o limite de ocupação.";
                LoggerService.LogWarning("Tentativa de salvar espaço sem limite de ocupação.");
                return;
            }

            double limiteOcupacao;

            if (!double.TryParse(PercentualAlerta, out limiteOcupacao))
            {
                MensagemEspaco = "Informe um limite de ocupação válido.";
                LoggerService.LogWarning("Tentativa de salvar espaço com limite inválido.");
                return;
            }

            if (limiteOcupacao <= 0 || limiteOcupacao > 100)
            {
                MensagemEspaco = "O limite de ocupação deve estar entre 1% e 100%.";
                LoggerService.LogWarning("Tentativa de salvar espaço com limite fora do permitido.");
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

        /// <summary>
        /// método responsável por iniciar a conexão com o Kinect, configurar os eventos de atualização de frames,
        /// e iniciar a medição automática.
        /// </summary>
        /// <returns></returns>
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
        /// <summary>
        /// método responsável por iniciar um timer que captura os frames de 
        ///profundidade do Kinect a cada 100 milissegundos, atualizando a imagem de 
        ///profundidade exibida na interface do usuário em tempo real.
        /// </summary>
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
        /// <summary>
        /// método responsável por iniciar um timer que realiza medições automáticas do volume a cada 15 segundos,
        /// e enviar essas medições para o banco de dados e para o SignalR,
        /// garantindo que as informações de ocupação estejam sempre atualizadas sem a necessidade de intervenção manual.
        /// </summary>
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

        /// <summary>
        /// método responsável por desligar o Kinect, parar os timers de captura de frames e medições automáticas,
        /// e liberar os recursos utilizados pelo Kinect.
        /// </summary>
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
        /// <summary>
        /// método responsável por realizar a calibração do ambiente utilizando o Kinect,
        /// ajustando os parâmetros necessários para medições precisas de volume.
        /// </summary>
        /// <returns></returns>
        private async Task ExecutarCalibracaoAsync()
        {
            try
            {
                IsCalibrating = true;
                EspacoSalvo = false;

                _volumeTimer?.Stop();

                StatusMessage = "Calibrando ambiente ...";

                var resultado = await _kinectService.CalibrateAsync(CancellationToken.None);

                _volumeMaximoCm3 = resultado.MaxVolume;

                VolumeMaximo = FormatarVolumeM3(resultado.MaxVolume);
                QuantidadePontosDepth = resultado.TotalPointsFound.ToString();

                VolumeTexto = "0.000 m3";
                PercentualOcupacaoTexto = "0%";
                EspacoLivreTexto = FormatarVolumeM3(resultado.MaxVolume);

                StatusMessage = $"Calibração concluída. Volume máximo: {FormatarVolumeM3(resultado.MaxVolume)}";
                MensagemEspaco = "Calibração concluída. Salve o espaço para liberar medições.";

                CalibracaoFinalizada?.Invoke();

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
        /// <summary>
        /// método responsável por realizar a medição do volume atual utilizando o Kinect,
        /// ajustando os parâmetros necessários para medições precisas de volume e apos a medição, 
        /// salva os resultados no banco de dados SQLite e envia as informações para o SignalR,
        /// </summary>
        /// <returns></returns>
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
        /// <summary>
        /// método responsável por atualizar os indicadores de volume exibidos na interface do usuário,
        /// incluindo o volume atual, percentual de ocupação, espaço livre e status de alerta.
        /// </summary>
        /// <param name="volumeAtualCm3">O volume atual em centímetros cúbicos.</param>
        private void AtualizarIndicadoresVolume(double volumeAtualCm3)
        {
            VolumeTexto = FormatarVolumeM3(volumeAtualCm3);

            if (_volumeMaximoCm3 <= 0)
            {
                PercentualOcupacaoTexto = "0%";
                EspacoLivreTexto = "0.000 m3";
                StatusAlertaTexto = "Normal";
                return;
            }

            double percentual = (volumeAtualCm3 / _volumeMaximoCm3) * 100.0;
            percentual = Math.Max(0, Math.Min(100, percentual));

            double espacoLivreCm3 = _volumeMaximoCm3 - volumeAtualCm3;
            espacoLivreCm3 = Math.Max(0, espacoLivreCm3);

            PercentualOcupacaoTexto = $"{percentual:F1}%";
            EspacoLivreTexto = FormatarVolumeM3(espacoLivreCm3);

            double limite = 0;

            if (!string.IsNullOrWhiteSpace(PercentualAlerta))
            {
                double.TryParse(PercentualAlerta, out limite);
            }

            StatusAlertaTexto = limite > 0 && percentual >= limite
                ? "Limite"
                : "Normal";
        }

        /// <summary>
        /// método responsável por carregar o histórico das últimas 100 medições de volume do banco de dados SQLite, 
        /// atualizando a coleção de medições exibida na interface do usuário.
        /// </summary>
        public void CarregarHistoricoMedicoes()
        {
            var medicoes = _repository.ObterMedicoesEmOrdemCrescente(100);
            HistoricoMedicoes = new ObservableCollection<MedicaoVolume>(medicoes);
        }
        /// <summary>
        /// método responsável por formatar o volume em centímetros cúbicos para uma string legível em metros cúbicos,
        /// </summary>
        /// <param name="volumeCm3"></param>
        /// <returns></returns>
        private string FormatarVolumeM3(double volumeCm3)
        {
            double volumeM3 = volumeCm3 / 1000000.0;
            return $"{volumeM3:F3} m3";
        }
    }
}