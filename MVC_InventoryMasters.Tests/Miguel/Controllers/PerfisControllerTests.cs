using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using MVC_InventoryMasters.Controllers;
using MVC_InventoryMasters.Models;
using MVC_InventoryMasters.Repositories;
using Xunit;

namespace MVC_InventoryMasters.Tests
{
    [Trait("Integrante", "Miguel")]
    public class PerfisControllerTests
    {
        private readonly Mock<IPerfisRepository> _repositoryMock; // Mock da Interface
        private readonly Mock<ILogger<PerfisController>> _loggerMock;
        private readonly PerfisController _controller;

        public PerfisControllerTests()
        {
            _loggerMock = new Mock<ILogger<PerfisController>>();
            _repositoryMock = new Mock<IPerfisRepository>(); // Instancia sem argumentos de construtor

            _controller = new PerfisController(_repositoryMock.Object, _loggerMock.Object)
            {
                TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>())
            };
        }

        [Fact]
        public async Task Index_RetornaViewResult_ComListaPaginadaDePerfis()
        {
            // Arrange
            var perfis = new List<Perfil>
            {
                new Perfil { Id = "1", Nome = "Administrador", Ativo = true },
                new Perfil { Id = "2", Nome = "Operador", Ativo = true }
            };

            _repositoryMock.Setup(r => r.ListarPorEmpresa(null))
                           .ReturnsAsync(perfis);

            // Act
            var result = await _controller.Index(1, null, null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<Perfil>>(viewResult.Model);
            Assert.Equal(2, model.Count);
        }

        [Fact]
        public async Task Create_Post_ComDadosValidos_RedirecionaParaIndex()
        {
            // Arrange
            var novoPerfil = new Perfil { Nome = "Gerente", Ativo = true };
            string[] permissoes = new[] { "Perfis.Gerenciar" };

            _repositoryMock.Setup(r => r.Adicionar(It.IsAny<Perfil>()))
                           .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Create(novoPerfil, permissoes);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            _repositoryMock.Verify(r => r.Adicionar(It.IsAny<Perfil>()), Times.Once);
        }

        [Fact]
        public async Task Edit_Get_ComIdNuloOuVazio_RetornaBadRequest()
        {
            // Act
            var result = await _controller.Edit(null);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task Details_ComIdExistente_RetornaViewComModel()
        {
            // Arrange
            var perfil = new Perfil { Id = "p123", Nome = "Supervisor" };
            _repositoryMock.Setup(r => r.BuscarPorId("p123"))
                           .ReturnsAsync(perfil);

            // Act
            var result = await _controller.Details("p123");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Perfil>(viewResult.Model);
            Assert.Equal("p123", model.Id);
        }

        [Fact]
        public async Task Inativar_ComIdValido_ExecutaInativacaoERedireciona()
        {
            // Arrange
            _repositoryMock.Setup(r => r.Inativar("p123"))
                           .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Inativar("p123");

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            _repositoryMock.Verify(r => r.Inativar("p123"), Times.Once);
        }
    }
}
