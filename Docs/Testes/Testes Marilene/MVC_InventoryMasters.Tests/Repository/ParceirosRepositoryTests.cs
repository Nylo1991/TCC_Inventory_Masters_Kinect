using System;
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
    public class ParceirosRepositoryTests
    {
        private ParceirosRepository CriarRepositorio(string projectId = "inventorymasters")
        {
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["Firebase:ProjectId"]).Returns("inventorymasters");
            mockConfiguration.Setup(c => c["Firebase:CredentialPath"]).Returns("Config/inventorymasters_firebase.json");

            var firebaseService = new FirebaseService(mockConfiguration.Object);
            var mockLogger = new Mock<ILogger<ParceirosRepository>>();
          
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var contextoUsuario = new ContextoUsuarioService(mockHttpContextAccessor.Object);

            var repository = new ParceirosRepository(
                firebaseService,
                mockLogger.Object,
                contextoUsuario);

            return repository;
        }

        #region Adicionar
        [Fact]
        public async Task Adicionar_ParceiroValido_AdicionaComSucesso()
        {
            // Arrange
            var repository = CriarRepositorio();

            var novoParceiro = new Parceiro
            {
                Nome = "Parceiro Teste Unitário",
                Email = "teste@parceiro.com",
                Telefone = "31999998888",
                Empresa = "Empresa Inventory Masters",
                Endereco = "Rua Teste, 123",
                Ativo = true
            };

            // Act
            await repository.Adicionar(novoParceiro);

            // Assert
            var lista = await repository.ListarTodos();
            var parceiroSalvo = lista.FirstOrDefault(p => p.Email == "teste@parceiro.com");

            Assert.NotNull(parceiroSalvo);
            Assert.Equal("Parceiro Teste Unitário", parceiroSalvo.Nome);
            Assert.True(parceiroSalvo.Ativo);
        }
        #endregion

        #region BuscarPorId
        [Fact]
        public async Task BuscarPorId_IdExistente_RetornaParceiroCorrespondente()
        {
            // Arrange
            var repository = CriarRepositorio();

            var novoParceiro = new Parceiro
            {
                Nome = "Parceiro Para Busca",
                Email = "busca@parceiro.com",
                Telefone = "31888887777",
                Empresa = "Empresa Inventory Masters",
                Ativo = true
            };

            await repository.Adicionar(novoParceiro);
            
            var lista = await repository.ListarTodos();
            var parceiroCriado = lista.First(p => p.Email == "busca@parceiro.com");

            // Act
            var parceiroRetornado = await repository.BuscarPorId(parceiroCriado.Id);

            // Assert
            Assert.NotNull(parceiroRetornado);
            Assert.Equal(parceiroCriado.Id, parceiroRetornado.Id);
            Assert.Equal("Parceiro Para Busca", parceiroRetornado.Nome);
            Assert.Equal("busca@parceiro.com", parceiroRetornado.Email);
        }

        [Fact]
        public async Task BuscarPorId_IdInexistente_RetornaNull()
        {
            // Arrange
            var repository = CriarRepositorio();
            string idInexistente = "id_que_nao_existe_999999";

            // Act
            var parceiroRetornado = await repository.BuscarPorId(idInexistente);

            // Assert
            Assert.Null(parceiroRetornado);
        }
        #endregion

        #region ListarTodos
        [Fact]
        public async Task ListarTodos_ParceirosCadastrados_RetornaListaCompleta()
        {
            // Arrange
            var repository = CriarRepositorio();

            var parceiro = new Parceiro
            {
                Nome = "Parceiro Listagem",
                Email = "listar@parceiro.com",
                Ativo = true
            };

            await repository.Adicionar(parceiro);

            // Act
            var lista = await repository.ListarTodos();

            // Assert
            Assert.NotNull(lista);
            Assert.True(lista.Count > 0);
        }
        #endregion

        #region ListarPorEmpres
        [Fact]
        public async Task ListarPorEmpresa_EmpresaValida_RetornaListaFiltrada()
        {
            // Arrange
            var repository = CriarRepositorio();
            string empresaAlvo = "empresa_especifica_123";

            var parceiro = new Parceiro
            {
                Nome = "Parceiro Empresa",
                Email = "empresa@parceiro.com",
                EmpresaId = empresaAlvo,
                Ativo = true
            };

            await repository.Adicionar(parceiro);

            // Act
            var listaFiltrada = await repository.ListarPorEmpresa(empresaAlvo);

            // Assert
            Assert.NotNull(listaFiltrada);
            Assert.Contains(listaFiltrada, p => p.Email == "empresa@parceiro.com" && p.EmpresaId == empresaAlvo);
        }
        #endregion

        #region Pesquisar
        [Fact]
        public async Task Pesquisar_TermoValido_RetornaParceirosCorrespondentes()
        {
            // Arrange
            var repository = CriarRepositorio();
            string termoUnico = "UniqueSearchTerm_" + Guid.NewGuid().ToString().Substring(0, 6);

            var parceiro = new Parceiro
            {
                Nome = $"Parceiro {termoUnico}",
                Email = "pesquisa@parceiro.com",
                Ativo = true
            };

            await repository.Adicionar(parceiro);

            // Act
            var resultados = await repository.Pesquisar(termoUnico);

            // Assert
            Assert.NotNull(resultados);
            Assert.Contains(resultados, p => p.Nome.Contains(termoUnico));
        }
        #endregion

        #region FiltrarAvancado
        [Fact]
        public async Task FiltrarAvancado_ComFiltrosPreenchidos_RetornaListaFiltrada()
        {
            // Arrange
            var repository = CriarRepositorio();
            string termo = "FiltroAvancado_" + Guid.NewGuid().ToString().Substring(0, 4);

            var parceiro = new Parceiro
            {
                Nome = termo,
                Email = "avancado@parceiro.com",
                Ativo = true,
                Data_Cadastro = DateTime.UtcNow
            };

            await repository.Adicionar(parceiro);

            // Act
            var resultados = await repository.FiltrarAvancado(termo, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1), true);

            // Assert
            Assert.NotNull(resultados);
            Assert.Contains(resultados, p => p.Nome == termo && p.Ativo);
        }
        #endregion

        #region Atualizar
        [Fact]
        public async Task Atualizar_ParceiroValido_AtualizaComSucesso()
        {
            // Arrange
            var repository = CriarRepositorio();

            var parceiro = new Parceiro
            {
                Nome = "Parceiro Para Atualizar",
                Email = "update@parceiro.com",
                Ativo = true
            };

            await repository.Adicionar(parceiro);

            var lista = await repository.ListarTodos();
            var parceiroSalvo = lista.First(p => p.Email == "update@parceiro.com");
            
            parceiroSalvo.Nome = "Parceiro Atualizado com Sucesso";
            parceiroSalvo.Telefone = "31977778888";

            // Act
            await repository.Atualizar(parceiroSalvo);

            // Assert
            var parceiroAtualizado = await repository.BuscarPorId(parceiroSalvo.Id);
            Assert.NotNull(parceiroAtualizado);
            Assert.Equal("Parceiro Atualizado com Sucesso", parceiroAtualizado.Nome);
            Assert.Equal("31977778888", parceiroAtualizado.Telefone);
        }
        #endregion

        #region Excluir
        [Fact]
        public async Task Excluir_IdValido_RemoveComSucesso()
        {
            // Arrange
            var repository = CriarRepositorio();

            var parceiro = new Parceiro
            {
                Nome = "Parceiro Para Excluir",
                Email = "excluir@parceiro.com",
                Ativo = true
            };

            await repository.Adicionar(parceiro);

            var lista = await repository.ListarTodos();
            var parceiroSalvo = lista.First(p => p.Email == "excluir@parceiro.com");

            // Act
            await repository.Excluir(parceiroSalvo.Id);

            // Assert
            var parceiroRemovido = await repository.BuscarPorId(parceiroSalvo.Id);
            Assert.Null(parceiroRemovido);
        }
       
        [Fact]
        public async Task Excluir_ErroNoBanco_LancaExcecao()
        {
            // Arrange
            var repository = CriarRepositorio();
            string idInvalido = null;

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => repository.Excluir(idInvalido));
            Assert.Equal("Não foi possível excluir o parceiro.", exception.Message);
        }
        #endregion

        #region AtualizarStatu
        [Fact]
        public async Task AtualizarStatus_IdEStatusValidos_AtualizaComSucesso()
        {
            // Arrange
            var repository = CriarRepositorio();

            var parceiro = new Parceiro
            {
                Nome = "Parceiro Para Atualizar Status",
                Email = "status@parceiro.com",
                Ativo = true
            };

            await repository.Adicionar(parceiro);

            var lista = await repository.ListarTodos();
            var parceiroSalvo = lista.First(p => p.Email == "status@parceiro.com");

            // Act 
            bool novoStatus = !parceiroSalvo.Ativo;
            await repository.AtualizarStatus(parceiroSalvo.Id, novoStatus);

            // Assert
            var parceiroAtualizado = await repository.BuscarPorId(parceiroSalvo.Id);
            Assert.NotNull(parceiroAtualizado);
            Assert.Equal(novoStatus, parceiroAtualizado.Ativo);
        }

        [Fact]
        public async Task AtualizarStatus_ErroNoBanco_LancaExcecao()
        {
            // Arrange
            var repositoryInvalido = CriarRepositorio("projeto_inexistente_abc_123");
            string idInvalido = null; 
            bool statusDesejado = false;

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => repositoryInvalido.AtualizarStatus(idInvalido, statusDesejado));
            Assert.Equal("Não foi possível atualizar o status do parceiro.", exception.Message);
        }
        #endregion
    }
}