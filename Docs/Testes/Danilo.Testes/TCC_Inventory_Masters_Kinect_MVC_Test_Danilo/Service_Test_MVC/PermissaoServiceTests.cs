using MVC_InventoryMasters.Models;
using MVC_InventoryMasters.Services;
using Xunit;

namespace MVC_InventoryMasters.Danilo.Tests.Service_Test_MVC
{
    /// <summary>
    /// Testes das permissões padrão e da verificação
    /// de acesso dos diferentes perfis do sistema.
    /// </summary>
    public class PermissaoServiceTests
    {
        #region Permissões padrão por perfil

        /// <summary>
        /// Verifica se o administrador recebe todas as permissões.
        /// </summary>
        [Fact]
        public void ObterPermissoes_Administrador_RetornaTodas()
        {
            // Arrange
            var service = new PermissaoService();

            // Act
            var resultado =
                service.ObterPermissoesPadrao("administrador");

            // Assert
            Assert.Equal(PermissoesSistema.Todas, resultado);
        }

        /// <summary>
        /// Verifica as permissões padrão do gestor.
        /// </summary>
        [Fact]
        public void ObterPermissoes_Gestor_RetornaPermissoesCorretas()
        {
            // Arrange
            var service = new PermissaoService();

            // Act
            var resultado =
                service.ObterPermissoesPadrao("gestor");

            // Assert
            Assert.Equal(4, resultado.Count);

            Assert.Contains(
                PermissoesSistema.DashboardVisualizar,
                resultado);

            Assert.Contains(
                PermissoesSistema.MedicoesVisualizar,
                resultado);

            Assert.Contains(
                PermissoesSistema.NotificacoesVisualizar,
                resultado);

            Assert.Contains(
                PermissoesSistema.ParceirosVisualizar,
                resultado);
        }

        /// <summary>
        /// Verifica as permissões padrão do operador.
        /// </summary>
        [Fact]
        public void ObterPermissoes_Operador_RetornaPermissoesCorretas()
        {
            // Arrange
            var service = new PermissaoService();

            // Act
            var resultado =
                service.ObterPermissoesPadrao("operador");

            // Assert
            Assert.Equal(3, resultado.Count);

            Assert.Contains(
                PermissoesSistema.DashboardVisualizar,
                resultado);

            Assert.Contains(
                PermissoesSistema.MedicoesVisualizar,
                resultado);

            Assert.Contains(
                PermissoesSistema.KinectAcessar,
                resultado);
        }

        /// <summary>
        /// Verifica as permissões padrão do visualizador.
        /// </summary>
        [Fact]
        public void ObterPermissoes_Visualizador_RetornaPermissoesCorretas()
        {
            // Arrange
            var service = new PermissaoService();

            // Act
            var resultado =
                service.ObterPermissoesPadrao("visualizador");

            // Assert
            Assert.Equal(3, resultado.Count);

            Assert.Contains(
                PermissoesSistema.DashboardVisualizar,
                resultado);

            Assert.Contains(
                PermissoesSistema.MedicoesVisualizar,
                resultado);

            Assert.Contains(
                PermissoesSistema.NotificacoesVisualizar,
                resultado);
        }

        /// <summary>
        /// Verifica se um perfil desconhecido
        /// não recebe permissões.
        /// </summary>
        [Fact]
        public void ObterPermissoes_PerfilDesconhecido_RetornaVazio()
        {
            // Arrange
            var service = new PermissaoService();

            // Act
            var resultado =
                service.ObterPermissoesPadrao("perfil-inexistente");

            // Assert
            Assert.Empty(resultado);
        }

        /// <summary>
        /// Verifica se perfil nulo não recebe permissões.
        /// </summary>
        [Fact]
        public void ObterPermissoes_PerfilNulo_RetornaVazio()
        {
            // Arrange
            var service = new PermissaoService();

            // Act
            var resultado =
                service.ObterPermissoesPadrao(null);

            // Assert
            Assert.Empty(resultado);
        }

        /// <summary>
        /// Verifica se perfil vazio não recebe permissões.
        /// </summary>
        [Fact]
        public void ObterPermissoes_PerfilVazio_RetornaVazio()
        {
            // Arrange
            var service = new PermissaoService();

            // Act
            var resultado =
                service.ObterPermissoesPadrao("   ");

            // Assert
            Assert.Empty(resultado);
        }

        /// <summary>
        /// Verifica se diferenças entre letras maiúsculas,
        /// minúsculas e espaços são ignoradas.
        /// </summary>
        [Fact]
        public void ObterPermissoes_PerfilFormatado_ReconhecePerfil()
        {
            // Arrange
            var service = new PermissaoService();

            // Act
            var resultado =
                service.ObterPermissoesPadrao("  GeStOr  ");

            // Assert
            Assert.Equal(4, resultado.Count);

            Assert.Contains(
                PermissoesSistema.ParceirosVisualizar,
                resultado);
        }

        #endregion

        #region Verificação de permissões

        /// <summary>
        /// Verifica uma permissão concedida ao gestor.
        /// </summary>
        [Fact]
        public void PossuiPermissao_GestorAutorizado_RetornaTrue()
        {
            // Arrange
            var service = new PermissaoService();

            // Act
            bool resultado = service.PerfilPossuiPermissao(
                "gestor",
                PermissoesSistema.DashboardVisualizar);

            // Assert
            Assert.True(resultado);
        }

        /// <summary>
        /// Verifica uma permissão não concedida ao gestor.
        /// </summary>
        [Fact]
        public void PossuiPermissao_GestorNaoAutorizado_RetornaFalse()
        {
            // Arrange
            var service = new PermissaoService();

            // Act
            bool resultado = service.PerfilPossuiPermissao(
                "gestor",
                PermissoesSistema.KinectAcessar);

            // Assert
            Assert.False(resultado);
        }

        /// <summary>
        /// Verifica se a comparação da permissão
        /// ignora letras maiúsculas e minúsculas.
        /// </summary>
        [Fact]
        public void PossuiPermissao_TextoEmMinusculo_RetornaTrue()
        {
            // Arrange
            var service = new PermissaoService();

            string permissao =
                PermissoesSistema.KinectAcessar.ToLowerInvariant();

            // Act
            bool resultado = service.PerfilPossuiPermissao(
                "OPERADOR",
                permissao);

            // Assert
            Assert.True(resultado);
        }

        /// <summary>
        /// Verifica se perfil desconhecido
        /// não possui permissão.
        /// </summary>
        [Fact]
        public void PossuiPermissao_PerfilDesconhecido_RetornaFalse()
        {
            // Arrange
            var service = new PermissaoService();

            // Act
            bool resultado = service.PerfilPossuiPermissao(
                "perfil-inexistente",
                PermissoesSistema.DashboardVisualizar);

            // Assert
            Assert.False(resultado);
        }

        /// <summary>
        /// Verifica se perfil nulo não possui permissão.
        /// </summary>
        [Fact]
        public void PossuiPermissao_PerfilNulo_RetornaFalse()
        {
            // Arrange
            var service = new PermissaoService();

            // Act
            bool resultado = service.PerfilPossuiPermissao(
                null,
                PermissoesSistema.DashboardVisualizar);

            // Assert
            Assert.False(resultado);
        }

        #endregion
    }
}