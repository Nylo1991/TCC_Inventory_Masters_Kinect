using System;
using System.Threading.Tasks;
using TCC_Inventory_Masters_Kinect.Model;
using Xunit;

namespace TCC_Inventory_Masters_Kinect.Diulie.Tests.ViewModel
{
    [Xunit.Trait("Integrante", "Diulie")]
    public class MainViewModelInterfaceTests
    {
        [Fact]
        public void InatividadeTimerTick_DeveBloquearSessaoELimparToken()
        {
            // Arrange
            var viewModel = ViewModelFactory.CriarMainViewModel();
            viewModel.TokenDesbloqueio = "999999";

            // Act
            viewModel.InatividadeTimerTick(null, EventArgs.Empty);

            // Assert
            Assert.True(viewModel.SessaoBloqueada);
            Assert.Equal(string.Empty, viewModel.TokenDesbloqueio);
            Assert.Contains("novo token", viewModel.MensagemBloqueio);
        }

        [Fact]
        public async Task DesbloquearSessaoAsync_TokenVazio_DeveManterBloqueio()
        {
            // Arrange
            var viewModel = ViewModelFactory.CriarMainViewModel();
            viewModel.InatividadeTimerTick(null, EventArgs.Empty);
            viewModel.TokenDesbloqueio = " ";

            // Act
            await viewModel.DesbloquearSessaoAsync();

            // Assert
            Assert.True(viewModel.SessaoBloqueada);
            Assert.Equal("Informe o token para desbloquear.", viewModel.MensagemBloqueio);
            Assert.True(viewModel.DesbloquearHabilitado);
        }

        [Fact]
        public async Task DesbloquearSessaoAsync_TokenDeOutroUsuario_DeveRecusar()
        {
            // Arrange
            var autenticacao = new AutenticacaoMvcServiceFake
            {
                AoValidarToken = token => Task.FromResult(new ValidacaoTokenResultado
                {
                    TokenValido = true,
                    Email = "outro@empresa.com",
                    Empresa = "Empresa Teste",
                    Mensagem = "Token pertence a outro usuário."
                })
            };
            var viewModel = ViewModelFactory.CriarMainViewModel(autenticacao: autenticacao);
            viewModel.InatividadeTimerTick(null, EventArgs.Empty);
            viewModel.TokenDesbloqueio = "654321";

            // Act
            await viewModel.DesbloquearSessaoAsync();

            // Assert
            Assert.True(viewModel.SessaoBloqueada);
            Assert.Equal("Token pertence a outro usuário.", viewModel.MensagemBloqueio);
        }

        [Fact]
        public async Task DesbloquearSessaoAsync_MesmaSessao_DeveDesbloquear()
        {
            // Arrange
            var autenticacao = new AutenticacaoMvcServiceFake
            {
                AoValidarToken = token => Task.FromResult(new ValidacaoTokenResultado
                {
                    TokenValido = true,
                    Email = "TESTE@EMPRESA.COM",
                    Empresa = "EMPRESA TESTE"
                })
            };
            var viewModel = ViewModelFactory.CriarMainViewModel(autenticacao: autenticacao);
            viewModel.InatividadeTimerTick(null, EventArgs.Empty);
            viewModel.TokenDesbloqueio = "654321";

            // Act
            await viewModel.DesbloquearSessaoAsync();

            // Assert
            Assert.False(viewModel.SessaoBloqueada);
            Assert.Equal(string.Empty, viewModel.TokenDesbloqueio);
            Assert.Equal(string.Empty, viewModel.MensagemBloqueio);
        }

        [Fact]
        public async Task SolicitarNovoTokenAsync_Sucesso_DeveExibirConfirmacao()
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
            var viewModel = ViewModelFactory.CriarMainViewModel(autenticacao: autenticacao);

            // Act
            await viewModel.SolicitarNovoTokenAsync();

            // Assert
            Assert.Equal("teste@empresa.com", emailRecebido);
            Assert.Contains("Token enviado", viewModel.MensagemBloqueio);
            Assert.True(viewModel.SolicitarNovoTokenHabilitado);
        }

        [Fact]
        public async Task SolicitarNovoTokenAsync_QuandoServicoFalha_DeveExibirErro()
        {
            // Arrange
            var autenticacao = new AutenticacaoMvcServiceFake
            {
                AoSolicitarToken = email => throw new InvalidOperationException("Falha simulada")
            };
            var viewModel = ViewModelFactory.CriarMainViewModel(autenticacao: autenticacao);

            // Act
            await viewModel.SolicitarNovoTokenAsync();

            // Assert
            Assert.Equal("Erro ao solicitar o token de desbloqueio.", viewModel.MensagemBloqueio);
            Assert.True(viewModel.SolicitarNovoTokenHabilitado);
        }
    }
}
