using System.ComponentModel;
using System;
using System.Threading.Tasks;
using TCC_Inventory_Masters_Kinect.Model;
using TCC_Inventory_Masters_Kinect.ViewModel;
using Xunit;

namespace TCC_Inventory_Masters_Kinect.Diulie.Tests.ViewModel
{
    [Xunit.Trait("Integrante", "Diulie")]
    public class KinectLoginViewModelTests
    {
        #region Teste estado inicial do ViewModel

        [Fact]
        public void Construtor_AoInicializar_DeveExibirAbaSolicitacaoToken()
        {
            // Arrange + Act
            var viewModel = new KinectLoginViewModel();

            // Assert
            Assert.False(viewModel.AbaLoginSelecionada);
            Assert.True(viewModel.AbaSolicitacaoSelecionada);

            Assert.Equal("Solicitar Token", viewModel.Titulo);

            Assert.Equal(
                "O MVC envia o token para o e-mail cadastrado",
                viewModel.Subtitulo);
        }

        #endregion


        #region Teste valores iniciais

        [Fact]
        public void Construtor_AoInicializar_DeveInicializarCamposVazios()
        {
            // Arrange + Act
            var viewModel = new KinectLoginViewModel();

            // Assert
            Assert.Equal(string.Empty, viewModel.TokenAcesso);
            Assert.Equal(string.Empty, viewModel.EmailCadastro);
            Assert.Equal(string.Empty, viewModel.Mensagem);

            Assert.True(viewModel.TokenVazio);
            Assert.False(viewModel.MensagemSucesso);
            Assert.True(viewModel.PodeInteragir);
        }

        #endregion


        #region Teste comandos inicializados

        [Fact]
        public void Construtor_AoInicializar_DeveCriarTodosOsComandos()
        {
            // Arrange + Act
            var viewModel = new KinectLoginViewModel();

            // Assert
            Assert.NotNull(viewModel.MostrarLoginCommand);
            Assert.NotNull(viewModel.MostrarSolicitacaoTokenCommand);
            Assert.NotNull(viewModel.EntrarCommand);
            Assert.NotNull(viewModel.SolicitarTokenCommand);
        }

        #endregion


        #region Teste mostrar tela de login

        [Fact]
        public void MostrarLoginCommand_AoExecutar_DeveSelecionarAbaLogin()
        {
            // Arrange
            var viewModel = new KinectLoginViewModel();

            // Act
            viewModel.MostrarLoginCommand.Execute(null);

            // Assert
            Assert.True(viewModel.AbaLoginSelecionada);
            Assert.False(viewModel.AbaSolicitacaoSelecionada);

            Assert.Equal(
                "Acesso ao Kinect",
                viewModel.Titulo);

            Assert.Equal(
                "Informe o token enviado pelo sistema",
                viewModel.Subtitulo);
        }

        #endregion


        #region Teste mostrar tela de solicitação

        [Fact]
        public void MostrarSolicitacaoTokenCommand_AoExecutar_DeveSelecionarAbaSolicitacao()
        {
            // Arrange
            var viewModel = new KinectLoginViewModel();

            viewModel.MostrarLoginCommand.Execute(null);

            // Act
            viewModel.MostrarSolicitacaoTokenCommand.Execute(null);

            // Assert
            Assert.False(viewModel.AbaLoginSelecionada);
            Assert.True(viewModel.AbaSolicitacaoSelecionada);

            Assert.Equal(
                "Solicitar Token",
                viewModel.Titulo);

            Assert.Equal(
                "O MVC envia o token para o e-mail cadastrado",
                viewModel.Subtitulo);
        }

        #endregion


        #region Teste TokenVazio com token vazio

        [Fact]
        public void TokenAcesso_QuandoVazio_TokenVazioDeveSerTrue()
        {
            // Arrange
            var viewModel = new KinectLoginViewModel();

            // Act
            viewModel.TokenAcesso = string.Empty;

            // Assert
            Assert.True(viewModel.TokenVazio);
        }

        #endregion


        #region Teste TokenVazio com token preenchido

        [Fact]
        public void TokenAcesso_QuandoPreenchido_TokenVazioDeveSerFalse()
        {
            // Arrange
            var viewModel = new KinectLoginViewModel();

            // Act
            viewModel.TokenAcesso = "123456";

            // Assert
            Assert.False(viewModel.TokenVazio);
        }

        #endregion


        #region Teste PropertyChanged do TokenAcesso

