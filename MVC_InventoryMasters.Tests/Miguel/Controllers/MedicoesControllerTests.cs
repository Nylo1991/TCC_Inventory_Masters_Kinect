using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Moq;
using MVC_InventoryMasters.Controllers;
using MVC_InventoryMasters.Hubs;
using MVC_InventoryMasters.Models;
using MVC_InventoryMasters.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace MVC_InventoryMasters.Tests
{
    [Trait("Integrante", "Miguel")]
    public class MedicoesControllerTests
    {
        private readonly Mock<IMedicaoVolumeRepository> _mockRepo;
        private readonly Mock<IHubContext<MedicaoHub>> _mockHub;
        private readonly Mock<ILogger<MedicoesController>> _mockLogger;
        private readonly MedicoesController _controller;

        public MedicoesControllerTests()
        {
            _mockRepo = new Mock<IMedicaoVolumeRepository>();
            _mockHub = new Mock<IHubContext<MedicaoHub>>();
            _mockLogger = new Mock<ILogger<MedicoesController>>();

            _controller = new MedicoesController(_mockRepo.Object, _mockHub.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Index_DeveRetornarViewComListaPaginadaEViewBagPreenchida()
        {
            // Arrange
            var medicoesExemplo = new List<MedicaoVolume>
            {
                new() { DataHora = DateTime.UtcNow, VolumeMedido = 100, Status = "Normal" },
                new() { DataHora = DateTime.UtcNow.AddMinutes(-5), VolumeMedido = 250, Status = "Alerta" }
            };

            _mockRepo
                .Setup(r => r.FiltrarAvancado(It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                .ReturnsAsync(medicoesExemplo);

            // Act
            var result = await _controller.Index(1, null, null, null, null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<MedicaoVolume>>(viewResult.Model);

            Assert.Equal(2, model.Count);
            Assert.Equal(2, _controller.ViewBag.TotalRegistros);
            Assert.Equal(1, _controller.ViewBag.TotalNormal);
            Assert.Equal(1, _controller.ViewBag.TotalAlerta);
            Assert.Equal(175, _controller.ViewBag.VolumeMedio);
        }

        [Fact]
        public async Task Index_QuandoOcorrerExcecao_DeveRedirecionarParaError()
        {
            // Arrange
            _mockRepo
                .Setup(r => r.FiltrarAvancado(It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                .ThrowsAsync(new Exception("Falha de conexão com o banco"));

            // Act
            var result = await _controller.Index();

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", redirectResult.ActionName);
            Assert.Equal("Home", redirectResult.ControllerName);
        }

        [Fact]
        public async Task Summary_DeveRetornarJsonComResumo()
        {
            // Arrange
            var resumoEsperado = new MedicaoSummary
            {
                TotalMedicoes = 50,
                MediaVolume = 120.5,
                MaxVolume = 300,
                MinVolume = 10
            };

            _mockRepo.Setup(r => r.ObterSummary()).ReturnsAsync(resumoEsperado);

            // Act
            var result = await _controller.Summary();

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.Equal(resumoEsperado, jsonResult.Value);
        }

        [Fact]
        public async Task Summary_QuandoOcorrerExcecao_DeveRetornarStatusCode500()
        {
            // Arrange
            _mockRepo.Setup(r => r.ObterSummary()).ThrowsAsync(new Exception("Erro interno ao calcular resumo"));

            // Act
            var result = await _controller.Summary();

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
        }
    }
}
