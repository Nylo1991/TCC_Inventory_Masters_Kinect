using System;
using TCC_Inventory_Masters_Kinect.Service;
using Xunit;

namespace TCC_Inventory_Masters_Kinect.Danilo.Tests.Service_Test_MVVM
{
    /// <summary>
    /// Testes das funcionalidades de captura de câmera
    /// e profundidade do KinectService.
    /// </summary>
    [Trait("Integrante", "Danilo")]
    public class KinectServiceCameraTests
    {
        #region CapturarFrameCamera

        /// <summary>
        /// Verifica se a captura RGB retorna null
        /// quando o Kinect não está conectado.
        /// </summary>
        [Fact]
        public void CapturarFrameCamera_SemKinectConectado_DeveRetornarNull()
        {
            // Arrange
            var service = new KinectService();

            // Act
            var imagem = service.CapturarFrameCamera();

            // Assert
            Assert.Null(imagem);
        }

        /// <summary>
        /// Verifica se a tentativa de captura RGB sem Kinect
        /// não lança uma exceção.
        /// </summary>
        [Fact]
        public void CapturarFrameCamera_SemKinectConectado_NaoDeveLancarExcecao()
        {
            // Arrange
            var service = new KinectService();

            // Act
            Exception excecao = Record.Exception(() =>
            {
                service.CapturarFrameCamera();
            });

            // Assert
            Assert.Null(excecao);
        }

        /// <summary>
        /// Verifica se chamadas repetidas de captura RGB
        /// permanecem seguras sem Kinect conectado.
        /// </summary>
        [Fact]
        public void CapturarFrameCamera_ChamadaDuasVezesSemKinect_DeveRetornarNull()
        {
            // Arrange
            var service = new KinectService();

            // Act
            var primeiraCaptura = service.CapturarFrameCamera();
            var segundaCaptura = service.CapturarFrameCamera();

            // Assert
            Assert.Null(primeiraCaptura);
            Assert.Null(segundaCaptura);
        }

        #endregion

        #region CapturarDepthColorido

        /// <summary>
        /// Verifica se a captura de profundidade retorna null
        /// quando o Kinect não está conectado.
        /// </summary>
        [Fact]
        public void CapturarDepthColorido_SemKinectConectado_DeveRetornarNull()
        {
            // Arrange
            var service = new KinectService();

            // Act
            var imagem = service.CapturarDepthColorido();

            // Assert
            Assert.Null(imagem);
        }

        /// <summary>
        /// Verifica se a tentativa de captura de profundidade
        /// sem Kinect não lança uma exceção.
        /// </summary>
        [Fact]
        public void CapturarDepthColorido_SemKinectConectado_NaoDeveLancarExcecao()
        {
            // Arrange
            var service = new KinectService();

            // Act
            Exception excecao = Record.Exception(() =>
            {
                service.CapturarDepthColorido();
            });

            // Assert
            Assert.Null(excecao);
        }

        /// <summary>
        /// Verifica se chamadas repetidas da captura de profundidade
        /// permanecem seguras sem Kinect conectado.
        /// </summary>
        [Fact]
        public void CapturarDepthColorido_ChamadoDuasVezesSemKinect_DeveRetornarNull()
        {
            // Arrange
            var service = new KinectService();

            // Act
            var primeiraCaptura = service.CapturarDepthColorido();
            var segundaCaptura = service.CapturarDepthColorido();

            // Assert
            Assert.Null(primeiraCaptura);
            Assert.Null(segundaCaptura);
        }

        #endregion

        #region Evento CameraFrameAtualizado

        /// <summary>
        /// Verifica se o evento não é disparado quando
        /// nenhuma imagem RGB é capturada.
        /// </summary>
        [Fact]
        public void CameraFrameAtualizado_SemKinectConectado_NaoDeveSerDisparado()
        {
            // Arrange
            var service = new KinectService();
            bool eventoDisparado = false;

            service.CameraFrameAtualizado += imagem =>
            {
                eventoDisparado = true;
            };

            // Act
            var resultado = service.CapturarFrameCamera();

            // Assert
            Assert.Null(resultado);
            Assert.False(eventoDisparado);
        }

        #endregion
    }
}
