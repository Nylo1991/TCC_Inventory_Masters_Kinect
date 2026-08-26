using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
    public class UsuariosControllerTests
    {
        private readonly Mock<IUsuariosRepository> _usuariosRepositoryMock;
        private readonly Mock<IPerfisRepository> _perfisRepositoryMock;
        private readonly Mock<ILogger<UsuariosController>> _loggerMock;
        private readonly UsuariosController _controller;

        public UsuariosControllerTests()
        {
            _usuariosRepositoryMock = new Mock<IUsuariosRepository>();
            _perfisRepositoryMock = new Mock<IPerfisRepository>();
            _loggerMock = new Mock<ILogger<UsuariosController>>();

            _controller = new UsuariosController(
                _usuariosRepositoryMock.Object,
                _perfisRepositoryMock.Object,
                _loggerMock.Object)
            {
                TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>())
            };
        }

        [Fact]
        public async Task Index_RetornaViewResult_ComListaPaginadaDeUsuarios()
        {
            // Arrange
            var usuarios = new List<Usuario>
            {
                new Usuario { Id = "1", Nome = "João Silva", Email = "joao@email.com", Ativo = true },
                new Usuario { Id = "2", Nome = "Maria Souza", Email = "maria@email.com", Ativo = true }
            };

            _usuariosRepositoryMock.Setup(r => r.ListarPorEmpresa(It.IsAny<string?>()))
                                   .ReturnsAsync(usuarios);

            // Act
            var result = await _controller.Index(1, null, null, null, null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<Usuario>>(viewResult.Model);
            Assert.Equal(2, model.Count);
        }

        [Fact]
        public async Task Create_Get_CarregaPerfisERetornaView()
        {
            // Arrange
            var perfis = new List<Perfil> { new Perfil { Id = "p1", Nome = "Administrador" } };
            _perfisRepositoryMock.Setup(r => r.ListarPorEmpresa(It.IsAny<string?>()))
                                 .ReturnsAsync(perfis);

            // Act
            var result = await _controller.Create();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Usuario>(viewResult.Model);
            Assert.True(model.Ativo);

            var perfisViewBag = Assert.IsAssignableFrom<List<SelectListItem>>(_controller.ViewBag.Perfis);
            Assert.Single(perfisViewBag);
            Assert.Equal("Administrador", perfisViewBag[0].Value);
        }

        [Fact]
        public async Task Create_Post_ModeloValido_RedirecionaParaIndex()
        {
            // Arrange
            var novoUsuario = new Usuario { Nome = "Novo Usuario", Email = "novo@email.com" };
            _usuariosRepositoryMock.Setup(r => r.Adicionar(It.IsAny<Usuario>()))
                                   .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Create(novoUsuario);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            _usuariosRepositoryMock.Verify(r => r.Adicionar(It.IsAny<Usuario>()), Times.Once);
        }

        [Fact]
        public async Task Edit_Get_IdNulo_RetornaBadRequest()
        {
            // Act (especifique o tipo do nulo para desfazer a ambiguidade)
            var result = await _controller.Edit((string?)null);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task Edit_Post_ComAlteracao_AtualizaERedirecionaParaIndex()
        {
            // Arrange
            var usuarioOriginal = new Usuario
            {
                Id = "u1",
                Nome = "Nome Antigo",
                Email = "email@email.com",
                Senha = "123",
                Data_Cadastro = DateTime.UtcNow,
                EmpresaId = "emp1"
            };

            var usuarioEditado = new Usuario
            {
                Id = "u1",
                Nome = "Nome Alterado",
                Email = "email@email.com"
            };

            _usuariosRepositoryMock.Setup(r => r.BuscarPorId("u1"))
                                   .ReturnsAsync(usuarioOriginal);

            _usuariosRepositoryMock.Setup(r => r.Atualizar(It.IsAny<Usuario>()))
                                   .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Edit(usuarioEditado);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            _usuariosRepositoryMock.Verify(r => r.Atualizar(It.IsAny<Usuario>()), Times.Once);
        }

        [Fact]
        public async Task DeleteConfirmed_ComIdValido_ExcluiERedireciona()
        {
            // Arrange
            _usuariosRepositoryMock.Setup(r => r.Excluir("u123"))
                                   .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteConfirmed("u123");

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            _usuariosRepositoryMock.Verify(r => r.Excluir("u123"), Times.Once);
        }

        [Fact]
        public async Task AlternarStatus_ComIdValido_InverteStatusERedireciona()
        {
            // Arrange
            _usuariosRepositoryMock.Setup(r => r.AtualizarStatus("u123", false))
                                   .Returns(Task.CompletedTask);

            // Act (se ativo == true, envia false para inativar)
            var result = await _controller.AlternarStatus("u123", true);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            _usuariosRepositoryMock.Verify(r => r.AtualizarStatus("u123", false), Times.Once);
        }
    }
}
