using System;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using TCC_Inventory_Masters_Kinect.Service;
using Xunit;

namespace TCC_Inventory_Masters_Kinect.Danilo.Tests.Service_Test_MVVM

{
    /// <summary>
    /// Testes das validações de autenticação 
    /// realizadas pelo serviço MVC.
    /// </summary>
    public class AutenticacaoMvcServiceTests
    {
        #region Inicialização do serviço

        /// <summary>
        /// Verifica se o serviço cria o HttpClient interno.
        /// </summary>
        [Fact]
        public void Construtor_CriaHttpClient()
        {
            // Arrange e Act
            var service = new AutenticacaoMvcService();

            // Assert
            HttpClient httpClient = ObterHttpClient(service);

            Assert.NotNull(service);
            Assert.NotNull(httpClient);
        }

        /// <summary>
        /// Verifica se o tempo limite foi definido
        /// como quinze segundos.
        /// </summary>
        [Fact]
        public void Construtor_DefineTimeoutDeQuinzeSegundos()
        {
            // Arrange
            var service = new AutenticacaoMvcService();

            // Act
            HttpClient httpClient = ObterHttpClient(service);

            // Assert
            Assert.Equal(
                TimeSpan.FromSeconds(15),
                httpClient.Timeout);
        }

        #endregion

        #region Validação da solicitação de token

        /// <summary>
        /// Verifica o resultado quando o e-mail é nulo.
        /// </summary>
        [Fact]
        public async Task SolicitarToken_EmailNulo_RetornaErro()
        {
            // Arrange
            var service = new AutenticacaoMvcService();

            // Act
            var resultado =
                await service.SolicitarTokenAsync(null!);

            // Assert
            Assert.False(resultado.Sucesso);

            Assert.Equal(
                "Informe o e-mail cadastrado.",
                resultado.Mensagem);
        }

        /// <summary>
        /// Verifica o resultado quando o e-mail está vazio.
        /// </summary>
        [Fact]
        public async Task SolicitarToken_EmailVazio_RetornaErro()
        {
            // Arrange
            var service = new AutenticacaoMvcService();

            // Act
            var resultado =
                await service.SolicitarTokenAsync(string.Empty);

            // Assert
            Assert.False(resultado.Sucesso);

            Assert.Equal(
                "Informe o e-mail cadastrado.",
                resultado.Mensagem);
        }

        /// <summary>
        /// Verifica o resultado quando o e-mail
        /// contém somente espaços.
        /// </summary>
        [Fact]
        public async Task SolicitarToken_EmailComEspacos_RetornaErro()
        {
            // Arrange
            var service = new AutenticacaoMvcService();

            // Act
            var resultado =
                await service.SolicitarTokenAsync("   ");

            // Assert
            Assert.False(resultado.Sucesso);

            Assert.Equal(
                "Informe o e-mail cadastrado.",
                resultado.Mensagem);
        }

        #endregion

        #region Validação do token de acesso

        /// <summary>
        /// Verifica o resultado quando o token é nulo.
        /// </summary>
        [Fact]
        public async Task ValidarToken_TokenNulo_RetornaInvalido()
        {
            // Arrange
            var service = new AutenticacaoMvcService();

            // Act
            var resultado =
                await service.ValidarTokenAsync(null!);

            // Assert
            Assert.False(resultado.TokenValido);
            Assert.False(resultado.EmailValidado);

            Assert.Equal(
                "Informe o token de acesso.",
                resultado.Mensagem);
        }

        /// <summary>
        /// Verifica o resultado quando o token está vazio.
        /// </summary>
        [Fact]
        public async Task ValidarToken_TokenVazio_RetornaInvalido()
        {
            // Arrange
            var service = new AutenticacaoMvcService();

            // Act
            var resultado =
                await service.ValidarTokenAsync(string.Empty);

            // Assert
            Assert.False(resultado.TokenValido);
            Assert.False(resultado.EmailValidado);

            Assert.Equal(
                "Informe o token de acesso.",
                resultado.Mensagem);
        }

        /// <summary>
        /// Verifica o resultado quando o token
        /// contém somente espaços.
        /// </summary>
        [Fact]
        public async Task ValidarToken_TokenComEspacos_RetornaInvalido()
        {
            // Arrange
            var service = new AutenticacaoMvcService();

            // Act
            var resultado =
                await service.ValidarTokenAsync("   ");

            // Assert
            Assert.False(resultado.TokenValido);
            Assert.False(resultado.EmailValidado);

            Assert.Equal(
                "Informe o token de acesso.",
                resultado.Mensagem);
        }

        #endregion

        #region Detalhamento das exceções

        /// <summary>
        /// Verifica a extração de uma mensagem simples.
        /// </summary>
        [Fact]
        public void ObterDetalhesErro_ExcecaoSimples_RetornaMensagem()
        {
            // Arrange
            var excecao =
                new InvalidOperationException("Erro principal.");

            // Act
            string detalhes = InvocarObterDetalhesErro(excecao);

            // Assert
            Assert.Equal("Erro principal.", detalhes);
        }

        /// <summary>
        /// Verifica se as mensagens das exceções
        /// encadeadas são reunidas.
        /// </summary>
        [Fact]
        public void ObterDetalhesErro_ExcecoesEncadeadas_JuntaMensagens()
        {
            // Arrange
            var excecaoInterna =
                new InvalidOperationException("Erro interno.");

            var excecaoExterna =
                new HttpRequestException(
                    "Erro de conexão.",
                    excecaoInterna);

            // Act
            string detalhes =
                InvocarObterDetalhesErro(excecaoExterna);

            // Assert
            Assert.Equal(
                "Erro de conexão. | Erro interno.",
                detalhes);
        }

        #endregion

        #region Métodos auxiliares dos testes

        private static HttpClient ObterHttpClient(
            AutenticacaoMvcService service)
        {
            FieldInfo? campo =
                typeof(AutenticacaoMvcService).GetField(
                    "_httpClient",
                    BindingFlags.Instance |
                    BindingFlags.NonPublic);

            Assert.NotNull(campo);

            object? valor = campo!.GetValue(service);

            Assert.NotNull(valor);

            return (HttpClient)valor!;
        }

        private static string InvocarObterDetalhesErro(
            Exception excecao)
        {
            MethodInfo? metodo =
                typeof(AutenticacaoMvcService).GetMethod(
                    "ObterDetalhesErro",
                    BindingFlags.Static |
                    BindingFlags.NonPublic);

            Assert.NotNull(metodo);

            object? resultado = metodo!.Invoke(
                null,
                new object[] { excecao });

            Assert.NotNull(resultado);

            return (string)resultado!;
        }

        #endregion
    }
}