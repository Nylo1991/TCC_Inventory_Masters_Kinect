using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using MVC_InventoryMasters.Repositories;
using MVC_InventoryMasters.Models;
using MVC_InventoryMasters.Services;

namespace MVC_InventoryMasters.Tests.Repository
{
    public class TokensAcessoKinectRepositoryTests
    {
        #region Adicionar
        [Fact]
        public async Task Adicionar_TokenValido_AdicionaComSucesso()
        {
            // Arrange
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["Firebase:ProjectId"]).Returns("inventorymasters");
            mockConfiguration.Setup(c => c["Firebase:CredentialPath"]).Returns("Config/inventorymasters_firebase.json");

            var firebaseService = new FirebaseService(mockConfiguration.Object);
            var mockLogger = new Mock<ILogger<TokensAcessoKinectRepository>>();

            var repository = new TokensAcessoKinectRepository(
                firebaseService,
                mockLogger.Object);

            // Garante que o DateTime possui o Kind explicitamente configurado como Utc para o Firestore
            var dataAtualUtc = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);

            var novoToken = new TokenAcessoKinect
            {
                TokenHash = "hash_teste_" + Guid.NewGuid().ToString().Substring(0, 8),
                Utilizado = false,
                Revogado = false,
                CriadoEm = dataAtualUtc,
                ExpiraEm = dataAtualUtc.AddMinutes(15)
            };

            // Act
            await repository.Adicionar(novoToken);

            // Assert
            var tokenSalvo = await repository.BuscarAtivoPorHash(novoToken.TokenHash);
            Assert.NotNull(tokenSalvo);
            Assert.Equal(novoToken.TokenHash, tokenSalvo.TokenHash);
            Assert.False(tokenSalvo.Utilizado);
            Assert.False(tokenSalvo.Revogado);
        }

        #endregion

        #region BuscarAtivoPorHash
        [Fact]
        public async Task BuscarAtivoPorHash_HashExistenteEValido_RetornaTokenCorrespondente()
        {
            // Arrange
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["Firebase:ProjectId"]).Returns("inventorymasters");
            mockConfiguration.Setup(c => c["Firebase:CredentialPath"]).Returns("Config/inventorymasters_firebase.json");

            var firebaseService = new FirebaseService(mockConfiguration.Object);
            var mockLogger = new Mock<ILogger<TokensAcessoKinectRepository>>();
            var repository = new TokensAcessoKinectRepository(firebaseService, mockLogger.Object);

            var dataAtualUtc = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
            string hashUnico = "hash_busca_" + Guid.NewGuid().ToString().Substring(0, 8);

            var novoToken = new TokenAcessoKinect
            {
                TokenHash = hashUnico,
                Utilizado = false,
                Revogado = false,
                CriadoEm = dataAtualUtc,
                ExpiraEm = dataAtualUtc.AddMinutes(15)
            };

            await repository.Adicionar(novoToken);

            // Act
            var tokenRetornado = await repository.BuscarAtivoPorHash(hashUnico);

            // Assert
            Assert.NotNull(tokenRetornado);
            Assert.Equal(hashUnico, tokenRetornado.TokenHash);
            Assert.False(tokenRetornado.Utilizado);
            Assert.False(tokenRetornado.Revogado);
        }

        [Fact]
        public async Task BuscarAtivoPorHash_HashInexistenteOuInvalido_RetornaNull()
        {
            // Arrange
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["Firebase:ProjectId"]).Returns("inventorymasters");
            mockConfiguration.Setup(c => c["Firebase:CredentialPath"]).Returns("Config/inventorymasters_firebase.json");

            var firebaseService = new FirebaseService(mockConfiguration.Object);
            var mockLogger = new Mock<ILogger<TokensAcessoKinectRepository>>();
            var repository = new TokensAcessoKinectRepository(firebaseService, mockLogger.Object);

            string hashInexistente = "hash_que_nao_existe_" + Guid.NewGuid();

            // Act
            var tokenRetornado = await repository.BuscarAtivoPorHash(hashInexistente);

            // Assert
            Assert.Null(tokenRetornado);
        }

        [Fact]
        public async Task BuscarAtivoPorHash_ErroNoBanco_RetornaNull()
        {
            // Arrange
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["Firebase:ProjectId"]).Returns("projeto_invalido_inexistente_123");
            mockConfiguration.Setup(c => c["Firebase:CredentialPath"]).Returns("Config/inventorymasters_firebase.json");

            var firebaseService = new FirebaseService(mockConfiguration.Object);
            var mockLogger = new Mock<ILogger<TokensAcessoKinectRepository>>();
            var repository = new TokensAcessoKinectRepository(firebaseService, mockLogger.Object);

            // Act
            var tokenRetornado = await repository.BuscarAtivoPorHash("qualquer_hash");

            // Assert
            Assert.Null(tokenRetornado);
        }
        #endregion

        [Fact]
        public async Task MarcarComoUtilizado_TokenComIdValido_AtualizaComSucesso()
        {
            // Arrange
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["Firebase:ProjectId"]).Returns("inventorymasters");
            mockConfiguration.Setup(c => c["Firebase:CredentialPath"]).Returns("Config/inventorymasters_firebase.json");

            var firebaseService = new FirebaseService(mockConfiguration.Object);
            var mockLogger = new Mock<ILogger<TokensAcessoKinectRepository>>();
            var repository = new TokensAcessoKinectRepository(firebaseService, mockLogger.Object);

            var dataAtualUtc = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
            string hashUnico = "hash_utilizar_" + Guid.NewGuid().ToString().Substring(0, 8);

            var novoToken = new TokenAcessoKinect
            {
                TokenHash = hashUnico,
                Utilizado = false,
                Revogado = false,
                CriadoEm = dataAtualUtc,
                ExpiraEm = dataAtualUtc.AddMinutes(15)
            };

            await repository.Adicionar(novoToken);
            var tokenSalvo = await repository.BuscarAtivoPorHash(hashUnico);

            // Act
            await repository.MarcarComoUtilizado(tokenSalvo);

            // Assert
            var tokenAtualizado = await repository.BuscarAtivoPorHash(hashUnico);
            Assert.Null(tokenAtualizado); 
        }
        #region MarcarComoUtilizado 
        [Fact]
        public async Task MarcarComoUtilizado_TokenComIdNuloOuVazio_NaoExecutaAtualizacao()
        {
            // Arrange
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["Firebase:ProjectId"]).Returns("inventorymasters");
            mockConfiguration.Setup(c => c["Firebase:CredentialPath"]).Returns("Config/inventorymasters_firebase.json");

            var firebaseService = new FirebaseService(mockConfiguration.Object);
            var mockLogger = new Mock<ILogger<TokensAcessoKinectRepository>>();
            var repository = new TokensAcessoKinectRepository(firebaseService, mockLogger.Object);

            var tokenInvalido = new TokenAcessoKinect
            {
                Id = null,
                TokenHash = "hash_invalido",
                Utilizado = false
            };

            // Act & Assert 
            var exception = await Record.ExceptionAsync(() => repository.MarcarComoUtilizado(tokenInvalido));
            Assert.Null(exception);
        }
        #endregion
    }
}