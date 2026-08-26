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
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MVC_InventoryMasters.Tests
{
    public class NotificacoesControllerTests
    {
        private readonly Mock<INotificacaoRepository> _mockRepo;
        private readonly Mock<IParceirosRepository> _mockParceirosRepo;
        private readonly Mock<IHubContext<NotificacaoHub>> _mockHub;
        private readonly Mock<IHubClients> _mockClients;
        private readonly Mock<IClientProxy> _mockClientProxy;
        private readonly Mock<ILogger<NotificacoesController>> _mockLogger;
        private readonly NotificacoesController _controller;

        public NotificacoesControllerTests()
        {
            _mockRepo = new Mock<INotificacaoRepository>();
            _mockParceirosRepo = new Mock<IParceirosRepository>();
            _mockHub = new Mock<IHubContext<NotificacaoHub>>();
            _mockClients = new Mock<IHubClients>();
            _mockClientProxy = new Mock<IClientProxy>();
            _mockLogger = new Mock<ILogger<NotificacoesController>>();

            // Configuração do Mock do SignalR
            _mockClients.Setup(c => c.All).Returns(_mockClientProxy.Object);
            _mockHub.Setup(h => h.Clients).Returns(_mockClients.Object);

            _controller = new NotificacoesController(
                _mockRepo.Object,
                _mockParceirosRepo.Object,
                _mockHub.Object,
                _mockLogger.Object);
        }

        [Fact]
        public async Task Index_DeveRetornarViewComListaPaginadaEViewBagPreenchida()
        {
            // Arrange
            var notificacoes = new List<Notificacao>
            {
                new() { Id = "1", DataHora = DateTime.UtcNow, StatusEnvio = "Aceito", Tipo = "Coleta", ParceiroId = "P1" },
                new() { Id = "2", DataHora = DateTime.UtcNow.AddMinutes(-10), StatusEnvio = "Erro", Tipo = "Alerta", ParceiroId = "P2" },
                new() { Id = "3", DataHora = DateTime.UtcNow.AddMinutes(-20), StatusEnvio = "Pendente", Tipo = "Coleta", ParceiroId = "P1" }
            };

            var parceiros = new List<Parceiro>
            {
                new() { Id = "P1", Nome = "Parceiro A" },
                new() { Id = "P2", Nome = "Parceiro B" }
            };

            _mockRepo.Setup(r => r.ListarPorEmpresa(It.IsAny<string?>())).ReturnsAsync(notificacoes);
            _mockParceirosRepo.Setup(r => r.ListarPorEmpresa(It.IsAny<string?>())).ReturnsAsync(parceiros);

            // Act
            var result = await _controller.Index(1, null, null, null, null, null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<Notificacao>>(viewResult.Model);

            Assert.Equal(3, model.Count);
            Assert.Equal(3, _controller.ViewBag.TotalRegistros);
            Assert.Equal(1, _controller.ViewBag.TotalSucesso);
            Assert.Equal(1, _controller.ViewBag.TotalErro);
            Assert.Equal(1, _controller.ViewBag.TotalPendente);
        }

        [Fact]
        public async Task Index_QuandoOcorrerExcecao_DeveRedirecionarParaError()
        {
            // Arrange
            _mockRepo.Setup(r => r.ListarPorEmpresa(It.IsAny<string?>())).ThrowsAsync(new Exception("Erro de banco"));

            // Act
            var result = await _controller.Index();

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", redirectResult.ActionName);
            Assert.Equal("Home", redirectResult.ControllerName);
        }

        [Fact]
        public async Task AceitarColeta_ComIdNuloOuVazio_DeveRetornarBadRequest()
        {
            // Act
            var result = await _controller.AceitarColeta(string.Empty);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("ID da notificação não fornecido.", badRequestResult.Value);
        }

        [Fact]
        public async Task AceitarColeta_ComSucesso_DeveAtualizarStatusENotificarSignalR()
        {
            // Arrange
            string notificacaoId = "notif-123";
            _mockRepo.Setup(r => r.AtualizarStatus(notificacaoId, "Aceito")).ReturnsAsync(true);

            // Act
            var result = await _controller.AceitarColeta(notificacaoId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            _mockRepo.Verify(r => r.AtualizarStatus(notificacaoId, "Aceito"), Times.Once);

            // Verifica se a notificação foi emitida via SignalR
            _mockClientProxy.Verify(
                p => p.SendCoreAsync("ReceberNotificacao", It.Is<object[]>(o => o.Length > 0 && o[0].ToString() == "Uma nova coleta foi aceita!"), default),
                Times.Once);
        }

        [Fact]
        public async Task AceitarColeta_QuandoFalharAtualizacao_DeveRetornar500()
        {
            // Arrange
            string notificacaoId = "notif-123";
            _mockRepo.Setup(r => r.AtualizarStatus(notificacaoId, "Aceito")).ReturnsAsync(false);

            // Act
            var result = await _controller.AceitarColeta(notificacaoId);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
            Assert.Equal("Erro ao atualizar o banco de dados.", statusResult.Value);
        }

        [Fact]
        public async Task AceitarColeta_QuandoOcorrerExcecao_DeveRetornar500()
        {
            // Arrange
            string notificacaoId = "notif-123";
            _mockRepo.Setup(r => r.AtualizarStatus(notificacaoId, "Aceito")).ThrowsAsync(new Exception("Falha inesperada"));

            // Act
            var result = await _controller.AceitarColeta(notificacaoId);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
            Assert.Equal("Erro interno ao processar a solicitação.", statusResult.Value);
        }
    }
}