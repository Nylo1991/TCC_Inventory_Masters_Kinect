using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using TCC_Inventory_Masters_Kinect.Command;
using TCC_Inventory_Masters_Kinect.Logs;
using TCC_Inventory_Masters_Kinect.Model;
using TCC_Inventory_Masters_Kinect.Repository;
using TCC_Inventory_Masters_Kinect.Repository.Interface;
using TCC_Inventory_Masters_Kinect.Service;

namespace TCC_Inventory_Masters_Kinect.ViewModel
{
    /// <summary>
    /// ViewModel principal da aplicação, responsável por gerenciar o estado geral da tela,
    /// manter as propriedades exibidas na interface e inicializar os serviços principais do sistema.
    /// </summary>
    public partial class MainViewModel : BaseViewModel
    {
        /// <summary>
        /// Serviço responsável por gerenciar a comunicação com o Kinect.
        /// </summary>
        private readonly KinectService _kinectService;

        /// <summary>
        /// Serviço responsável pela comunicação em tempo real com o MVC via SignalR.
        /// </summary>
        private readonly SignalRService _signalRService;

        /// <summary>
        /// Repositório responsável pelo acesso ao banco SQLite.
        /// </summary>
        private readonly IKinectRepository _repository;

        /// <summary>
        /// Sessão atual do usuário logado.
        /// </summary>
        private readonly SessaoUsuario _sessao;
        private readonly IAutenticacaoMvcService _autenticacaoService;

        private DispatcherTimer _frameTimer;
        private DispatcherTimer _volumeTimer;

        private double _ultimoVolumeAtual;
        private double _volumeMaximoCm3;

        internal double VolumeMaximoCm3
        {
            get => _volumeMaximoCm3;
            set => _volumeMaximoCm3 = value;
        }

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
            set => SetProperty(ref _statusCalibracao, value);
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
        public ICommand SairCommand { get; }
        public ICommand AbrirHistoricoCommand { get; }
        public ICommand SolicitarNovoTokenCommand { get; }
        public ICommand DesbloquearSessaoCommand { get; }
        public ICommand RegistrarAtividadeCommand { get; }
        public ICommand FecharAvisoHistoricoCommand { get; }
        public ICommand EncerrarCommand { get; }
        public ICommand IniciarHistoricoCommand { get; }
        public ICommand PararHistoricoCommand { get; }
        public ICommand FecharHistoricoCommand { get; }

        /// <summary>
        /// Inicializa o monitor somente com uma sessao validada pelo MVC.
        /// </summary>
        public MainViewModel(SessaoUsuario sessao)
            : this(
                sessao,
                new KinectService(),
                new SignalRService(),
                new KinectRepository(sessao?.Empresa),
                new AutenticacaoMvcService(),
                true)
        {
        }

        internal MainViewModel(
            SessaoUsuario sessao,
            KinectService kinectService,
            SignalRService signalRService,
            IKinectRepository repository,
            IAutenticacaoMvcService autenticacaoService,
            bool iniciarTimerInatividade)
        {
            if (sessao == null)
            {
                throw new ArgumentNullException(nameof(sessao));
            }

            if (string.IsNullOrWhiteSpace(sessao.Usuario) ||
                string.IsNullOrWhiteSpace(sessao.Empresa) ||
                string.IsNullOrWhiteSpace(sessao.Email) ||
                string.IsNullOrWhiteSpace(sessao.Token) ||
                string.Equals(sessao.Token, "DEV", StringComparison.OrdinalIgnoreCase))
            {
                LoggerService.LogWarning("Tentativa de iniciar o monitor sem sessao valida do MVC.");
                throw new InvalidOperationException("O acesso ao monitor exige uma sessao validada pelo MVC.");
            }

            _sessao = sessao;
            _kinectService = kinectService ?? throw new ArgumentNullException(nameof(kinectService));
            _signalRService = signalRService ?? throw new ArgumentNullException(nameof(signalRService));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _autenticacaoService = autenticacaoService ?? throw new ArgumentNullException(nameof(autenticacaoService));

            UsuarioLogado = sessao.Usuario;
            EmpresaLogada = sessao.Empresa;
            LoggerService.DefinirEmpresa(sessao.Empresa);

            _signalRService.StatusSignalRAtualizado += status => StatusSignalR = status;

            LigarKinectCommand = new RelayCommand(LigarKinectAsync);
            DesligarKinectCommand = new RelayCommand(DesligarKinect);
            CalibrarCommand = new RelayCommand(ExecutarCalibracaoAsync);
            MedirCommand = new RelayCommand(ExecutarMedicaoAsync);
            SalvarEspacoCommand = new RelayCommand(SalvarEspaco);
            SairCommand = new RelayCommand(Sair);
            AbrirHistoricoCommand = new RelayCommand(AbrirHistorico);
            SolicitarNovoTokenCommand = new RelayCommand(SolicitarNovoTokenAsync);
            DesbloquearSessaoCommand = new RelayCommand(DesbloquearSessaoAsync);
            RegistrarAtividadeCommand = new RelayCommand(RegistrarAtividadeUsuario);
            FecharAvisoHistoricoCommand = new RelayCommand(() => AvisoHistoricoVisivel = false);
            EncerrarCommand = new RelayCommand(Encerrar);
            IniciarHistoricoCommand = new RelayCommand(IniciarAtualizacaoHistorico);
            PararHistoricoCommand = new RelayCommand(PararAtualizacaoHistorico);
            FecharHistoricoCommand = new RelayCommand(FecharHistorico);

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
            StatusCalibracao = string.Empty;

            InicializarInterfaceMonitor(iniciarTimerInatividade);

            CarregarHistoricoMedicoes();
        }

    }
}
