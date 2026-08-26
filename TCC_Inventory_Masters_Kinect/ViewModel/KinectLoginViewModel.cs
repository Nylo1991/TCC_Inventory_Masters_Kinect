using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TCC_Inventory_Masters_Kinect.Command;
using TCC_Inventory_Masters_Kinect.Logs;
using TCC_Inventory_Masters_Kinect.Model;
using TCC_Inventory_Masters_Kinect.Service;
using TCC_Inventory_Masters_Kinect.View;

namespace TCC_Inventory_Masters_Kinect.ViewModel
{
    /// <summary>
    /// Gerencia o estado e as ações da tela de acesso ao Kinect.
    /// </summary>
    public class KinectLoginViewModel : BaseViewModel
    {
        private readonly IAutenticacaoMvcService _autenticacaoService;
        private readonly Action<SessaoUsuario> _abrirMonitor;

        private bool _abaLoginSelecionada;
        public bool AbaLoginSelecionada
        {
            get => _abaLoginSelecionada;
            private set
            {
                if (SetProperty(ref _abaLoginSelecionada, value))
                {
                    OnPropertyChanged(nameof(AbaSolicitacaoSelecionada));
                }
            }
        }

        public bool AbaSolicitacaoSelecionada => !AbaLoginSelecionada;

        private string _titulo;
        public string Titulo
        {
            get => _titulo;
            private set => SetProperty(ref _titulo, value);
        }

        private string _subtitulo;
        public string Subtitulo
        {
            get => _subtitulo;
            private set => SetProperty(ref _subtitulo, value);
        }

        private string _tokenAcesso;
        public string TokenAcesso
        {
            get => _tokenAcesso;
            set
            {
                if (SetProperty(ref _tokenAcesso, value))
                {
                    OnPropertyChanged(nameof(TokenVazio));
                }
            }
        }

        public bool TokenVazio => string.IsNullOrEmpty(TokenAcesso);

        private string _emailCadastro;
        public string EmailCadastro
        {
            get => _emailCadastro;
            set => SetProperty(ref _emailCadastro, value);
        }

        private string _mensagem;
        public string Mensagem
        {
            get => _mensagem;
            private set => SetProperty(ref _mensagem, value);
        }

        private bool _mensagemSucesso;
        public bool MensagemSucesso
        {
            get => _mensagemSucesso;
            private set => SetProperty(ref _mensagemSucesso, value);
        }

        private bool _podeInteragir = true;
        public bool PodeInteragir
        {
            get => _podeInteragir;
            private set => SetProperty(ref _podeInteragir, value);
        }

        public ICommand MostrarLoginCommand { get; }
        public ICommand MostrarSolicitacaoTokenCommand { get; }
        public ICommand EntrarCommand { get; }
        public ICommand SolicitarTokenCommand { get; }

        public KinectLoginViewModel()
            : this(new AutenticacaoMvcService(), null)
        {
        }

        internal KinectLoginViewModel(
            IAutenticacaoMvcService autenticacaoService,
            Action<SessaoUsuario> abrirMonitor)
        {
            _autenticacaoService = autenticacaoService ??
                throw new ArgumentNullException(nameof(autenticacaoService));
            _abrirMonitor = abrirMonitor ?? AbrirMonitor;

            MostrarLoginCommand = new RelayCommand(MostrarLogin);
            MostrarSolicitacaoTokenCommand = new RelayCommand(MostrarSolicitacaoToken);
            EntrarCommand = new RelayCommand(EntrarAsync);
            SolicitarTokenCommand = new RelayCommand(SolicitarTokenAsync);

            TokenAcesso = string.Empty;
            EmailCadastro = string.Empty;
            MostrarSolicitacaoToken();
        }

        private void MostrarLogin()
        {
            AbaLoginSelecionada = true;
            Titulo = "Acesso ao Kinect";
            Subtitulo = "Informe o token enviado pelo sistema";
            LimparMensagem();
        }

        private void MostrarSolicitacaoToken()
        {
            AbaLoginSelecionada = false;
            Titulo = "Solicitar Token";
            Subtitulo = "O MVC envia o token para o e-mail cadastrado";
            LimparMensagem();
        }

        internal async Task EntrarAsync()
        {
            string token = TokenAcesso?.Trim();

            if (!Regex.IsMatch(token ?? string.Empty, @"^\d{6}$"))
            {
                ExibirErro("Informe os seis números do token de acesso.");
                return;
            }

            PodeInteragir = false;

            try
            {
                var resultado = await _autenticacaoService.ValidarTokenAsync(token);

                if (resultado == null || !resultado.TokenValido)
                {
                    ExibirErro(resultado?.Mensagem ?? "Token inválido ou expirado.");
                    LoggerService.LogWarning("Tentativa inválida de acesso ao Kinect.");
                    return;
                }

                var sessao = new SessaoUsuario
                {
                    Usuario = resultado.Usuario,
                    Empresa = resultado.Empresa,
                    Email = resultado.Email,
                    Token = token
                };

                LoggerService.Info("Acesso ao Kinect liberado com token validado pelo sistema.");
                _abrirMonitor(sessao);
            }
            catch
            {
                ExibirErro("Erro ao validar token no sistema.");
                LoggerService.Erro("Erro ao validar token pelo sistema.");
            }
            finally
            {
                PodeInteragir = true;
            }
        }

        internal async Task SolicitarTokenAsync()
        {
            string email = EmailCadastro?.Trim();

            if (string.IsNullOrWhiteSpace(email))
            {
                ExibirErro("Informe o e-mail cadastrado.");
                return;
            }

            PodeInteragir = false;

            try
            {
                var resultado = await _autenticacaoService.SolicitarTokenAsync(email);

                if (resultado == null || !resultado.Sucesso)
                {
                    ExibirErro(resultado?.Mensagem ?? "Não foi possível solicitar o token.");
                    return;
                }

                MostrarLogin();
                ExibirSucesso("Token enviado. Informe o código recebido para acessar o Kinect.");
                LoggerService.Info("Token solicitado ao MVC pelo aplicativo Kinect.");
            }
            catch
            {
                ExibirErro("Erro ao solicitar token no MVC.");
                LoggerService.Erro("Erro ao solicitar token no MVC pelo aplicativo Kinect.");
            }
            finally
            {
                PodeInteragir = true;
            }
        }

        private void AbrirMonitor(SessaoUsuario sessao)
        {
            var janelaLogin = Application.Current.Windows
                .OfType<Window>()
                .FirstOrDefault(janela => ReferenceEquals(janela.DataContext, this));

            var janelaMonitor = new KinectMonitorWindow(sessao);
            janelaMonitor.Show();
            janelaLogin?.Close();
        }

        private void LimparMensagem()
        {
            MensagemSucesso = false;
            Mensagem = string.Empty;
        }

        private void ExibirErro(string mensagem)
        {
            MensagemSucesso = false;
            Mensagem = mensagem;
        }

        private void ExibirSucesso(string mensagem)
        {
            MensagemSucesso = true;
            Mensagem = mensagem;
        }
    }
}
