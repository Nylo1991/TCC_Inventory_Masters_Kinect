using Microsoft.Extensions.Configuration;
using MVC_InventoryMasters.Services;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace MVC_InventoryMasters.Danilo.Tests.Service_Test_MVC
{
    /// <summary>
    /// Testes da validação e localização
    /// do arquivo de credenciais do Firebase.
    /// </summary>
    public class FirebaseServiceTests
    {
        #region Validação das credenciais

        /// <summary>
        /// Verifica se arquivo inexistente
        /// lança FileNotFoundException.
        /// </summary>
        [Fact]
        public void Construtor_ArquivoInexistente_LancaExcecao()
        {
            // Arrange
            string arquivo =
                CriarNomeDeArquivoInexistente();

            IConfiguration configuracao =
                CriarConfiguracao(arquivo);

            Action acao = () =>
            {
                new FirebaseService(configuracao);
            };

            // Act
            FileNotFoundException excecao =
                Assert.Throws<FileNotFoundException>(acao);

            // Assert
            Assert.NotNull(excecao);
        }

        /// <summary>
        /// Verifica se a mensagem informa
        /// o nome do arquivo não encontrado.
        /// </summary>
        [Fact]
        public void Construtor_ArquivoInexistente_InformaNomeDoArquivo()
        {
            // Arrange
            string arquivo =
                CriarNomeDeArquivoInexistente();

            IConfiguration configuracao =
                CriarConfiguracao(arquivo);

            Action acao = () =>
            {
                new FirebaseService(configuracao);
            };

            // Act
            FileNotFoundException excecao =
                Assert.Throws<FileNotFoundException>(acao);

            // Assert
            Assert.Contains(arquivo, excecao.Message);
        }

        /// <summary>
        /// Verifica o comportamento quando o caminho
        /// das credenciais está vazio.
        /// </summary>
        [Fact]
        public void Construtor_CaminhoVazio_LancaExcecao()
        {
            // Arrange
            IConfiguration configuracao =
                CriarConfiguracao(string.Empty);

            Action acao = () =>
            {
                new FirebaseService(configuracao);
            };

            // Act
            FileNotFoundException excecao =
                Assert.Throws<FileNotFoundException>(acao);

            // Assert
            Assert.Contains(
                "Arquivo de credenciais não encontrado",
                excecao.Message);
        }

        /// <summary>
        /// Verifica o comportamento quando o caminho
        /// das credenciais é nulo.
        /// </summary>
        [Fact]
        public void Construtor_CaminhoNulo_LancaExcecao()
        {
            // Arrange
            IConfiguration configuracao =
                CriarConfiguracao(null);

            Action acao = () =>
            {
                new FirebaseService(configuracao);
            };

            // Act e Assert
            Assert.Throws<ArgumentNullException>(acao);
        }

        #endregion

        #region Resolução do caminho

        /// <summary>
        /// Verifica se caminhos relativos são resolvidos
        /// a partir do diretório da aplicação.
        /// </summary>
        [Fact]
        public void Construtor_CaminhoRelativo_UsaDiretorioDaAplicacao()
        {
            // Arrange
            string arquivo =
                CriarNomeDeArquivoInexistente();

            string caminhoEsperado = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                arquivo);

            IConfiguration configuracao =
                CriarConfiguracao(arquivo);

            Action acao = () =>
            {
                new FirebaseService(configuracao);
            };

            // Act
            FileNotFoundException excecao =
                Assert.Throws<FileNotFoundException>(acao);

            // Assert
            Assert.Contains(
                caminhoEsperado,
                excecao.Message);
        }

        #endregion

        #region Proteção do ambiente

        /// <summary>
        /// Verifica se a variável de ambiente não é alterada
        /// quando o arquivo não existe.
        /// </summary>
        [Fact]
        public void Construtor_ArquivoInexistente_NaoAlteraVariavelAmbiente()
        {
            // Arrange
            const string nomeVariavel =
                "GOOGLE_APPLICATION_CREDENTIALS";

            string? valorAnterior =
                Environment.GetEnvironmentVariable(nomeVariavel);

            string arquivo =
                CriarNomeDeArquivoInexistente();

            IConfiguration configuracao =
                CriarConfiguracao(arquivo);

            Action acao = () =>
            {
                new FirebaseService(configuracao);
            };

            try
            {
                // Act
                Assert.Throws<FileNotFoundException>(acao);

                string? valorAtual =
                    Environment.GetEnvironmentVariable(
                        nomeVariavel);

                // Assert
                Assert.Equal(valorAnterior, valorAtual);
            }
            finally
            {
                Environment.SetEnvironmentVariable(
                    nomeVariavel,
                    valorAnterior);
            }
        }

        #endregion

        #region Métodos auxiliares dos testes

        private static IConfiguration CriarConfiguracao(
            string? caminhoCredencial)
        {
            var valores = new Dictionary<string, string?>
            {
                ["Firebase:ProjectId"] =
                    "inventory-masters-teste",

                ["Firebase:CredentialPath"] =
                    caminhoCredencial
            };

            return new ConfigurationBuilder()
                .AddInMemoryCollection(valores)
                .Build();
        }

        private static string CriarNomeDeArquivoInexistente()
        {
            return $"firebase-inexistente-{Guid.NewGuid():N}.json";
        }

        #endregion
    }
}