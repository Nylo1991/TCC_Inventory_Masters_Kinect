using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using TCC_Inventory_Masters_Kinect.Service;
using Xunit;

namespace TCC_Inventory_Masters_Kinect.Danilo.Tests.Service_Test_MVVM
{
    /// <summary>
    /// Testes do estado, validações e encerramento
    /// da comunicação realizada pelo SignalRService.
    /// </summary>
    [Trait("Integrante", "Danilo")]
    public class SignalRServiceTests
    {
        #region Estado inicial da conexão

        /// <summary>
        /// Verifica se o serviço é criado corretamente.
        /// </summary>
        [Fact]
        public void Construtor_DeveCriarServico()
        {
            // Arrange e Act
            var service = new SignalRService();

            // Assert
            Assert.NotNull(service);
        }

        /// <summary>
        /// Verifica se o serviço começa desconectado.
        /// </summary>
        [Fact]
        public void EstadoInicial_DeveSerDesconectado()
        {
            // Arrange
            var service = new SignalRService();

            // Act
            HubConnectionState estado = service.EstadoConexao;

            // Assert
            Assert.Equal(
                HubConnectionState.Disconnected,
                estado);
        }

        /// <summary>
        /// Verifica se a propriedade EstaConectado retorna
        /// falso antes de uma conexão ser estabelecida.
        /// </summary>
        [Fact]
        public void EstaConectado_SemConexao_DeveRetornarFalse()
        {
            // Arrange
            var service = new SignalRService();

            // Act
            bool conectado = service.EstaConectado;

            // Assert
            Assert.False(conectado);
        }

        /// <summary>
        /// Verifica se a conexão é considerada não saudável
        /// quando ainda não foi inicializada.
        /// </summary>
        [Fact]
        public void ConexaoSaudavel_SemConexao_DeveRetornarFalse()
        {
            // Arrange
            var service = new SignalRService();

            // Act
            bool saudavel = service.ConexaoSaudavel();

            // Assert
            Assert.False(saudavel);
        }

        /// <summary>
        /// Verifica se o serviço começa sem mensagem de erro.
        /// </summary>
        [Fact]
        public void UltimoErro_AoCriarServico_DeveEstarVazio()
        {
            // Arrange
            var service = new SignalRService();

            // Act
            string ultimoErro = service.UltimoErro;

            // Assert
            Assert.Equal(string.Empty, ultimoErro);
        }

        #endregion

        #region Envio de volume sem conexão

        /// <summary>
        /// Verifica se o envio de volume retorna falso
        /// quando a conexão não foi inicializada.
        /// </summary>
        [Fact]
        public async Task EnviarVolume_SemConexao_DeveRetornarFalse()
        {
            // Arrange
            var service = new SignalRService();

            // Act
            bool resultado =
                await service.EnviarVolumeAsync(1500);

            // Assert
            Assert.False(resultado);
        }

        /// <summary>
        /// Verifica a mensagem de erro gerada ao tentar
        /// enviar volume sem conexão.
        /// </summary>
        [Fact]
        public async Task EnviarVolume_SemConexao_DeveRegistrarErro()
        {
            // Arrange
            var service = new SignalRService();

            // Act
            await service.EnviarVolumeAsync(1500);

            // Assert
            Assert.Equal(
                "A conexao SignalR ainda nao foi inicializada.",
                service.UltimoErro);
        }

        /// <summary>
        /// Verifica se o evento informa a ausência de conexão
        /// durante a tentativa de envio de volume.
        /// </summary>
        [Fact]
        public async Task EnviarVolume_SemConexao_DeveNotificarEvento()
        {
            // Arrange
            var service = new SignalRService();
            string statusRecebido = string.Empty;

            service.StatusSignalRAtualizado += status =>
            {
                statusRecebido = status;
            };

            // Act
            await service.EnviarVolumeAsync(1500);

            // Assert
            Assert.Equal(
                "SignalR: Sem conexao",
                statusRecebido);
        }

        /// <summary>
        /// Verifica se um volume zero também não é enviado
        /// quando não existe conexão.
        /// </summary>
        [Fact]
        public async Task EnviarVolume_ValorZeroSemConexao_DeveRetornarFalse()
        {
            // Arrange
            var service = new SignalRService();

            // Act
            bool resultado =
                await service.EnviarVolumeAsync(0);

            // Assert
            Assert.False(resultado);
        }

