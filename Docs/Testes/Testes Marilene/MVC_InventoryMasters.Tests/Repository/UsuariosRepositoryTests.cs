using System;
using System.Collections.Generic;
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
    public class UsuariosRepositoryTests
    {
        #region ListarTodos
        [Fact]
        public async Task ListarTodos_ColecaoVazia_RetornaListaVazia()
        {
            // Arrange
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["Firebase:ProjectId"]).Returns("inventorymasters");
            mockConfiguration.Setup(c => c["Firebase:CredentialPath"]).Returns("Config/inventorymasters_firebase.json");

            var firebaseService = new FirebaseService(mockConfiguration.Object);
            var mockLogger = new Mock<ILogger<UsuariosRepository>>();
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var contextoUsuarioService = new ContextoUsuarioService(mockHttpContextAccessor.Object);

            var repository = new UsuariosRepository(
                firebaseService,
                mockLogger.Object,
                contextoUsuarioService);

            // Act
            var resultado = await repository.ListarTodos();

            // Assert
            Assert.NotNull(resultado);
            Assert.IsType<List<Usuario>>(resultado);
            Assert.Empty(resultado);
        }

        [Fact]

        public async Task ListarTodos_ComUsuariosCadastrados_RetornaListaPreenchidaComIds()
        {
            // Arrange
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["Firebase:ProjectId"]).Returns("inventorymasters");
            mockConfiguration.Setup(c => c["Firebase:CredentialPath"]).Returns("Config/inventorymasters_firebase.json");

            var firebaseService = new FirebaseService(mockConfiguration.Object);
            var mockLogger = new Mock<ILogger<UsuariosRepository>>();
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var contextoUsuarioService = new ContextoUsuarioService(mockHttpContextAccessor.Object);

            var repository = new UsuariosRepository(
                firebaseService,
                mockLogger.Object,
                contextoUsuarioService);

            // Act
            var resultado = await repository.ListarTodos();

            // Assert
            Assert.NotNull(resultado);
            Assert.IsType<List<Usuario>>(resultado);
            Assert.NotEmpty(resultado);
            
            foreach (var usuario in resultado)
            {
                Assert.False(string.IsNullOrWhiteSpace(usuario.Id), 
                    "O ID do usuário retornado não deve ser nulo ou vazio.");
            }
        }
        #endregion

        #region ListarPorEmpresa
        [Fact]
       public async Task ListarPorEmpresa_EmpresaValida_RetornaUsuariosDaEmpresa()
        {
            // Arrange
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["Firebase:ProjectId"]).Returns("inventorymasters");
            mockConfiguration.Setup(c => c["Firebase:CredentialPath"]).Returns("Config/inventorymasters_firebase.json");

            var firebaseService = new FirebaseService(mockConfiguration.Object);
            var mockLogger = new Mock<ILogger<UsuariosRepository>>();
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var contextoUsuarioService = new ContextoUsuarioService(mockHttpContextAccessor.Object);

            var repository = new UsuariosRepository(
                firebaseService,
                mockLogger.Object,
                contextoUsuarioService);
       
            string empresaIdValida = "EmpresaPadraoId";

            // Act
            var resultado = await repository.ListarPorEmpresa(empresaIdValida);

            // Assert
            Assert.NotNull(resultado);
            Assert.IsType<List<Usuario>>(resultado);
            
            foreach (var usuario in resultado)
            {
                bool pertenceAEmpresa = usuario.EmpresaId == empresaIdValida ||
                                       (string.IsNullOrWhiteSpace(usuario.EmpresaId));

                Assert.True(pertenceAEmpresa, "O usuário retornado não pertence à empresa especificada.");
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
            var mockLogger = new Mock<ILogger<UsuariosRepository>>();
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var contextoUsuarioService = new ContextoUsuarioService(mockHttpContextAccessor.Object);

            var repository = new UsuariosRepository(
                firebaseService,
                mockLogger.Object,
                contextoUsuarioService);

            // Act 
            var resultadoVazio = await repository.ListarPorEmpresa(string.Empty);
            var resultadoNulo = await repository.ListarPorEmpresa(null);

            // Assert
            Assert.NotNull(resultadoVazio);
            Assert.NotNull(resultadoNulo);
            Assert.IsType<List<Usuario>>(resultadoVazio);
            Assert.IsType<List<Usuario>>(resultadoNulo);            
            Assert.Equal(resultadoVazio.Count, resultadoNulo.Count);
        }
        #endregion

        #region BuscarPorId
        [Fact]
        public async Task BuscarPorId_IdExistente_RetornaUsuarioCorrespondente()
        {
            // Arrange
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["Firebase:ProjectId"]).Returns("inventorymasters");
            mockConfiguration.Setup(c => c["Firebase:CredentialPath"]).Returns("Config/inventorymasters_firebase.json");

            var firebaseService = new FirebaseService(mockConfiguration.Object);
            var mockLogger = new Mock<ILogger<UsuariosRepository>>();
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var contextoUsuarioService = new ContextoUsuarioService(mockHttpContextAccessor.Object);

            var repository = new UsuariosRepository(
                firebaseService,
                mockLogger.Object,
                contextoUsuarioService);
            
            var todosUsuarios = await repository.ListarTodos();
            string idExistente = todosUsuarios.First().Id;

            // Act
            var usuarioRetornado = await repository.BuscarPorId(idExistente);

            // Assert
            Assert.NotNull(usuarioRetornado);
            Assert.IsType<Usuario>(usuarioRetornado);
            Assert.Equal(idExistente, usuarioRetornado.Id);
        }

        [Fact]
        public async Task BuscarPorId_IdInexistente_RetornaNull()
        {
            // Arrange
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["Firebase:ProjectId"]).Returns("inventorymasters");
            mockConfiguration.Setup(c => c["Firebase:CredentialPath"]).Returns("Config/inventorymasters_firebase.json");

            var firebaseService = new FirebaseService(mockConfiguration.Object);
            var mockLogger = new Mock<ILogger<UsuariosRepository>>();
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var contextoUsuarioService = new ContextoUsuarioService(mockHttpContextAccessor.Object);

            var repository = new UsuariosRepository(
                firebaseService,
                mockLogger.Object,
                contextoUsuarioService);
            
            string idInexistente = "ID_Inexistente_" + Guid.NewGuid().ToString();

            // Act
            var usuarioRetornado = await repository.BuscarPorId(idInexistente);

            // Assert
            Assert.Null(usuarioRetornado);
        }
        #endregion

        #region BuscarPorEmail

        [Fact]
        public async Task BuscarPorEmail_EmailExistente_RetornaUsuarioCorrespondente()
        {
            // Arrange
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["Firebase:ProjectId"]).Returns("inventorymasters");
            mockConfiguration.Setup(c => c["Firebase:CredentialPath"]).Returns("Config/inventorymasters_firebase.json");

            var firebaseService = new FirebaseService(mockConfiguration.Object);
            var mockLogger = new Mock<ILogger<UsuariosRepository>>();
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var contextoUsuarioService = new ContextoUsuarioService(mockHttpContextAccessor.Object);

            var repository = new UsuariosRepository(
                firebaseService,
                mockLogger.Object,
                contextoUsuarioService);
           
            var todosUsuarios = await repository.ListarTodos();
            string emailExistente = todosUsuarios.First().Email;

            // Act
            var usuarioRetornado = await repository.BuscarPorEmail(emailExistente);

            // Assert
            Assert.NotNull(usuarioRetornado);
            Assert.IsType<Usuario>(usuarioRetornado);
            Assert.Equal(emailExistente, usuarioRetornado.Email);
        }

        [Fact]
        public async Task BuscarPorEmail_EmailInexistente_RetornaNull()
        {
            // Arrange
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["Firebase:ProjectId"]).Returns("inventorymasters");
            mockConfiguration.Setup(c => c["Firebase:CredentialPath"]).Returns("Config/inventorymasters_firebase.json");

            var firebaseService = new FirebaseService(mockConfiguration.Object);
            var mockLogger = new Mock<ILogger<UsuariosRepository>>();
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var contextoUsuarioService = new ContextoUsuarioService(mockHttpContextAccessor.Object);

            var repository = new UsuariosRepository(
                firebaseService,
                mockLogger.Object,
                contextoUsuarioService);

            string emailInexistente = $"nao_existe_{Guid.NewGuid()}@teste.com";

            // Act
            var usuarioRetornado = await repository.BuscarPorEmail(emailInexistente);

            // Assert
            Assert.Null(usuarioRetornado);
        }
        #endregion

        #region Adicionar
        [Fact]
        public async Task Adicionar_UsuarioValido_AdicionaComSucesso()
        {
            // Arrange
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["Firebase:ProjectId"]).Returns("inventorymasters");
            mockConfiguration.Setup(c => c["Firebase:CredentialPath"]).Returns("Config/inventorymasters_firebase.json");

            var firebaseService = new FirebaseService(mockConfiguration.Object);
            var mockLogger = new Mock<ILogger<UsuariosRepository>>();
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var contextoUsuarioService = new ContextoUsuarioService(mockHttpContextAccessor.Object);

            var repository = new UsuariosRepository(
                firebaseService,
                mockLogger.Object,
                contextoUsuarioService);

            var novoUsuario = new Usuario
            {
                Nome = "Usuário Teste Automático",
                Email = $"teste_{Guid.NewGuid()}@inventorymasters.com",
                Perfil = "Operador",
                EmpresaId = "Empresa Inventory Masters"
            };

            // Act            
            await repository.Adicionar(novoUsuario);

            // Assert            
            var usuarioCadastrado = await repository.BuscarPorEmail(novoUsuario.Email);

            Assert.NotNull(usuarioCadastrado);
            Assert.Equal(novoUsuario.Email, usuarioCadastrado.Email);
            Assert.Equal(novoUsuario.Nome, usuarioCadastrado.Nome);
        }

        [Fact]
        public async Task Adicionar_ErroNoBanco_LancaExcecao()
        {
            // Arrange
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["Firebase:ProjectId"]).Returns("inventorymasters");
            mockConfiguration.Setup(c => c["Firebase:CredentialPath"]).Returns("Config/inventorymasters_firebase.json");

            var firebaseService = new FirebaseService(mockConfiguration.Object);
            var mockLogger = new Mock<ILogger<UsuariosRepository>>();
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var contextoUsuarioService = new ContextoUsuarioService(mockHttpContextAccessor.Object);

            var repository = new UsuariosRepository(
                firebaseService,
                mockLogger.Object,
                contextoUsuarioService);

            // Act & Assert      
            await Assert.ThrowsAsync<Exception>(async () =>
            {
                await repository.Adicionar(null);
            });
        }
        #endregion

        #region Atualizar
        [Fact]
        public async Task Atualizar_UsuarioExistenteComDadosValidos_AtualizaComSucesso()
        {
            // Arrange
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["Firebase:ProjectId"]).Returns("inventorymasters");
            mockConfiguration.Setup(c => c["Firebase:CredentialPath"]).Returns("Config/inventorymasters_firebase.json");

            var firebaseService = new FirebaseService(mockConfiguration.Object);
            var mockLogger = new Mock<ILogger<UsuariosRepository>>();
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var contextoUsuarioService = new ContextoUsuarioService(mockHttpContextAccessor.Object);

            var repository = new UsuariosRepository(
                firebaseService,
                mockLogger.Object,
                contextoUsuarioService);

            // Busca um usuário existente na base para garantir que temos um ID válido para atualizar
            var todosUsuarios = await repository.ListarTodos();
            var usuarioParaAtualizar = todosUsuarios.First();

            // Modifica o nome para testar a atualização
            string novoNome = "Nome Atualizado " + Guid.NewGuid().ToString().Substring(0, 6);
            usuarioParaAtualizar.Nome = novoNome;

            // Act
            await repository.Atualizar(usuarioParaAtualizar);

            // Assert
            var usuarioAtualizado = await repository.BuscarPorId(usuarioParaAtualizar.Id);

            Assert.NotNull(usuarioAtualizado);
            Assert.Equal(novoNome, usuarioAtualizado.Nome);
        }

        [Fact]
        public async Task Atualizar_ErroDuranteAtualizacao_LancaExcecao()
        {
            // Arrange
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["Firebase:ProjectId"]).Returns("inventorymasters");
            mockConfiguration.Setup(c => c["Firebase:CredentialPath"]).Returns("Config/inventorymasters_firebase.json");

            var firebaseService = new FirebaseService(mockConfiguration.Object);
            var mockLogger = new Mock<ILogger<UsuariosRepository>>();
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var contextoUsuarioService = new ContextoUsuarioService(mockHttpContextAccessor.Object);

            var repository = new UsuariosRepository(
                firebaseService,
                mockLogger.Object,
                contextoUsuarioService);

            // Act & Assert           
            await Assert.ThrowsAsync<NullReferenceException>(async () =>
            {
                await repository.Atualizar(null);
            });
        }

        [Fact]
        public async Task AtualizarStatus_IdValidoENovoStatus_AtualizaStatusComSucesso()
        {
            // Arrange
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["Firebase:ProjectId"]).Returns("inventorymasters");
            mockConfiguration.Setup(c => c["Firebase:CredentialPath"]).Returns("Config/inventorymasters_firebase.json");

            var firebaseService = new FirebaseService(mockConfiguration.Object);
            var mockLogger = new Mock<ILogger<UsuariosRepository>>();
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var contextoUsuarioService = new ContextoUsuarioService(mockHttpContextAccessor.Object);

            var repository = new UsuariosRepository(
                firebaseService,
                mockLogger.Object,
                contextoUsuarioService);

            var todosUsuarios = await repository.ListarTodos();
            var usuario = todosUsuarios.First();

            string novoPerfilTeste = usuario.Perfil == "Administrador" ? "Operador" : "Administrador";
            usuario.Perfil = novoPerfilTeste;

            // Act
            await repository.Atualizar(usuario);

            // Assert
            var usuarioAtualizado = await repository.BuscarPorId(usuario.Id);
            Assert.NotNull(usuarioAtualizado);
            Assert.Equal(novoPerfilTeste, usuarioAtualizado.Perfil);
        }

        #endregion

        #region Excluir
        [Fact]
        public async Task Excluir_IdDeUsuarioExistente_RemoveComSucesso()
        {
            // Arrange
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["Firebase:ProjectId"]).Returns("inventorymasters");
            mockConfiguration.Setup(c => c["Firebase:CredentialPath"]).Returns("Config/inventorymasters_firebase.json");

            var firebaseService = new FirebaseService(mockConfiguration.Object);
            var mockLogger = new Mock<ILogger<UsuariosRepository>>();
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var contextoUsuarioService = new ContextoUsuarioService(mockHttpContextAccessor.Object);

            var repository = new UsuariosRepository(
                firebaseService,
                mockLogger.Object,
                contextoUsuarioService);
           
            var usuarioTemp = new Usuario
            {
                Nome = "Usuário Para Exclusão",
                Email = $"excluir_{Guid.NewGuid()}@inventorymasters.com",
                Perfil = "Visualizador",
                EmpresaId = "EmpresaInventoryMasters"
            };

            await repository.Adicionar(usuarioTemp);
            var usuarioCriado = await repository.BuscarPorEmail(usuarioTemp.Email);
            string idParaExcluir = usuarioCriado.Id;

            // Act
            await repository.Excluir(idParaExcluir);

            // Assert
            var usuarioAposExclusao = await repository.BuscarPorId(idParaExcluir);
            Assert.Null(usuarioAposExclusao);
        }

        [Fact]
        public async Task Excluir_IdInexistente_NaoLancaExcecao()
        {
            // Arrange
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["Firebase:ProjectId"]).Returns("inventorymasters");
            mockConfiguration.Setup(c => c["Firebase:CredentialPath"]).Returns("Config/inventorymasters_firebase.json");

            var firebaseService = new FirebaseService(mockConfiguration.Object);
            var mockLogger = new Mock<ILogger<UsuariosRepository>>();
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var contextoUsuarioService = new ContextoUsuarioService(mockHttpContextAccessor.Object);

            var repository = new UsuariosRepository(
                firebaseService,
                mockLogger.Object,
                contextoUsuarioService);

            string idInexistente = "ID_Inexistente_Invalido_" + Guid.NewGuid().ToString();

            // Act & Assert
            var exception = await Record.ExceptionAsync(async () =>
            {
                await repository.Excluir(idInexistente);
            });

            Assert.Null(exception);
        }
        #endregion    
    }
}

