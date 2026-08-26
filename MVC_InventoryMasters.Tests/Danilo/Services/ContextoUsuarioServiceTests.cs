using Microsoft.AspNetCore.Http;
using MVC_InventoryMasters.Services;
using System.Security.Claims;
using Xunit;

namespace MVC_InventoryMasters.Danilo.Tests.Service_Test_MVC
{
    /// <summary>
    /// Testes da identificação da empresa, usuário,
    /// perfil e estado de autenticação.
    /// </summary>
    [Trait("Integrante", "Danilo")]
    public class ContextoUsuarioServiceTests
    {
        #region Identificação da empresa

        /// <summary>
        /// Verifica se a empresa é obtida pelo claim.
        /// </summary>
        [Fact]
        public void ObterEmpresa_ComClaim_RetornaEmpresaDoUsuario()
        {
            // Arrange
            var contexto = CriarContexto(
                true,
                new Claim("EmpresaId", "empresa-123"));

            var service = CriarService(contexto);

            // Act
            string resultado = service.ObterEmpresaId();

            // Assert
            Assert.Equal("empresa-123", resultado);
        }

        /// <summary>
        /// Verifica se a empresa é obtida pelo cabeçalho
        /// quando o claim não está disponível.
        /// </summary>
        [Fact]
        public void ObterEmpresa_ComCabecalho_RetornaEmpresaInformada()
        {
            // Arrange
            var contexto = CriarContexto();

            contexto.Request.Headers["X-Empresa-Id"] =
                "empresa-header";

            var service = CriarService(contexto);

            // Act
            string resultado = service.ObterEmpresaId();

            // Assert
            Assert.Equal("empresa-header", resultado);
        }

        /// <summary>
        /// Verifica se o claim possui prioridade
        /// sobre o cabeçalho da requisição.
        /// </summary>
        [Fact]
        public void ObterEmpresa_ComClaimECabecalho_PriorizaClaim()
        {
            // Arrange
            var contexto = CriarContexto(
                true,
                new Claim("EmpresaId", "empresa-claim"));

            contexto.Request.Headers["X-Empresa-Id"] =
                "empresa-header";

            var service = CriarService(contexto);

            // Act
            string resultado = service.ObterEmpresaId();

            // Assert
            Assert.Equal("empresa-claim", resultado);
        }

        /// <summary>
        /// Verifica se a empresa global é utilizada
        /// quando nenhuma empresa foi informada.
        /// </summary>
        [Fact]
        public void ObterEmpresa_SemInformacao_RetornaGlobal()
        {
            // Arrange
            var contexto = CriarContexto();
            var service = CriarService(contexto);

            // Act
            string resultado = service.ObterEmpresaId();

            // Assert
            Assert.Equal(
                ContextoUsuarioService.EmpresaPadraoId,
                resultado);
        }

        /// <summary>
        /// Verifica o retorno quando não existe contexto HTTP.
        /// </summary>
        [Fact]
        public void ObterEmpresa_SemContexto_RetornaGlobal()
        {
            // Arrange
            var service = CriarService(null);

            // Act
            string resultado = service.ObterEmpresaId();

            // Assert
            Assert.Equal(
                ContextoUsuarioService.EmpresaPadraoId,
                resultado);
        }

        #endregion

        #region Identificação do usuário

        /// <summary>
        /// Verifica se o identificador do usuário
        /// é obtido pelo claim correto.
        /// </summary>
        [Fact]
        public void ObterUsuario_ComIdentificador_RetornaId()
        {
            // Arrange
            var contexto = CriarContexto(
                true,
                new Claim(
                    ClaimTypes.NameIdentifier,
                    "usuario-123"));

            var service = CriarService(contexto);

            // Act
            string? resultado = service.ObterUsuarioId();

            // Assert
            Assert.Equal("usuario-123", resultado);
        }

        /// <summary>
        /// Verifica o retorno quando o claim
        /// do usuário não está disponível.
        /// </summary>
        [Fact]
        public void ObterUsuario_SemIdentificador_RetornaNull()
        {
            // Arrange
            var contexto = CriarContexto();
            var service = CriarService(contexto);

            // Act
            string? resultado = service.ObterUsuarioId();

            // Assert
            Assert.Null(resultado);
        }

