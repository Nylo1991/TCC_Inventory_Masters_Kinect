using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using MVC_InventoryMasters.Repositories;
using MVC_InventoryMasters.Models;
using MVC_InventoryMasters.Services;

namespace MVC_InventoryMasters.Tests.Repository
{
    public class PerfisRepositoryTests
    {
        #region ListarTodos
        [Fact]
        public async Task ListarTodos_ComPerfisCadastrados_RetornaListaPreenchida()
        {
            // Arrange
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["Firebase:ProjectId"]).Returns("inventorymasters");
            mockConfiguration.Setup(c => c["Firebase:CredentialPath"]).Returns("Config/inventorymasters_firebase.json");

            var firebaseService = new FirebaseService(mockConfiguration.Object);
            var mockLogger = new Mock<ILogger<PerfisRepository>>();
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var contextoUsuarioService = new ContextoUsuarioService(mockHttpContextAccessor.Object);
            var permissaoService = new PermissaoService();

            var repository = new PerfisRepository(
                firebaseService,
                mockLogger.Object,
                contextoUsuarioService,
                permissaoService);

            // Act
            var resultado = await repository.ListarTodos();

            // Assert
            Assert.NotNull(resultado);
            Assert.IsType<List<Perfil>>(resultado);
            Assert.NotEmpty(resultado);
        }

        [Fact]
        public async Task ListarTodos_ErroNoBanco_RetornaListaVazia()
        {
            // Arrange
            var mockConfiguration = new Mock<IConfiguration>();
            // Mantém o arquivo de credenciais real para não dar erro de arquivo não encontrado,
            // mas usa um ProjectId inválido para forçar a falha de comunicação com o Firestore.
            mockConfiguration.Setup(c => c["Firebase:ProjectId"]).Returns("projeto_que_nao_existe_123");
            mockConfiguration.Setup(c => c["Firebase:CredentialPath"]).Returns("Config/inventorymasters_firebase.json");

            var firebaseService = new FirebaseService(mockConfiguration.Object);
            var mockLogger = new Mock<ILogger<PerfisRepository>>();
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var contextoUsuarioService = new ContextoUsuarioService(mockHttpContextAccessor.Object);
            var permissaoService = new PermissaoService();

            var repository = new PerfisRepository(
                firebaseService,
                mockLogger.Object,
                contextoUsuarioService,
                permissaoService);

            // Act
            var resultado = await repository.ListarTodos();

            // Assert
            Assert.NotNull(resultado);
            Assert.IsType<List<Perfil>>(resultado);
            Assert.Empty(resultado);
        }
        #endregion

        #region ListarPorEmpresa
        [Fact]
        public async Task ListarPorEmpresa_EmpresaValida_RetornaPerfisDaEmpresa()
        {
            // Arrange
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["Firebase:ProjectId"]).Returns("inventorymasters");
            mockConfiguration.Setup(c => c["Firebase:CredentialPath"]).Returns("Config/inventorymasters_firebase.json");

            var firebaseService = new FirebaseService(mockConfiguration.Object);
            var mockLogger = new Mock<ILogger<PerfisRepository>>();
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var contextoUsuarioService = new ContextoUsuarioService(mockHttpContextAccessor.Object);
            var permissaoService = new PermissaoService();

            var repository = new PerfisRepository(
                firebaseService,
                mockLogger.Object,
                contextoUsuarioService,
                permissaoService);

            string empresaIdValida = "EmpresaPadraoId";

            // Act
            var resultado = await repository.ListarPorEmpresa(empresaIdValida);

            // Assert
            Assert.NotNull(resultado);
            Assert.IsType<List<Perfil>>(resultado);

