using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MVC_InventoryMasters.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace MVC_InventoryMasters.Danilo.Tests.Service_Test_MVC
{
    /// <summary>
    /// Testes do comportamento do serviço de e-mail
    /// quando o SMTP não está configurado.
    /// </summary>
    [Trait("Integrante", "Danilo")]
    public class EmailTokenServiceTests
    {
        #region Configuração SMTP ausente

        /// <summary>
        /// Verifica se a ausência completa da configuração
        /// não provoca uma exceção.
        /// </summary>
        [Fact]
        public async Task EnviarToken_SmtpNaoConfigurado_NaoLancaExcecao()
        {
            // Arrange
            IConfiguration configuracao =
                new ConfigurationBuilder().Build();

            var logger = new LoggerFake<EmailTokenService>();

            var service = new EmailTokenService(
                configuracao,
                logger);

            // Act
            Exception? excecao =
                await Record.ExceptionAsync(() =>
                    service.EnviarTokenKinect(
                        "usuario@teste.com",
                        "Usuário Teste",
                        "123456",
                        15));

            // Assert
            Assert.Null(excecao);
        }

        /// <summary>
        /// Verifica se a ausência do servidor SMTP
        /// gera um aviso.
        /// </summary>
        [Fact]
        public async Task EnviarToken_SemHost_RegistraAviso()
        {
            // Arrange
            IConfiguration configuracao =
                CriarConfiguracao(host: null);

            var logger = new LoggerFake<EmailTokenService>();

            var service = new EmailTokenService(
                configuracao,
                logger);

            // Act
            await service.EnviarTokenKinect(
                "usuario@teste.com",
                "Usuário Teste",
                "123456",
                15);

            // Assert
            var registro = Assert.Single(logger.Registros);

            Assert.Equal(LogLevel.Warning, registro.Nivel);
        }

        /// <summary>
        /// Verifica se a ausência do remetente
        /// gera um aviso.
        /// </summary>
        [Fact]
        public async Task EnviarToken_SemRemetente_RegistraAviso()
        {
            // Arrange
            IConfiguration configuracao =
                CriarConfiguracao(remetente: null);

            var logger = new LoggerFake<EmailTokenService>();

            var service = new EmailTokenService(
                configuracao,
                logger);

            // Act
            await service.EnviarTokenKinect(
                "usuario@teste.com",
                "Usuário Teste",
                "123456",
                15);

            // Assert
            var registro = Assert.Single(logger.Registros);

            Assert.Equal(LogLevel.Warning, registro.Nivel);
        }

        /// <summary>
        /// Verifica se a ausência do usuário SMTP
        /// gera um aviso.
        /// </summary>
        [Fact]
        public async Task EnviarToken_SemUsuario_RegistraAviso()
        {
            // Arrange
            IConfiguration configuracao =
                CriarConfiguracao(usuario: null);

            var logger = new LoggerFake<EmailTokenService>();

            var service = new EmailTokenService(
                configuracao,
                logger);

            // Act
            await service.EnviarTokenKinect(
                "usuario@teste.com",
                "Usuário Teste",
                "123456",
                15);

            // Assert
            var registro = Assert.Single(logger.Registros);

            Assert.Equal(LogLevel.Warning, registro.Nivel);
        }

        /// <summary>
        /// Verifica se a ausência da senha SMTP
        /// gera um aviso.
        /// </summary>
        [Fact]
        public async Task EnviarToken_SemSenha_RegistraAviso()
        {
            // Arrange
            IConfiguration configuracao =
                CriarConfiguracao(senha: null);

            var logger = new LoggerFake<EmailTokenService>();

            var service = new EmailTokenService(
                configuracao,
                logger);

            // Act
            await service.EnviarTokenKinect(
                "usuario@teste.com",
                "Usuário Teste",
                "123456",
                15);

            // Assert
            var registro = Assert.Single(logger.Registros);

            Assert.Equal(LogLevel.Warning, registro.Nivel);
        }

        #endregion

        #region Registro do token em desenvolvimento

        /// <summary>
        /// Verifica se o aviso contém o e-mail,
        /// token e prazo de validade.
        /// </summary>
        [Fact]
        public async Task EnviarToken_SmtpNaoConfigurado_RegistraDados()
        {
            // Arrange
            IConfiguration configuracao =
                new ConfigurationBuilder().Build();

            var logger = new LoggerFake<EmailTokenService>();

            var service = new EmailTokenService(
                configuracao,
                logger);

            // Act
            await service.EnviarTokenKinect(
                "usuario@teste.com",
                "Usuário Teste",
                "654321",
                30);

            // Assert
            var registro = Assert.Single(logger.Registros);

            Assert.Equal(LogLevel.Warning, registro.Nivel);
            Assert.Contains("usuario@teste.com", registro.Mensagem);
            Assert.Contains("654321", registro.Mensagem);
            Assert.Contains("30", registro.Mensagem);
        }

        #endregion

        #region Métodos auxiliares dos testes

        private static IConfiguration CriarConfiguracao(
            string? host = "smtp.teste.com",
            string? remetente = "inventario@teste.com",
            string? usuario = "usuario-smtp",
            string? senha = "senha-smtp")
        {
            var valores = new Dictionary<string, string?>
            {
                ["Smtp:Host"] = host,
                ["Smtp:From"] = remetente,
                ["Smtp:User"] = usuario,
                ["Smtp:Password"] = senha
            };

            return new ConfigurationBuilder()
                .AddInMemoryCollection(valores)
                .Build();
        }

        private sealed class LoggerFake<T> : ILogger<T>
        {
            public List<(LogLevel Nivel, string Mensagem)>
                Registros
            { get; } = new();

            public IDisposable? BeginScope<TState>(
                TState state)
                where TState : notnull
            {
                return null;
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                return true;
            }

            public void Log<TState>(
                LogLevel logLevel,
                EventId eventId,
                TState state,
                Exception? exception,
                Func<TState, Exception?, string> formatter)
            {
                string mensagem =
                    formatter(state, exception);

                Registros.Add((
                    logLevel,
                    mensagem));
            }
        }

        #endregion
    }
}