        /// <summary>
        /// Verifica se um volume negativo não é enviado
        /// quando não existe conexão.
        /// </summary>
        [Fact]
        public async Task EnviarVolume_ValorNegativoSemConexao_DeveRetornarFalse()
        {
            // Arrange
            var service = new SignalRService();

            // Act
            bool resultado =
                await service.EnviarVolumeAsync(-100);

            // Assert
            Assert.False(resultado);
        }

        #endregion

        #region Envio de status sem conexão

        /// <summary>
        /// Verifica se o envio de status sem conexão
        /// é concluído sem lançar exceção.
        /// </summary>
        [Fact]
        public async Task EnviarStatus_SemConexao_NaoDeveLancarExcecao()
        {
            // Arrange
            var service = new SignalRService();

            // Act
            Exception excecao = await Record.ExceptionAsync(
                () => service.EnviarStatusAsync("Kinect conectado"));

            // Assert
            Assert.Null(excecao);
        }

        /// <summary>
        /// Verifica a mensagem de erro registrada ao tentar
        /// enviar um status sem conexão.
        /// </summary>
        [Fact]
        public async Task EnviarStatus_SemConexao_DeveRegistrarErro()
        {
            // Arrange
            var service = new SignalRService();

            // Act
            await service.EnviarStatusAsync("Kinect conectado");

            // Assert
            Assert.Equal(
                "A conexao SignalR ainda nao foi inicializada.",
                service.UltimoErro);
        }

        /// <summary>
        /// Verifica se o evento informa a ausência de conexão
        /// durante a tentativa de envio de status.
        /// </summary>
        [Fact]
        public async Task EnviarStatus_SemConexao_DeveNotificarEvento()
        {
            // Arrange
            var service = new SignalRService();
            string statusRecebido = string.Empty;

            service.StatusSignalRAtualizado += status =>
            {
                statusRecebido = status;
            };

            // Act
            await service.EnviarStatusAsync("Kinect conectado");

            // Assert
            Assert.Equal(
                "SignalR: Sem conexao",
                statusRecebido);
        }

        /// <summary>
        /// Verifica o comportamento quando o status informado
        /// é nulo e não existe conexão.
        /// </summary>
        [Fact]
        public async Task EnviarStatus_StatusNuloSemConexao_DeveRegistrarErro()
        {
            // Arrange
            var service = new SignalRService();

            // Act
            await service.EnviarStatusAsync(null!);

            // Assert
            Assert.Equal(
                "A conexao SignalR ainda nao foi inicializada.",
                service.UltimoErro);
        }

        #endregion

        #region Encerramento sem conexão

        /// <summary>
        /// Verifica se a desconexão pode ser chamada antes
        /// da inicialização sem lançar exceção.
        /// </summary>
        [Fact]
        public async Task Desconectar_SemConexao_NaoDeveLancarExcecao()
        {
            // Arrange
            var service = new SignalRService();

            // Act
            Exception excecao = await Record.ExceptionAsync(
                () => service.DesconectarAsync());

            // Assert
            Assert.Null(excecao);
        }

        /// <summary>
        /// Verifica se o estado continua desconectado após
        /// chamar DesconectarAsync sem conexão anterior.
        /// </summary>
        [Fact]
        public async Task Desconectar_SemConexao_DeveContinuarDesconectado()
        {
            // Arrange
            var service = new SignalRService();

            // Act
            await service.DesconectarAsync();

            // Assert
            Assert.Equal(
                HubConnectionState.Disconnected,
                service.EstadoConexao);

            Assert.False(service.EstaConectado);
            Assert.False(service.ConexaoSaudavel());
        }

        /// <summary>
        /// Verifica se a desconexão pode ser chamada duas
        /// vezes sem lançar exceção.
        /// </summary>
        [Fact]
        public async Task Desconectar_DuasVezes_NaoDeveLancarExcecao()
        {
            // Arrange
            var service = new SignalRService();

            // Act
            Exception excecao = await Record.ExceptionAsync(
                async () =>
                {
                    await service.DesconectarAsync();
                    await service.DesconectarAsync();
                });

            // Assert
            Assert.Null(excecao);
            Assert.False(service.EstaConectado);
        }

        #endregion
    }
}
