using System;
using TCC_Inventory_Masters_Kinect.Service;
using Xunit;

namespace TCC_Inventory_Masters_Kinect.Danilo.Tests.Service_Test_MVVM
{
    public class KinectServiceTests
    {
        


        #region TestesDeConectividade

        /// <summary>
        /// Verifica se IsConnected retorna false antes
        /// da inicialização do Kinect.
        /// </summary>
        [Fact]
        public void IsConnected_AntesDeIniciar_DeveRetornarFalse()
        {
            // Arrange
            var service = new KinectService();

            // Act
            bool conectado = service.IsConnected;

            // Assert
            Assert.False(conectado);
        }

        /// <summary>
        /// Verifica se o serviço permanece desconectado
        /// depois de Stop ser chamado sem inicialização.
        /// </summary>
        [Fact]
        public void IsConnected_AposStopSemStart_DeveRetornarFalse()
        {
            // Arrange
            var service = new KinectService();

            // Act
            service.Stop();

            // Assert
            Assert.False(service.IsConnected);
        }

        #endregion

        #region TestesStart

        /// <summary>
        /// Verifica se Start lança InvalidOperationException
        /// quando nenhum Kinect está conectado.
        /// </summary>
        [Fact]
        public void Start_SemKinectConectado_DeveLancarInvalidOperationException()
        {
            // Arrange
            var service = new KinectService();

            Action acao = service.Start;

            // Act
            InvalidOperationException excecao =
                Assert.Throws<InvalidOperationException>(acao);

            // Assert
            Assert.Equal(
                "Nenhum Kinect conectado foi encontrado.",
                excecao.Message);

            Assert.False(service.IsConnected);
        }

        #endregion

        #region TestesDeEncerramento

        /// <summary>
        /// Verifica se Stop pode ser chamado antes de Start
        /// sem lançar exceção.
        /// </summary>
        [Fact]
        public void Stop_SemStartAnterior_NaoDeveLancarExcecao()
        {
            // Arrange
            var service = new KinectService();

            Action acao = service.Stop;

            // Act
            Exception excecao = Record.Exception(acao);

            // Assert
            Assert.Null(excecao);
            Assert.False(service.IsConnected);
        }

        /// <summary>
        /// Verifica se Stop pode ser chamado duas vezes
        /// sem lançar exceção.
        /// </summary>
        [Fact]
        public void Stop_ChamadoDuasVezes_NaoDeveLancarExcecao()
        {
            // Arrange
            var service = new KinectService();

            Action acao = () =>
            {
                service.Stop();
                service.Stop();
            };

            // Act
            Exception excecao = Record.Exception(acao);

            // Assert
            Assert.Null(excecao);
            Assert.False(service.IsConnected);
        }

        #endregion
    }
}