using Microsoft.Extensions.Configuration;
using MVC_InventoryMasters.Services;
using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;

namespace MVC_InventoryMasters.Danilo.Tests.Service_Test_MVC
{
    /// <summary>
    /// Testes da geração e configuração
    /// dos tokens de acesso do Kinect.
    /// </summary>
    [Trait("Integrante", "Danilo")]
    public class TokenAcessoKinectServiceTests
    {
        #region Configuração da validade

        /// <summary>
        /// Verifica se a validade padrão é de 15 minutos.
        /// </summary>
        [Fact]
        public void Construtor_SemConfiguracao_UsaQuinzeMinutos()
        {
            // Arrange
            IConfiguration configuracao =
                new ConfigurationBuilder().Build();

            // Act
            var service = CriarService(configuracao);
            int validade = ObterValidade(service);

            // Assert
            Assert.Equal(15, validade);
        }

        /// <summary>
        /// Verifica se a validade informada
        /// na configuração é utilizada.
        /// </summary>
        [Fact]
        public void Construtor_ComConfiguracao_UsaValidadeInformada()
        {
            // Arrange
            var valores = new Dictionary<string, string?>
            {
                ["KinectAccess:TokenValidityMinutes"] = "30"
            };

            IConfiguration configuracao =
                new ConfigurationBuilder()
                    .AddInMemoryCollection(valores)
                    .Build();

            // Act
            var service = CriarService(configuracao);
            int validade = ObterValidade(service);

            // Assert
            Assert.Equal(30, validade);
        }

        #endregion

        #region Geração do hash

        /// <summary>
        /// Verifica se o mesmo valor produz o mesmo hash.
        /// </summary>
        [Fact]
        public void GerarHash_MesmoValor_RetornaMesmoHash()
        {
            // Act
            string primeiroHash =
                TokenAcessoKinectService.GerarHash("123456");

            string segundoHash =
                TokenAcessoKinectService.GerarHash("123456");

            // Assert
            Assert.Equal(primeiroHash, segundoHash);
        }

        /// <summary>
        /// Verifica se valores diferentes produzem hashes diferentes.
        /// </summary>
        [Fact]
        public void GerarHash_ValoresDiferentes_RetornaHashesDiferentes()
        {
            // Act
            string primeiroHash =
                TokenAcessoKinectService.GerarHash("123456");

            string segundoHash =
                TokenAcessoKinectService.GerarHash("654321");

            // Assert
            Assert.NotEqual(primeiroHash, segundoHash);
        }

        /// <summary>
        /// Verifica se o hash SHA-256 possui
        /// 64 caracteres hexadecimais.
        /// </summary>
        [Fact]
        public void GerarHash_ValorValido_RetornaHashComFormatoCorreto()
        {
            // Act
            string hash =
                TokenAcessoKinectService.GerarHash("TOKEN-123");

            // Assert
            Assert.Equal(64, hash.Length);
            Assert.Matches("^[0-9A-F]{64}$", hash);
        }

        /// <summary>
        /// Compara o resultado com um hash SHA-256 conhecido.
        /// </summary>
        [Fact]
        public void GerarHash_ValorConhecido_RetornaHashEsperado()
        {
            // Arrange
            const string hashEsperado =
                "BA7816BF8F01CFEA414140DE5DAE2223" +
                "B00361A396177A9CB410FF61F20015AD";

            // Act
            string resultado =
                TokenAcessoKinectService.GerarHash("abc");

            // Assert
            Assert.Equal(hashEsperado, resultado);
        }

        /// <summary>
        /// Verifica se valor nulo lança a exceção esperada.
        /// </summary>
        [Fact]
        public void GerarHash_ValorNulo_LancaExcecao()
        {
            // Arrange e Act
            Action acao = () =>
            {
                TokenAcessoKinectService.GerarHash(null!);
            };

            // Assert
            Assert.Throws<ArgumentNullException>(acao);
        }

        #endregion

        #region Geração do token numérico

        /// <summary>
        /// Verifica se o token possui seis números.
        /// </summary>
        [Fact]
        public void GerarToken_RetornaSeisNumeros()
        {
            // Act
            string token = InvocarGerarToken();

            // Assert
            Assert.Equal(6, token.Length);
            Assert.Matches("^[0-9]{6}$", token);
        }

        /// <summary>
        /// Verifica se o token está dentro
        /// do intervalo configurado.
        /// </summary>
        [Fact]
        public void GerarToken_RetornaValorDentroDoIntervalo()
        {
            // Act
            string token = InvocarGerarToken();
            int valor = int.Parse(token);

            // Assert
            Assert.InRange(valor, 100000, 999998);
        }

        /// <summary>
        /// Verifica o formato em várias gerações.
        /// </summary>
        [Fact]
        public void GerarToken_EmVariasTentativas_MantemFormato()
        {
            // Act e Assert
            for (int tentativa = 0; tentativa < 100; tentativa++)
            {
                string token = InvocarGerarToken();

                Assert.Equal(6, token.Length);
                Assert.Matches("^[0-9]{6}$", token);
            }
        }

        #endregion

        #region Métodos auxiliares dos testes

        private static TokenAcessoKinectService CriarService(
            IConfiguration configuracao)
        {
            return new TokenAcessoKinectService(
                null!,
                null!,
                null!,
                configuracao);
        }

        private static int ObterValidade(
            TokenAcessoKinectService service)
        {
            FieldInfo? campo =
                typeof(TokenAcessoKinectService).GetField(
                    "_validadeMinutos",
                    BindingFlags.Instance |
                    BindingFlags.NonPublic);

            Assert.NotNull(campo);

            object? resultado = campo!.GetValue(service);

            Assert.NotNull(resultado);

            return (int)resultado!;
        }

        private static string InvocarGerarToken()
        {
            MethodInfo? metodo =
                typeof(TokenAcessoKinectService).GetMethod(
                    "GerarTokenNumerico",
                    BindingFlags.Static |
                    BindingFlags.NonPublic);

            Assert.NotNull(metodo);

            object? resultado = metodo!.Invoke(null, null);

            Assert.NotNull(resultado);

            return (string)resultado!;
        }

        #endregion
    }
}