        #endregion

        #region Identificação do perfil

        /// <summary>
        /// Verifica se o perfil é obtido pelo claim de função.
        /// </summary>
        [Fact]
        public void ObterPerfil_ComRole_RetornaPerfil()
        {
            // Arrange
            var contexto = CriarContexto(
                true,
                new Claim(
                    ClaimTypes.Role,
                    "Administrador"));

            var service = CriarService(contexto);

            // Act
            string? resultado = service.ObterPerfil();

            // Assert
            Assert.Equal("Administrador", resultado);
        }

        /// <summary>
        /// Verifica o claim alternativo Perfil.
        /// </summary>
        [Fact]
        public void ObterPerfil_ComClaimPerfil_RetornaPerfil()
        {
            // Arrange
            var contexto = CriarContexto(
                true,
                new Claim("Perfil", "Operador"));

            var service = CriarService(contexto);

            // Act
            string? resultado = service.ObterPerfil();

            // Assert
            Assert.Equal("Operador", resultado);
        }

        /// <summary>
        /// Verifica se o claim Role possui prioridade
        /// sobre o claim alternativo Perfil.
        /// </summary>
        [Fact]
        public void ObterPerfil_ComDoisClaims_PriorizaRole()
        {
            // Arrange
            var contexto = CriarContexto(
                true,
                new Claim(
                    ClaimTypes.Role,
                    "Administrador"),
                new Claim("Perfil", "Operador"));

            var service = CriarService(contexto);

            // Act
            string? resultado = service.ObterPerfil();

            // Assert
            Assert.Equal("Administrador", resultado);
        }

        /// <summary>
        /// Verifica o retorno quando não existe perfil.
        /// </summary>
        [Fact]
        public void ObterPerfil_SemClaim_RetornaNull()
        {
            // Arrange
            var contexto = CriarContexto();
            var service = CriarService(contexto);

            // Act
            string? resultado = service.ObterPerfil();

            // Assert
            Assert.Null(resultado);
        }

        #endregion

        #region Estado de autenticação

        /// <summary>
        /// Verifica um usuário autenticado.
        /// </summary>
        [Fact]
        public void UsuarioAutenticado_ComIdentidadeValida_RetornaTrue()
        {
            // Arrange
            var contexto = CriarContexto(
                true,
                new Claim(
                    ClaimTypes.NameIdentifier,
                    "usuario-123"));

            var service = CriarService(contexto);

            // Act
            bool resultado =
                service.UsuarioEstaAutenticado();

            // Assert
            Assert.True(resultado);
        }

        /// <summary>
        /// Verifica uma identidade não autenticada.
        /// </summary>
        [Fact]
        public void UsuarioAutenticado_SemAutenticacao_RetornaFalse()
        {
            // Arrange
            var contexto = CriarContexto(false);
            var service = CriarService(contexto);

            // Act
            bool resultado =
                service.UsuarioEstaAutenticado();

            // Assert
            Assert.False(resultado);
        }

        /// <summary>
        /// Verifica o resultado quando não existe contexto HTTP.
        /// </summary>
        [Fact]
        public void UsuarioAutenticado_SemContexto_RetornaFalse()
        {
            // Arrange
            var service = CriarService(null);

            // Act
            bool resultado =
                service.UsuarioEstaAutenticado();

            // Assert
            Assert.False(resultado);
        }

        #endregion

        #region Métodos auxiliares dos testes

        private static ContextoUsuarioService CriarService(
            HttpContext? contexto)
        {
            var accessor = new HttpContextAccessor
            {
                HttpContext = contexto
            };

            return new ContextoUsuarioService(accessor);
        }

        private static DefaultHttpContext CriarContexto(
            bool autenticado = true,
            params Claim[] claims)
        {
            ClaimsIdentity identidade;

            if (autenticado)
            {
                identidade = new ClaimsIdentity(
                    claims,
                    "AutenticacaoTeste");
            }
            else
            {
                identidade = new ClaimsIdentity(claims);
            }

            return new DefaultHttpContext
            {
                User = new ClaimsPrincipal(identidade)
            };
        }

        #endregion
    }
}
