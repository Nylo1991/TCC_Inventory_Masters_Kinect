using TCC_Inventory_Masters_Kinect.ViewModel;
using Xunit;

namespace TCC_Inventory_Masters_Kinect.Diulie.Tests.ViewModel
{
    [Xunit.Trait("Integrante", "Diulie")]
    public class MainViewModelKinectTests
    {
        [Fact]
        public void DesligarMonitoramento_SemSensorConectado_DeveManterEstadoSeguro()
        {
            // Arrange
            var viewModel = ViewModelFactory.CriarMainViewModel();

            // Act
            viewModel.DesligarMonitoramento();

            // Assert
            Assert.Null(viewModel.CameraImage);
            Assert.Null(viewModel.DepthImage);
            Assert.Equal("Kinect desligado", viewModel.StatusKinect);
            Assert.Equal("Kinect encerrado pelo usuário", viewModel.StatusMessage);
        }
    }
}