        [Fact]
        public void TokenAcesso_QuandoAlterado_DeveNotificarTokenVazio()
        {
            // Arrange
            var viewModel = new KinectLoginViewModel();

            var tokenVazioNotificado = false;

            viewModel.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(viewModel.TokenVazio))
                {
                    tokenVazioNotificado = true;
                }
            };

            // Act
            viewModel.TokenAcesso = "123456";

            // Assert
            Assert.True(tokenVazioNotificado);
        }

        #endregion


        #region Teste token com menos de seis números

        [Fact]
        public void EntrarCommand_TokenComMenosDeSeisNumeros_DeveExibirErro()
        {
            // Arrange
            var viewModel = new KinectLoginViewModel();

            viewModel.TokenAcesso = "12345";

            // Act
            viewModel.EntrarCommand.Execute(null);

            // Assert
            Assert.False(viewModel.MensagemSucesso);

            Assert.Equal(
                "Informe os seis números do token de acesso.",
                viewModel.Mensagem);

            Assert.True(viewModel.PodeInteragir);
        }

        #endregion


        #region Teste token com letras

        [Fact]
        public void EntrarCommand_TokenComLetras_DeveExibirErro()
        {
            // Arrange
            var viewModel = new KinectLoginViewModel();

            viewModel.TokenAcesso = "12AB56";

            // Act
            viewModel.EntrarCommand.Execute(null);

            // Assert
            Assert.False(viewModel.MensagemSucesso);

            Assert.Equal(
                "Informe os seis números do token de acesso.",
                viewModel.Mensagem);
        }

        #endregion


        #region Teste token vazio

        [Fact]
        public void EntrarCommand_TokenVazio_DeveExibirErro()
        {
            // Arrange
            var viewModel = new KinectLoginViewModel();

            viewModel.TokenAcesso = string.Empty;

            // Act
            viewModel.EntrarCommand.Execute(null);

            // Assert
            Assert.False(viewModel.MensagemSucesso);

            Assert.Equal(
                "Informe os seis números do token de acesso.",
                viewModel.Mensagem);
        }

        #endregion


        #region Teste e-mail vazio

        [Fact]
        public void SolicitarTokenCommand_EmailVazio_DeveExibirErro()
        {
            // Arrange
            var viewModel = new KinectLoginViewModel();

            viewModel.EmailCadastro = string.Empty;

            // Act
            viewModel.SolicitarTokenCommand.Execute(null);

            // Assert
            Assert.False(viewModel.MensagemSucesso);

            Assert.Equal(
                "Informe o e-mail cadastrado.",
                viewModel.Mensagem);

            Assert.True(viewModel.PodeInteragir);
        }

        #endregion


        #region Teste e-mail somente com espaços

        [Fact]
        public void SolicitarTokenCommand_EmailEmBranco_DeveExibirErro()
        {
            // Arrange
            var viewModel = new KinectLoginViewModel();

            viewModel.EmailCadastro = "   ";

            // Act
            viewModel.SolicitarTokenCommand.Execute(null);

            // Assert
            Assert.False(viewModel.MensagemSucesso);

            Assert.Equal(
                "Informe o e-mail cadastrado.",
                viewModel.Mensagem);
        }

        #endregion


        #region Teste limpeza da mensagem ao mudar para Login

        [Fact]
        public void MostrarLoginCommand_AoExecutar_DeveLimparMensagemAnterior()
        {
            // Arrange
            var viewModel = new KinectLoginViewModel();

            viewModel.TokenAcesso = "";

            viewModel.EntrarCommand.Execute(null);

            Assert.NotEmpty(viewModel.Mensagem);

            // Act
            viewModel.MostrarLoginCommand.Execute(null);

            // Assert
            Assert.Equal(string.Empty, viewModel.Mensagem);
            Assert.False(viewModel.MensagemSucesso);
        }

        #endregion

        [Fact]
        public async Task EntrarAsync_TokenValido_DeveCriarSessaoEAbrirMonitor()
        {
            // Arrange
            string tokenRecebido = null;
            SessaoUsuario sessaoAberta = null;
            var autenticacao = new AutenticacaoMvcServiceFake
            {
                AoValidarToken = token =>
                {
                    tokenRecebido = token;
                    return Task.FromResult(new ValidacaoTokenResultado
                    {
                        TokenValido = true,
                        Usuario = "Maria",
                        Empresa = "Estoque A",
                        Email = "maria@empresa.com"
                    });
                }
            };
            var viewModel = new KinectLoginViewModel(
                autenticacao,
                sessao => sessaoAberta = sessao);
            viewModel.TokenAcesso = " 123456 ";

            // Act
            await viewModel.EntrarAsync();

            // Assert
            Assert.Equal("123456", tokenRecebido);
            Assert.NotNull(sessaoAberta);
            Assert.Equal("Maria", sessaoAberta.Usuario);
            Assert.Equal("Estoque A", sessaoAberta.Empresa);
            Assert.Equal("maria@empresa.com", sessaoAberta.Email);
            Assert.Equal("123456", sessaoAberta.Token);
            Assert.True(viewModel.PodeInteragir);
        }

        [Fact]
        public async Task EntrarAsync_TokenRecusado_DeveExibirMensagemDoServico()
        {
            // Arrange
            var autenticacao = new AutenticacaoMvcServiceFake
            {
                AoValidarToken = token => Task.FromResult(new ValidacaoTokenResultado
                {
                    TokenValido = false,
                    Mensagem = "Token expirado."
                })
            };
            var viewModel = new KinectLoginViewModel(autenticacao, sessao => { });
            viewModel.TokenAcesso = "123456";

            // Act
            await viewModel.EntrarAsync();

            // Assert
            Assert.Equal("Token expirado.", viewModel.Mensagem);
            Assert.False(viewModel.MensagemSucesso);
            Assert.True(viewModel.PodeInteragir);
        }

        [Fact]
        public async Task EntrarAsync_QuandoServicoFalha_DeveExibirErroGenerico()
        {
            // Arrange
            var autenticacao = new AutenticacaoMvcServiceFake
            {
                AoValidarToken = token => throw new InvalidOperationException("Falha simulada")
            };
            var viewModel = new KinectLoginViewModel(autenticacao, sessao => { });
            viewModel.TokenAcesso = "123456";

            // Act
            await viewModel.EntrarAsync();

            // Assert
            Assert.Equal("Erro ao validar token no sistema.", viewModel.Mensagem);
            Assert.True(viewModel.PodeInteragir);
        }

        [Fact]
        public async Task SolicitarTokenAsync_Sucesso_DeveIrParaLoginEExibirConfirmacao()
        {
            // Arrange
            string emailRecebido = null;
            var autenticacao = new AutenticacaoMvcServiceFake
            {
                AoSolicitarToken = email =>
                {
                    emailRecebido = email;
                    return Task.FromResult(new TokenSolicitadoResultado { Sucesso = true });
                }
            };
            var viewModel = new KinectLoginViewModel(autenticacao, sessao => { });
            viewModel.EmailCadastro = " usuario@empresa.com ";

            // Act
            await viewModel.SolicitarTokenAsync();

            // Assert
            Assert.Equal("usuario@empresa.com", emailRecebido);
            Assert.True(viewModel.AbaLoginSelecionada);
            Assert.True(viewModel.MensagemSucesso);
            Assert.Contains("Token enviado", viewModel.Mensagem);
            Assert.True(viewModel.PodeInteragir);
        }

        [Fact]
        public async Task SolicitarTokenAsync_Recusado_DeveExibirMensagemDoServico()
        {
            // Arrange
            var autenticacao = new AutenticacaoMvcServiceFake
            {
                AoSolicitarToken = email => Task.FromResult(new TokenSolicitadoResultado
                {
                    Sucesso = false,
                    Mensagem = "E-mail não cadastrado."
                })
            };
            var viewModel = new KinectLoginViewModel(autenticacao, sessao => { });
            viewModel.EmailCadastro = "naoexiste@empresa.com";

            // Act
            await viewModel.SolicitarTokenAsync();

            // Assert
            Assert.Equal("E-mail não cadastrado.", viewModel.Mensagem);
            Assert.False(viewModel.MensagemSucesso);
            Assert.False(viewModel.AbaLoginSelecionada);
        }

        [Fact]
        public async Task SolicitarTokenAsync_QuandoServicoFalha_DeveRestaurarInteracao()
        {
            // Arrange
            var autenticacao = new AutenticacaoMvcServiceFake
            {
                AoSolicitarToken = email => throw new InvalidOperationException("Falha simulada")
            };
            var viewModel = new KinectLoginViewModel(autenticacao, sessao => { });
            viewModel.EmailCadastro = "usuario@empresa.com";

            // Act
            await viewModel.SolicitarTokenAsync();

            // Assert
            Assert.Equal("Erro ao solicitar token no MVC.", viewModel.Mensagem);
            Assert.True(viewModel.PodeInteragir);
        }
    }
}
