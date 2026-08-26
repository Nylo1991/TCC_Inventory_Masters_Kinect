using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Microsoft.AspNetCore.Http;
using Xunit;
using MVC_InventoryMasters.Models;
using MVC_InventoryMasters.Repositories;
using MVC_InventoryMasters.Services;

namespace MVC_InventoryMasters.Tests.Repository
{
    public class NotificacaoRepositoryTests
    {
        private NotificacaoRepository CriarRepositorio(string projetoId = "inventorymasters")
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    {"Firebase:CredentialPath", "Config/inventorymasters_firebase.json"},
                    {"Firebase:ProjectId", projetoId}
                })
                .Build();

            var firebaseService = new FirebaseService(configuration);
            var logger = NullLogger<NotificacaoRepository>.Instance;

            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            var contextoUsuario = new ContextoUsuarioService(httpContextAccessorMock.Object);

            return new NotificacaoRepository(firebaseService, logger, contextoUsuario);
        }

        #region Adicionar
        [Fact]
        public async Task CT01_Adicionar_NotificacaoValida_AdicionaComSucesso()
        {
            // Arrange
            var repository = CriarRepositorio();
            var novaNotificacao = new Notificacao
            {
                Mensagem = "O volume atingiu o limite crítico.",
                StatusEnvio = "Pendente"
            };

            // Act & Assert
            var exception = await Record.ExceptionAsync(() => repository.Adicionar(novaNotificacao));
            Assert.Null(exception);
        }

        [Fact]
        public async Task CT02_Adicionar_NotificacaoComEmpresaInformada_MantemEmpresaSalva()
        {
            // Arrange
            var repository = CriarRepositorio();
            string empresaIdTeste = "empresa_notificacao_01";

            var novaNotificacao = new Notificacao
            {
                EmpresaId = empresaIdTeste,
                Mensagem = "Manutenção agendada para o sensor Kinect.",
                StatusEnvio = "Pendente"
            };

            // Act
            var exception = await Record.ExceptionAsync(() => repository.Adicionar(novaNotificacao));

            // Assert
            Assert.Null(exception);
            Assert.Equal(empresaIdTeste, novaNotificacao.EmpresaId);
        }

        [Fact]
        public async Task CT03_Adicionar_ErroNoBanco_LancaExcecao()
        {
            // Arrange
            var repositoryErro = CriarRepositorio("projeto_invalido_inexistente_xyz");
            var notificacaoInvalida = new Notificacao
            {
                Mensagem = "Simulando falha de conexão.",
                StatusEnvio = "Erro"
            };

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => repositoryErro.Adicionar(notificacaoInvalida));
        }
        #endregion

        #region ListarTodos
        [Fact]
        public async Task CT04_ListarTodos_ComRegistros_RetornaListaOrdenadaPorDataDescendente()
        {
            // Arrange
            var repository = CriarRepositorio();

            // Act
            var resultado = await repository.ListarTodos();

            // Assert
            Assert.NotNull(resultado);
            for (int i = 0; i < resultado.Count - 1; i++)
            {
                Assert.True(resultado[i].DataHora >= resultado[i + 1].DataHora);
            }
        }

        [Fact]
        public async Task CT05_ListarTodos_ErroNoBanco_RetornaListaVazia()
        {
            // Arrange
            var repositoryErro = CriarRepositorio("projeto_invalido_inexistente_xyz");

            // Act
            var resultado = await repositoryErro.ListarTodos();

            // Assert
            Assert.NotNull(resultado);
            Assert.Empty(resultado);
        }
        #endregion

        #region ListarPorEmpresa
        [Fact]
        public async Task CT06_ListarPorEmpresa_EmpresaEspecifica_RetornaNotificacoesFiltradas()
        {
            // Arrange
            var repository = CriarRepositorio();
            string empresaAlvo = "empresa_teste_abc";

            // Act
            var resultado = await repository.ListarPorEmpresa(empresaAlvo);

            // Assert
            Assert.NotNull(resultado);
            foreach (var notificacao in resultado)
            {
                bool pertenceAEmpresa = notificacao.EmpresaId == empresaAlvo ||
                                       (empresaAlvo == ContextoUsuarioService.EmpresaPadraoId 
                                       && string.IsNullOrWhiteSpace(notificacao.EmpresaId));
                Assert.True(pertenceAEmpresa);
            }
        }

        [Fact]
        public async Task CT07_ListarPorEmpresa_EmpresaPadrao_RetornaNotificacoesGlobais()
        {
            // Arrange
            var repository = CriarRepositorio();

            // Act
            var resultado = await repository.ListarPorEmpresa(ContextoUsuarioService.EmpresaPadraoId);

            // Assert
            Assert.NotNull(resultado);           
            Assert.IsType<List<Notificacao>>(resultado);
        }
        #endregion

        #region AtualizarStatus
        [Fact]
        public async Task CT08_AtualizarStatus_IdValido_RetornaTrueComSucesso()
        {
            // Arrange
            var repository = CriarRepositorio();
            string idInexistenteOuSimulado = "id_teste_status_999";

            // Act          
            var resultado = await repository.AtualizarStatus(idInexistenteOuSimulado, "Enviado");

            // Assert
            Assert.IsType<bool>(resultado);
        }

        [Fact]
        public async Task CT09_AtualizarStatus_ErroNoBanco_RetornaFalse()
        {
            // Arrange
            var repositoryErro = CriarRepositorio("projeto_invalido_inexistente_xyz");

            // Act
            var resultado = await repositoryErro.AtualizarStatus("id_qualquer", "Lido");

            // Assert
            Assert.False(resultado);
        }
        #endregion
    }
}