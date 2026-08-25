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
    public class MedicaoVolumeRepositoryTests
    {
        private MedicaoVolumeRepository CriarRepositorio(string projetoId = "inventorymasters")
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    {"Firebase:CredentialPath", "Config/inventorymasters_firebase.json"},
                    {"Firebase:ProjectId", projetoId}
                })
                .Build();

            var firebaseService = new FirebaseService(configuration);

            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            var contextoUsuario = new ContextoUsuarioService(httpContextAccessorMock.Object);

            return new MedicaoVolumeRepository(firebaseService, contextoUsuario);
        }

        #region Adicionar
        [Fact]
        public async Task Adicionar_MedicaoValida_AdicionaComSucesso()
        {
            // Arrange
            var repository = CriarRepositorio();
            var novaMedicao = new MedicaoVolume
            {
                VolumeMedido = 150.5,
                OrigemLeitura = "Sensor Kinect",
                Status = "Normal"
            };

            // Act & Assert
            var exception = await Record.ExceptionAsync(() => repository.Adicionar(novaMedicao));
            Assert.Null(exception);
            Assert.True(novaMedicao.DataHora != default);
        }

        [Fact]
        public async Task Adicionar_MedicaoComEmpresaInformada_MantemEmpresaSalva()
        {
            // Arrange
            var repository = CriarRepositorio();
            string empresaIdTeste = "empresa_medicao_01";

            var novaMedicao = new MedicaoVolume
            {
                EmpresaId = empresaIdTeste,
                VolumeMedido = 200.0,
                OrigemLeitura = "Sensor Kinect",
                Status = "Alerta"
            };

            // Act
            var exception = await Record.ExceptionAsync(() => repository.Adicionar(novaMedicao));

            // Assert
            Assert.Null(exception);
            Assert.Equal(empresaIdTeste, novaMedicao.EmpresaId);
        }

        [Fact]
        public async Task CT03_Adicionar_ErroNoBanco_LancaExcecao()
        {
            // Arrange
            var repositoryErro = CriarRepositorio("projeto_invalido_inexistente_xyz");
            var medicaoInvalida = new MedicaoVolume
            {
                VolumeMedido = 50.0,
                OrigemLeitura = "Sensor Kinect",
                Status = "Erro"
            };

            // Act & Assert
            await Assert.ThrowsAsync<Grpc.Core.RpcException>(() => repositoryErro.Adicionar(medicaoInvalida));
        }
        #endregion

        #region ListarTodos
        [Fact]
        public async Task ListarTodos_ComRegistros_RetornaListaPreenchida()
        {
            // Arrange
            var repository = CriarRepositorio();

            // Act
            var resultado = await repository.ListarTodos();

            // Assert
            Assert.NotNull(resultado);           
            foreach (var medicao in resultado)
            {
                Assert.NotNull(medicao);
            }
        }

        [Fact]
        public async Task ListarTodos_ErroNoBanco_RetornaListaVazia()
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
        public async Task ListarPorEmpresa_EmpresaEspecifica_RetornaMedicoesFiltradas()
        {
            // Arrange
            var repository = CriarRepositorio();
            string empresaAlvo = "empresa_teste_medicao";

            // Act
            var resultado = await repository.ListarPorEmpresa(empresaAlvo);

            // Assert
            Assert.NotNull(resultado);
            foreach (var medicao in resultado)
            {
                bool pertenceAEmpresa = medicao.EmpresaId == empresaAlvo ||
                                       (empresaAlvo == ContextoUsuarioService.EmpresaPadraoId 
                                       && string.IsNullOrWhiteSpace(medicao.EmpresaId));
                Assert.True(pertenceAEmpresa);
            }
        }

        [Fact]
        public async Task ListarPorEmpresa_EmpresaPadrao_RetornaMedicoesGlobais()
        {
            // Arrange
            var repository = CriarRepositorio();

            // Act
            var resultado = await repository.ListarPorEmpresa(ContextoUsuarioService.EmpresaPadraoId);

            // Assert
            Assert.NotNull(resultado);
            Assert.IsType<List<MedicaoVolume>>(resultado);
        }
        #endregion

        #region FiltrarAvancado
        [Fact]
        public async Task FiltrarAvancado_PorOrigemEStatus_RetornaListaFiltrada()
        {
            // Arrange
            var repository = CriarRepositorio();
            string origemBusca = "Kinect";
            string statusBusca = "Normal";

            // Act
            var resultado = await repository.FiltrarAvancado(origemBusca, statusBusca, null, null);

            // Assert
            Assert.NotNull(resultado);
            foreach (var medicao in resultado)
            {
                if (!string.IsNullOrEmpty(medicao.OrigemLeitura))
                {
                    Assert.Contains(origemBusca, medicao.OrigemLeitura, StringComparison.OrdinalIgnoreCase);
                }
                if (!string.IsNullOrEmpty(medicao.Status))
                {
                    Assert.Contains(statusBusca, medicao.Status, StringComparison.OrdinalIgnoreCase);
                }
            }
        }

        [Fact]
        public async Task FiltrarAvancado_PorPeriodoData_RetornaListaFiltrada()
        {
            // Arrange
            var repository = CriarRepositorio();
            DateTime dataInicio = DateTime.UtcNow.AddDays(-7);
            DateTime dataFim = DateTime.UtcNow;

            // Act
            var resultado = await repository.FiltrarAvancado(string.Empty, string.Empty, dataInicio, dataFim);

            // Assert
            Assert.NotNull(resultado);
            foreach (var medicao in resultado)
            {
                if (medicao.DataHora.HasValue)
                {
                    Assert.True(medicao.DataHora.Value.Date >= dataInicio.Date);
                    Assert.True(medicao.DataHora.Value.Date <= dataFim.Date);
                }
            }
        }
        #endregion

        #region ObterSummary
        [Fact]
        public async Task ObterSummary_ComMedicoes_RetornaEstatisticasCorretas()
        {
            // Arrange
            var repository = CriarRepositorio();

            // Act
            var summary = await repository.ObterSummary();

            // Assert
            Assert.NotNull(summary);
            Assert.IsType<MedicaoSummary>(summary);
            if (summary.TotalMedicoes > 0)
            {
                Assert.True(summary.MaxVolume >= summary.MinVolume);
                Assert.True(summary.MediaVolume >= summary.MinVolume);
                Assert.True(summary.MediaVolume <= summary.MaxVolume);
            }
        }

        [Fact]
        public async Task ObterSummary_SemMedicoes_RetornaSummaryZerado()
        {
            // Arrange
            var repositoryErro = CriarRepositorio("projeto_invalido_inexistente_xyz");

            // Act
            var summary = await repositoryErro.ObterSummary();

            // Assert
            Assert.NotNull(summary);
            Assert.Equal(0, summary.TotalMedicoes);
            Assert.Equal(0, summary.MediaVolume);
            Assert.Equal(0, summary.MaxVolume);
            Assert.Equal(0, summary.MinVolume);
        }
        #endregion
    }
}