            foreach (var perfil in resultado)
            {
                bool pertence = perfil.EmpresaId == empresaIdValida ||
                               (string.IsNullOrWhiteSpace(perfil.EmpresaId));
                Assert.True(pertence, "O perfil retornado não pertence à empresa especificada.");
            }
        }

        [Fact]
        public async Task ListarPorEmpresa_EmpresaNulaOuVazia_UsaEmpresaContextoOuPadrao()
        {
            // Arrange
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["Firebase:ProjectId"]).Returns("inventorymasters");
            mockConfiguration.Setup(c => c["Firebase:CredentialPath"]).Returns("Config/inventorymasters_firebase.json");

            var firebaseService = new FirebaseService(mockConfiguration.Object);
            var mockLogger = new Mock<ILogger<PerfisRepository>>();
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var contextoUsuarioService = new ContextoUsuarioService(mockHttpContextAccessor.Object);
            var permissaoService = new PermissaoService();

            var repository = new PerfisRepository(
                firebaseService,
                mockLogger.Object,
                contextoUsuarioService,
                permissaoService);

            // Act
            var resultadoVazio = await repository.ListarPorEmpresa(string.Empty);
            var resultadoNulo = await repository.ListarPorEmpresa(null);

            // Assert
            Assert.NotNull(resultadoVazio);
            Assert.NotNull(resultadoNulo);
            Assert.Equal(resultadoVazio.Count, resultadoNulo.Count);
        }
        #endregion

        #region BuscarPorId
        [Fact]
        public async Task BuscarPorId_IdExistente_RetornaPerfilCorrespondente()
        {
            // Arrange
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["Firebase:ProjectId"]).Returns("inventorymasters");
            mockConfiguration.Setup(c => c["Firebase:CredentialPath"]).Returns("Config/inventorymasters_firebase.json");

            var firebaseService = new FirebaseService(mockConfiguration.Object);
            var mockLogger = new Mock<ILogger<PerfisRepository>>();
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var contextoUsuarioService = new ContextoUsuarioService(mockHttpContextAccessor.Object);
            var permissaoService = new PermissaoService();

            var repository = new PerfisRepository(
                firebaseService,
                mockLogger.Object,
                contextoUsuarioService,
                permissaoService);

            var todos = await repository.ListarTodos();
            string idExistente = todos.First().Id;

            // Act
            var perfilRetornado = await repository.BuscarPorId(idExistente);

            // Assert
            Assert.NotNull(perfilRetornado);
            Assert.IsType<Perfil>(perfilRetornado);
            Assert.Equal(idExistente, perfilRetornado.Id);
        }

        [Fact]
        public async Task BuscarPorId_IdInexistente_RetornaNull()
        {
            // Arrange
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["Firebase:ProjectId"]).Returns("inventorymasters");
            mockConfiguration.Setup(c => c["Firebase:CredentialPath"]).Returns("Config/inventorymasters_firebase.json");

            var firebaseService = new FirebaseService(mockConfiguration.Object);
            var mockLogger = new Mock<ILogger<PerfisRepository>>();
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var contextoUsuarioService = new ContextoUsuarioService(mockHttpContextAccessor.Object);
            var permissaoService = new PermissaoService();

            var repository = new PerfisRepository(
                firebaseService,
                mockLogger.Object,
                contextoUsuarioService,
                permissaoService);

            string idInexistente = "ID_Inexistente_" + Guid.NewGuid();

            // Act
            var perfilRetornado = await repository.BuscarPorId(idInexistente);

            // Assert
            Assert.Null(perfilRetornado);
        }

        [Fact]
        public async Task BuscarPorId_ErroNoBanco_RetornaNull()
        {
            // Arrange
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["Firebase:ProjectId"]).Returns("projeto_invalido_123");
            mockConfiguration.Setup(c => c["Firebase:CredentialPath"]).Returns("Config/inventorymasters_firebase.json");

            var firebaseService = new FirebaseService(mockConfiguration.Object);
            var mockLogger = new Mock<ILogger<PerfisRepository>>();
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var contextoUsuarioService = new ContextoUsuarioService(mockHttpContextAccessor.Object);
            var permissaoService = new PermissaoService();

            var repository = new PerfisRepository(
                firebaseService,
                mockLogger.Object,
                contextoUsuarioService,
                permissaoService);

            // Act
            var perfilRetornado = await repository.BuscarPorId("qualquer_id");

            // Assert
            Assert.Null(perfilRetornado);
        }
        #endregion

        #region Adicionar
        [Fact]
        public async Task Adicionar_PerfilValido_AdicionaComSucesso()
        {
            // Arrange
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["Firebase:ProjectId"]).Returns("inventorymasters");
            mockConfiguration.Setup(c => c["Firebase:CredentialPath"]).Returns("Config/inventorymasters_firebase.json");

            var firebaseService = new FirebaseService(mockConfiguration.Object);
            var mockLogger = new Mock<ILogger<PerfisRepository>>();
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var contextoUsuarioService = new ContextoUsuarioService(mockHttpContextAccessor.Object);
            var permissaoService = new PermissaoService();

            var repository = new PerfisRepository(
                firebaseService,
                mockLogger.Object,
                contextoUsuarioService,
                permissaoService);

            var novoPerfil = new Perfil
            {
                Nome = "Perfil Teste Automático " + Guid.NewGuid().ToString().Substring(0, 5),
                EmpresaId = "EmpresaPadraoId",
                Permissoes = new List<string> { "Leitura" }
            };

            // Act
            await repository.Adicionar(novoPerfil);

            // Assert
            var lista = await repository.ListarTodos();
            var perfilAdicionado = lista.FirstOrDefault(p => p.Nome == novoPerfil.Nome);

            Assert.NotNull(perfilAdicionado);
            Assert.Equal(novoPerfil.Nome, perfilAdicionado.Nome);
        }
        #endregion

        #region Atualizar
        [Fact]
        public async Task Atualizar_PerfilExistente_AtualizaComSucesso()
        {
            // Arrange
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["Firebase:ProjectId"]).Returns("inventorymasters");
            mockConfiguration.Setup(c => c["Firebase:CredentialPath"]).Returns("Config/inventorymasters_firebase.json");

            var firebaseService = new FirebaseService(mockConfiguration.Object);
            var mockLogger = new Mock<ILogger<PerfisRepository>>();
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var contextoUsuarioService = new ContextoUsuarioService(mockHttpContextAccessor.Object);
            var permissaoService = new PermissaoService();

            var repository = new PerfisRepository(
                firebaseService,
                mockLogger.Object,
                contextoUsuarioService,
                permissaoService);

            var todos = await repository.ListarTodos();
            var perfilParaAtualizar = todos.First();

            string novoNome = "Perfil Atualizado " + Guid.NewGuid().ToString().Substring(0, 5);
            perfilParaAtualizar.Nome = novoNome;

            // Act
            await repository.Atualizar(perfilParaAtualizar);

            // Assert
            var perfilAtualizado = await repository.BuscarPorId(perfilParaAtualizar.Id);
            Assert.NotNull(perfilAtualizado);
            Assert.Equal(novoNome, perfilAtualizado.Nome);
        }
        #endregion

        #region Inativar
        [Fact]
        public async Task Inativar_IdValido_InativaComSucesso()
        {
            // Arrange
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["Firebase:ProjectId"]).Returns("inventorymasters");
            mockConfiguration.Setup(c => c["Firebase:CredentialPath"]).Returns("Config/inventorymasters_firebase.json");

            var firebaseService = new FirebaseService(mockConfiguration.Object);
            var mockLogger = new Mock<ILogger<PerfisRepository>>();
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var contextoUsuarioService = new ContextoUsuarioService(mockHttpContextAccessor.Object);
            var permissaoService = new PermissaoService();

            var repository = new PerfisRepository(
                firebaseService,
                mockLogger.Object,
                contextoUsuarioService,
                permissaoService);

            var perfilTemp = new Perfil
            {
                Nome = "Perfil Para Inativar",
                EmpresaId = "EmpresaPadraoId",
                Permissoes = new List<string> { "Leitura" }
            };

            await repository.Adicionar(perfilTemp);
            var lista = await repository.ListarTodos();
            var criado = lista.First(p => p.Nome == "Perfil Para Inativar");

            // Act
            await repository.Inativar(criado.Id);

            // Assert
            var inativado = await repository.BuscarPorId(criado.Id);
            Assert.NotNull(inativado);
            Assert.False(inativado.Ativo);
        }
        #endregion
    }
}