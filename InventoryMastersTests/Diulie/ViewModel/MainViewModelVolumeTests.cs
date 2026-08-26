using System.Threading.Tasks;
using TCC_Inventory_Masters_Kinect.ViewModel;
using Xunit;

namespace TCC_Inventory_Masters_Kinect.Diulie.Tests.ViewModel
{
    [Xunit.Trait("Integrante", "Diulie")]
    public class MainViewModelVolumeTests
    {
        [Theory]
        [InlineData(0, "0.000 m3")]
        [InlineData(1000000, "1.000 m3")]
        [InlineData(2500000, "2.500 m3")]
        public void FormatarVolumeM3_DeveConverterCentimetrosCubicos(
            double volumeCm3,
            string esperadoInvariante)
        {
            // Arrange
            string esperado = esperadoInvariante.Replace(
                ".",
                System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);

            // Act
            string resultado = MainViewModel.FormatarVolumeM3(volumeCm3);

            // Assert
            Assert.Equal(esperado, resultado);
        }

        [Fact]
        public void AtualizarIndicadoresVolume_SemVolumeMaximo_DeveManterValoresSeguros()
        {
            // Arrange
            var viewModel = ViewModelFactory.CriarMainViewModel();
            viewModel.VolumeMaximoCm3 = 0;

            // Act
            viewModel.AtualizarIndicadoresVolume(500000);

            // Assert
            Assert.Equal("0%", viewModel.PercentualOcupacaoTexto);
            Assert.Equal("0.000 m3", viewModel.EspacoLivreTexto);
            Assert.Equal("Normal", viewModel.StatusAlertaTexto);
        }

        [Fact]
        public void AtualizarIndicadoresVolume_AbaixoDoLimite_DeveCalcularOcupacaoELivre()
        {
            // Arrange
            var viewModel = ViewModelFactory.CriarMainViewModel();
            viewModel.VolumeMaximoCm3 = 2000000;
            viewModel.PercentualAlerta = "80";

            // Act
            viewModel.AtualizarIndicadoresVolume(1000000);

            // Assert
            Assert.Equal("50.0%".Replace(".", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator), viewModel.PercentualOcupacaoTexto);
            Assert.Equal(MainViewModel.FormatarVolumeM3(1000000), viewModel.EspacoLivreTexto);
            Assert.Equal("Normal", viewModel.StatusAlertaTexto);
        }

        [Fact]
        public void AtualizarIndicadoresVolume_AcimaDoLimite_DeveSinalizarLimite()
        {
            // Arrange
            var viewModel = ViewModelFactory.CriarMainViewModel();
            viewModel.VolumeMaximoCm3 = 2000000;
            viewModel.PercentualAlerta = "80";

            // Act
            viewModel.AtualizarIndicadoresVolume(1800000);

            // Assert
            Assert.Equal("Limite", viewModel.StatusAlertaTexto);
            Assert.Equal("90.0%".Replace(".", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator), viewModel.PercentualOcupacaoTexto);
        }

        [Fact]
        public void AtualizarIndicadoresVolume_AcimaDaCapacidade_DeveLimitarEmCemPorCento()
        {
            // Arrange
            var viewModel = ViewModelFactory.CriarMainViewModel();
            viewModel.VolumeMaximoCm3 = 1000000;

            // Act
            viewModel.AtualizarIndicadoresVolume(1500000);

            // Assert
            Assert.Equal("100.0%".Replace(".", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator), viewModel.PercentualOcupacaoTexto);
            Assert.Equal(MainViewModel.FormatarVolumeM3(0), viewModel.EspacoLivreTexto);
        }

        [Fact]
        public async Task MedirSalvarEEnviarAsync_KinectDesconectado_DeveRecusarMedicao()
        {
            // Arrange
            var repository = new KinectRepositoryFake();
            var viewModel = ViewModelFactory.CriarMainViewModel(repository);

            // Act
            await viewModel.MedirSalvarEEnviarAsync("Medição manual");

            // Assert
            Assert.Equal("Kinect não está conectado.", viewModel.StatusMessage);
            Assert.Null(repository.MedicaoSalva);
        }
    }
}
