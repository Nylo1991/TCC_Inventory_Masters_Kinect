using TCC_Inventory_Masters_Kinect.ViewModel;
using Xunit;

namespace TCC_Inventory_Masters_Kinect.Diulie.Tests.ViewModel
{
    [Xunit.Trait("Integrante", "Diulie")]
    public class MainViewModelEspacoTests
    {
        [Fact]
        public void SalvarEspaco_SemNome_DeveRecusar()
        {
            // Arrange
            var viewModel = ViewModelFactory.CriarMainViewModel();
            viewModel.PercentualAlerta = "80";
            viewModel.VolumeMaximoCm3 = 1000000;

            // Act
            viewModel.SalvarEspacoCommand.Execute(null);

            // Assert
            Assert.False(viewModel.EspacoSalvo);
            Assert.Equal("Informe o nome do espaço.", viewModel.MensagemEspaco);
        }

        [Fact]
        public void SalvarEspaco_SemPercentual_DeveRecusar()
        {
            // Arrange
            var viewModel = ViewModelFactory.CriarMainViewModel();
            viewModel.NomeEspaco = "Depósito";
            viewModel.VolumeMaximoCm3 = 1000000;

            // Act
            viewModel.SalvarEspacoCommand.Execute(null);

            // Assert
            Assert.False(viewModel.EspacoSalvo);
            Assert.Equal("Informe o limite de ocupação.", viewModel.MensagemEspaco);
        }

        [Fact]
        public void SalvarEspaco_PercentualNaoNumerico_DeveRecusar()
        {
            // Arrange
            var viewModel = ViewModelFactory.CriarMainViewModel();
            viewModel.NomeEspaco = "Depósito";
            viewModel.PercentualAlerta = "oitenta";
            viewModel.VolumeMaximoCm3 = 1000000;

            // Act
            viewModel.SalvarEspacoCommand.Execute(null);

            // Assert
            Assert.False(viewModel.EspacoSalvo);
            Assert.Equal("Informe um limite de ocupação válido.", viewModel.MensagemEspaco);
        }

        [Theory]
        [InlineData("0")]
        [InlineData("-1")]
        [InlineData("101")]
        public void SalvarEspaco_PercentualForaDoIntervalo_DeveRecusar(string percentual)
        {
            // Arrange
            var viewModel = ViewModelFactory.CriarMainViewModel();
            viewModel.NomeEspaco = "Depósito";
            viewModel.PercentualAlerta = percentual;
            viewModel.VolumeMaximoCm3 = 1000000;

            // Act
            viewModel.SalvarEspacoCommand.Execute(null);

            // Assert
            Assert.False(viewModel.EspacoSalvo);
            Assert.Contains("entre 1% e 100%", viewModel.MensagemEspaco);
        }

        [Fact]
        public void SalvarEspaco_SemCalibracao_DeveRecusar()
        {
            // Arrange
            var viewModel = ViewModelFactory.CriarMainViewModel();
            viewModel.NomeEspaco = "Depósito";
            viewModel.PercentualAlerta = "80";
            viewModel.VolumeMaximoCm3 = 0;

            // Act
            viewModel.SalvarEspacoCommand.Execute(null);

            // Assert
            Assert.False(viewModel.EspacoSalvo);
            Assert.Equal("Calibre o espaço antes de salvar.", viewModel.MensagemEspaco);
        }

        [Theory]
        [InlineData("1")]
        [InlineData("80")]
        [InlineData("100")]
        public void SalvarEspaco_DadosValidosInclusiveLimites_DeveSalvar(string percentual)
        {
            // Arrange
            var viewModel = ViewModelFactory.CriarMainViewModel();
            viewModel.NomeEspaco = "Depósito";
            viewModel.PercentualAlerta = percentual;
            viewModel.VolumeMaximoCm3 = 2000000;

            // Act
            viewModel.SalvarEspacoCommand.Execute(null);

            // Assert
            Assert.True(viewModel.EspacoSalvo);
            Assert.Contains("Espaço salvo", viewModel.MensagemEspaco);
            Assert.Equal("Espaço salvo com sucesso.", viewModel.StatusMessage);

            viewModel.DesligarMonitoramento();
        }
    }
}
