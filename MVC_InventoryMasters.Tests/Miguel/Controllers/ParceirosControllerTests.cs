using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using MVC_InventoryMasters.Controllers;
using MVC_InventoryMasters.Models;
using MVC_InventoryMasters.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace MVC_InventoryMasters.Tests
{
    [Trait("Integrante", "Miguel")]
    public class ParceirosControllerTests
    {
        private readonly Mock<IParceirosRepository> _mockRepo;
        private readonly Mock<ILogger<ParceirosController>> _mockLogger;
        private readonly ParceirosController _controller;

        public ParceirosControllerTests()
        {
            _mockRepo = new Mock<IParceirosRepository>();
            _mockLogger = new Mock<ILogger<ParceirosController>>();

            _controller = new ParceirosController(_mockRepo.Object, _mockLogger.Object);

            var httpContext = new DefaultHttpContext();
            var tempDataProvider = new Mock<ITempDataProvider>();
            _controller.TempData = new TempDataDictionary(httpContext, tempDataProvider.Object);
        }

        #region Index Tests

        [Fact]
        public async Task Index_DeveRetornarViewComListaPaginadaEViewBagPreenchida()
        {
            // Arrange
            var parceiros = new List<Parceiro>
            {
                new() { Id = "1", Nome = "Parceiro A" },
                new() { Id = "2", Nome = "Parceiro B" }
            };

            _mockRepo.Setup(r => r.FiltrarAvancado(It.IsAny<string?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<bool?>()))
                     .ReturnsAsync(parceiros);

            // Act
            var result = await _controller.Index(1, "busca", null, null, true);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<Parceiro>>(viewResult.Model);

            Assert.Equal(2, model.Count);
            Assert.Equal("busca", _controller.ViewBag.Termo);
            Assert.Equal(2, _controller.ViewBag.TotalRegistros);
            Assert.Equal(1, _controller.ViewBag.TotalPaginas);
        }

        [Fact]
        public async Task Index_QuandoOcorrerExcecao_DeveRedirecionarParaError()
        {
            // Arrange
            _mockRepo.Setup(r => r.FiltrarAvancado(It.IsAny<string?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<bool?>()))
                     .ThrowsAsync(new Exception("Erro de banco"));

            // Act
            var result = await _controller.Index();

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", redirectResult.ActionName);
            Assert.Equal("Home", redirectResult.ControllerName);
        }

        #endregion

        #region Details Tests

        [Fact]
        public async Task Details_ComIdVazio_DeveRetornarBadRequest()
        {
            // Act
            var result = await _controller.Details(string.Empty);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task Details_QuandoNaoEncontrado_DeveRetornarNotFound()
        {
            // Arrange
            _mockRepo.Setup(r => r.BuscarPorId("123")).ReturnsAsync((Parceiro?)null);

            // Act
            var result = await _controller.Details("123");

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Details_ComIdValido_DeveRetornarViewComParceiro()
        {
            // Arrange
            var parceiro = new Parceiro { Id = "123", Nome = "Parceiro X" };
            _mockRepo.Setup(r => r.BuscarPorId("123")).ReturnsAsync(parceiro);

            // Act
            var result = await _controller.Details("123");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Parceiro>(viewResult.Model);
            Assert.Equal("123", model.Id);
        }

        [Fact]
        public async Task Details_QuandoOcorrerExcecao_DeveRedirecionarParaError()
        {
            // Arrange
            _mockRepo.Setup(r => r.BuscarPorId("123")).ThrowsAsync(new Exception("Erro"));

            // Act
            var result = await _controller.Details("123");

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", redirectResult.ActionName);
        }

        #endregion

        #region Create Tests

        [Fact]
        public void Create_Get_DeveRetornarViewComNovoParceiroAtivo()
        {
            // Act
            var result = _controller.Create();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Parceiro>(viewResult.Model);
            Assert.True(model.Ativo);
        }

        [Fact]
        public async Task Create_Post_ComModelStateInvalido_DeveRetornarViewComModelo()
        {
            // Arrange
            var parceiro = new Parceiro();
            _controller.ModelState.AddModelError("Nome", "Nome é obrigatório");

            // Act
            var result = await _controller.Create(parceiro);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(parceiro, viewResult.Model);
            _mockRepo.Verify(r => r.Adicionar(It.IsAny<Parceiro>()), Times.Never);
        }

        [Fact]
        public async Task Create_Post_ComSucesso_DeveAdicionarERedirecionar()
        {
            // Arrange
            var parceiro = new Parceiro { Nome = "Novo Parceiro" };

            // Act
            var result = await _controller.Create(parceiro);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(ParceirosController.Index), redirectResult.ActionName);
            Assert.Equal("Parceiro cadastrado com sucesso.", _controller.TempData["Sucesso"]);
            _mockRepo.Verify(r => r.Adicionar(parceiro), Times.Once);
        }

        [Fact]
        public async Task Create_Post_QuandoOcorrerExcecao_DeveRetornarViewETempDataErro()
        {
            // Arrange
            var parceiro = new Parceiro { Nome = "Novo Parceiro" };
            _mockRepo.Setup(r => r.Adicionar(parceiro)).ThrowsAsync(new Exception("Erro ao salvar"));

            // Act
            var result = await _controller.Create(parceiro);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Ocorreu um erro ao salvar o parceiro.", _controller.TempData["Erro"]);
        }

        #endregion

        #region Edit Tests

        [Fact]
        public async Task Edit_Post_SemAlteracoes_DeveDefinirViewBagAvisoERetornarView()
        {
            // Arrange
            var existente = new Parceiro
            {
                Id = "1",
                Nome = "Parceiro A",
                Email = "email@teste.com",
                Telefone = "(11) 99999-9999",
                Empresa = "Empresa X",
                Endereco = "Rua A",
                Ativo = true
            };

            var modeloFormulario = new Parceiro
            {
                Id = "1",
                Nome = "Parceiro A",
                Email = "email@teste.com",
                Telefone = "11999999999", // Mesmo telefone sem máscara
                Empresa = "Empresa X",
                Endereco = "Rua A",
                Ativo = true
            };

            _mockRepo.Setup(r => r.BuscarPorId("1")).ReturnsAsync(existente);

            // Act
            var result = await _controller.Edit(modeloFormulario);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Nenhuma alteração foi realizada.", _controller.ViewBag.Aviso);
            _mockRepo.Verify(r => r.Atualizar(It.IsAny<Parceiro>()), Times.Never);
        }

        [Fact]
        public async Task Edit_Post_ComAlteracoes_DeveAtualizarERedirecionar()
        {
            // Arrange
            var existente = new Parceiro { Id = "1", Nome = "Nome Antigo", Data_Cadastro = DateTime.UtcNow.AddDays(-10) };
            var modeloFormulario = new Parceiro { Id = "1", Nome = "Nome Novo" };

            _mockRepo.Setup(r => r.BuscarPorId("1")).ReturnsAsync(existente);

            // Act
            var result = await _controller.Edit(modeloFormulario);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(ParceirosController.Index), redirectResult.ActionName);
            Assert.Equal("Parceiro atualizado com sucesso.", _controller.TempData["Sucesso"]);
            Assert.Equal(existente.Data_Cadastro, modeloFormulario.Data_Cadastro);
            _mockRepo.Verify(r => r.Atualizar(modeloFormulario), Times.Once);
        }

        #endregion

        #region Delete & Status Tests

        [Fact]
        public async Task DeleteConfirmed_ComSucesso_DeveExcluirERedirecionar()
        {
            // Act
            var result = await _controller.DeleteConfirmed("123");

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(ParceirosController.Index), redirectResult.ActionName);
            Assert.Equal("Parceiro excluído com sucesso.", _controller.TempData["Sucesso"]);
            _mockRepo.Verify(r => r.Excluir("123"), Times.Once);
        }

        [Fact]
        public async Task AlternarStatus_AtivoTrue_DeveInativarEInformarSucesso()
        {
            // Act (passando ativo = true para inverter para false)
            var result = await _controller.AlternarStatus("123", true);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(ParceirosController.Index), redirectResult.ActionName);
            Assert.Equal("Parceiro inativado com sucesso.", _controller.TempData["Sucesso"]);
            _mockRepo.Verify(r => r.AtualizarStatus("123", false), Times.Once);
        }

        [Fact]
        public async Task AlternarStatus_AtivoFalse_DeveAtivarEInformarSucesso()
        {
            // Act (passando ativo = false para inverter para true)
            var result = await _controller.AlternarStatus("123", false);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(ParceirosController.Index), redirectResult.ActionName);
            Assert.Equal("Parceiro ativado com sucesso.", _controller.TempData["Sucesso"]);
            _mockRepo.Verify(r => r.AtualizarStatus("123", true), Times.Once);
        }

        #endregion
    }
}
