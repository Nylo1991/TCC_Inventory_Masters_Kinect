using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using MVC_InventoryMasters.Controllers;
using MVC_InventoryMasters.Models;
using MVC_InventoryMasters.Repositories;
using MVC_InventoryMasters.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace MVC_InventoryMasters.Tests.Controllers
{
    [Trait("Integrante", "Miguel")]
    public class DashboardControllerTests
    {
        // 1. Declaração dos Mocks corretamente (usando as Interfaces)
        private readonly Mock<IMedicaoVolumeRepository> _medicaoRepoMock;
        private readonly Mock<INotificacaoRepository> _notificacaoRepoMock;
        private readonly Mock<IParceirosRepository> _parceirosRepoMock;
        private readonly Mock<IParametrosSistemaRepository> _parametrosRepoMock;
        private readonly Mock<IUsuariosRepository> _usuariosRepoMock;
        private readonly Mock<ILogger<DashboardController>> _loggerMock;

        // 2. O Controller que será testado
        private readonly DashboardController _controller;

        // 3. O construtor de inicialização dos testes
        public DashboardControllerTests()
        {
            // Inicializando os Mocks
            _medicaoRepoMock = new Mock<IMedicaoVolumeRepository>();
            _notificacaoRepoMock = new Mock<INotificacaoRepository>();
            _parceirosRepoMock = new Mock<IParceirosRepository>();
            _parametrosRepoMock = new Mock<IParametrosSistemaRepository>();
            _usuariosRepoMock = new Mock<IUsuariosRepository>();
            _loggerMock = new Mock<ILogger<DashboardController>>();

            // Criando o Controller e injetando as instâncias falsas (.Object)
            _controller = new DashboardController(
                _medicaoRepoMock.Object,
                _notificacaoRepoMock.Object,
                _parceirosRepoMock.Object,
                _parametrosRepoMock.Object,
                _usuariosRepoMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task Index_DeveRetornarViewComDashboardViewModel_QuandoTudoOcorreBem()
        {
            // Arrange
            _parceirosRepoMock.Setup(repo => repo.ListarPorEmpresa(It.IsAny<string>()))
                  .ReturnsAsync(new List<Parceiro>());

            _usuariosRepoMock.Setup(repo => repo.ListarPorEmpresa(It.IsAny<string>()))
                             .ReturnsAsync(new List<Usuario>());

            _notificacaoRepoMock.Setup(repo => repo.ListarPorEmpresa(It.IsAny<string>()))
                                .ReturnsAsync(new List<Notificacao>());

            _medicaoRepoMock.Setup(repo => repo.ListarPorEmpresa(It.IsAny<string>()))
                            .ReturnsAsync(new List<MedicaoVolume>
                            {
                                new MedicaoVolume { DataHora = DateTime.Now, VolumeMedido = 5000 }
                            });

            _parametrosRepoMock.Setup(repo => repo.Buscar()).Returns(new ParametrosSistema
            {
                CapacidadeMaxima = 10000
            });

            // Act
            var resultado = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(resultado);
            var model = Assert.IsType<DashboardViewModel>(viewResult.Model);

            Assert.NotNull(model.Parceiros);
            Assert.NotNull(model.Usuarios);
            Assert.NotNull(model.Medicoes);
            Assert.NotNull(model.Alertas);
            Assert.NotNull(model.Parametros);

            Assert.Equal(50, model.PercentualOcupacao);
        }

        [Fact]
        public async Task Index_DeveRedirecionarParaHomeError_QuandoOcorreExcecao()
        {
            // Arrange
            _parceirosRepoMock.Setup(repo => repo.ListarPorEmpresa(It.IsAny<string>())).ThrowsAsync(new Exception("Erro simulado no banco"));

            // Act
            var resultado = await _controller.Index();

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(resultado);
            Assert.Equal("Error", redirectResult.ActionName);
            Assert.Equal("Home", redirectResult.ControllerName);
        }
    }
}
