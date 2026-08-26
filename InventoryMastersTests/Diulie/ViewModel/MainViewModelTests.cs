using System;
using System.Collections.Generic;
using TCC_Inventory_Masters_Kinect.Model;
using TCC_Inventory_Masters_Kinect.Service;
using TCC_Inventory_Masters_Kinect.ViewModel;
using Xunit;

namespace TCC_Inventory_Masters_Kinect.Diulie.Tests.ViewModel
{
    [Xunit.Trait("Integrante", "Diulie")]
    public class MainViewModelTests
    {
        public static IEnumerable<object[]> SessoesInvalidas()
        {
            yield return new object[] { null };
            yield return new object[] { new SessaoUsuario() };
            yield return new object[] { new SessaoUsuario { Usuario = "U", Empresa = "E", Email = "a@b.com", Token = "DEV" } };
            yield return new object[] { new SessaoUsuario { Usuario = "", Empresa = "E", Email = "a@b.com", Token = "123456" } };
            yield return new object[] { new SessaoUsuario { Usuario = "U", Empresa = "", Email = "a@b.com", Token = "123456" } };
            yield return new object[] { new SessaoUsuario { Usuario = "U", Empresa = "E", Email = "", Token = "123456" } };
        }

        [Theory]
        [MemberData(nameof(SessoesInvalidas))]
        public void Construtor_SessaoInvalida_DeveRecusarInicializacao(SessaoUsuario sessao)
        {
            // Arrange + Act
            Action acao = () => new MainViewModel(
                sessao,
                new KinectService(),
                new SignalRService(),
                new KinectRepositoryFake(),
                new AutenticacaoMvcServiceFake(),
                false);

            // Assert
            Assert.ThrowsAny<Exception>(acao);
        }

        [Fact]
        public void Construtor_SessaoValida_DeveInicializarIdentidadeEStatus()
        {
            // Arrange + Act
            var viewModel = ViewModelFactory.CriarMainViewModel();

            // Assert
            Assert.Equal("Usuário Teste", viewModel.UsuarioLogado);
            Assert.Equal("Empresa Teste", viewModel.EmpresaLogada);
            Assert.Equal("teste@empresa.com", viewModel.EmailSessao);
            Assert.Equal("Pronto", viewModel.StatusMessage);
            Assert.Equal("Kinect desligado", viewModel.StatusKinect);
            Assert.Equal("0.000 m3", viewModel.VolumeTexto);
            Assert.False(viewModel.EspacoSalvo);
            Assert.False(viewModel.SessaoBloqueada);
        }

        [Fact]
        public void Construtor_SessaoValida_DeveCriarTodosOsComandos()
        {
            // Arrange + Act
            var viewModel = ViewModelFactory.CriarMainViewModel();

            // Assert
            Assert.NotNull(viewModel.LigarKinectCommand);
            Assert.NotNull(viewModel.DesligarKinectCommand);
            Assert.NotNull(viewModel.CalibrarCommand);
            Assert.NotNull(viewModel.MedirCommand);
            Assert.NotNull(viewModel.SalvarEspacoCommand);
            Assert.NotNull(viewModel.AbrirHistoricoCommand);
            Assert.NotNull(viewModel.SolicitarNovoTokenCommand);
            Assert.NotNull(viewModel.DesbloquearSessaoCommand);
            Assert.NotNull(viewModel.IniciarHistoricoCommand);
            Assert.NotNull(viewModel.FecharHistoricoCommand);
        }

        [Fact]
        public void StatusMessage_QuandoAlterado_DeveNotificarPropertyChanged()
        {
            // Arrange
            var viewModel = ViewModelFactory.CriarMainViewModel();
            string propriedade = null;
            viewModel.PropertyChanged += (sender, args) => propriedade = args.PropertyName;

            // Act
            viewModel.StatusMessage = "Novo status";

            // Assert
            Assert.Equal(nameof(MainViewModel.StatusMessage), propriedade);
            Assert.Equal("Novo status", viewModel.StatusMessage);
        }
    }
}
