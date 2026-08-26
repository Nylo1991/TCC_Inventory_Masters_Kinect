using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using MVC_InventoryMasters.Controllers;
using MVC_InventoryMasters.Models;
using MVC_InventoryMasters.Repositories;
using System;
using Xunit;

namespace MVC_InventoryMasters.Tests
{
    public class ParametrosControllerTests
    {
        private readonly Mock<IParametrosSistemaRepository> _mockRepo;
        private readonly Mock<ILogger<ParametrosController>> _mockLogger;
        private readonly ParametrosController _controller;

        public ParametrosControllerTests()
        {
            _mockRepo = new Mock<IParametrosSistemaRepository>();
            _mockLogger = new Mock<ILogger<ParametrosController>>();

            _controller = new ParametrosController(_mockRepo.Object, _mockLogger.Object);

            // Configuração do TempData
            var httpContext = new DefaultHttpContext();
            var tempDataProvider = new Mock<ITempDataProvider>();
            _controller.TempData = new TempDataDictionary(httpContext, tempDataProvider.Object);
        }

        [Fact]
        public void Index_DeveRetornarViewComParametros()
        {
            // Arrange
            var parametrosExemplo = new ParametrosSistema { CapacidadeMinima = 10, CapacidadeMaxima = 100 };
            _mockRepo.Setup(r => r.Buscar()).Returns(parametrosExemplo);

            // Act
            var result = _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ParametrosSistema>(viewResult.Model);
            Assert.Equal(10, model.CapacidadeMinima);
            Assert.Equal(100, model.CapacidadeMaxima);
        }

        [Fact]
        public void Index_QuandoOcorrerExcecao_DeveRetornarViewComModeloVazioETempDataErro()
        {
            // Arrange
            _mockRepo.Setup(r => r.Buscar()).Throws(new Exception("Erro no banco"));

            // Act
            var result = _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<ParametrosSistema>(viewResult.Model);
            Assert.Equal("Ocorreu um erro ao carregar as configurações.", _controller.TempData["Erro"]);
        }

        [Fact]
        public void Salvar_ComModelStateInvalido_DeveRetornarViewIndexComModelo()
        {
            // Arrange
            var model = new ParametrosSistema();
            _controller.ModelState.AddModelError("CapacidadeMaxima", "Campo Obrigatório");

            // Act
            var result = _controller.Salvar(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Index", viewResult.ViewName);
            Assert.Equal(model, viewResult.Model);
        }

        [Fact]
        public void Salvar_ComCapacidadeMinimaMaiorOuIgualAMaxima_DeveAdicionarErroNoModelStateERetornarViewIndex()
        {
            // Arrange
            var model = new ParametrosSistema { CapacidadeMinima = 100, CapacidadeMaxima = 50 };

            // Act
            var result = _controller.Salvar(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Index", viewResult.ViewName);
            Assert.False(_controller.ModelState.IsValid);
            Assert.True(_controller.ModelState.ContainsKey(nameof(ParametrosSistema.CapacidadeMinima)));
        }

        [Fact]
        public void Salvar_SemAlteracoes_DeveDefinirTempDataAvisoERedirecionarParaIndex()
        {
            // Arrange
            var parametrosAtuais = new ParametrosSistema
            {
                CapacidadeMinima = 10,
                CapacidadeMaxima = 100,
                PercentualAlerta = 80
            };

            var modelSemAlteracao = new ParametrosSistema
            {
                CapacidadeMinima = 10,
                CapacidadeMaxima = 100,
                PercentualAlerta = 80
            };

            _mockRepo.Setup(r => r.Buscar()).Returns(parametrosAtuais);

            // Act
            var result = _controller.Salvar(modelSemAlteracao);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(ParametrosController.Index), redirectResult.ActionName);
            Assert.Equal("Nenhuma alteração foi realizada.", _controller.TempData["Aviso"]);
            _mockRepo.Verify(r => r.Salvar(It.IsAny<ParametrosSistema>()), Times.Never);
        }

        [Fact]
        public void Salvar_ComAlteracoes_DeveSalvarEDefinirTempDataSucesso()
        {
            // Arrange
            var parametrosAtuais = new ParametrosSistema
            {
                CapacidadeMinima = 10,
                CapacidadeMaxima = 100
            };

            var modelComAlteracao = new ParametrosSistema
            {
                CapacidadeMinima = 10,
                CapacidadeMaxima = 200 // Valor alterado
            };

            _mockRepo.Setup(r => r.Buscar()).Returns(parametrosAtuais);

            // Act
            var result = _controller.Salvar(modelComAlteracao);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(ParametrosController.Index), redirectResult.ActionName);
            Assert.Equal("Configurações atualizadas com sucesso.", _controller.TempData["Sucesso"]);
            _mockRepo.Verify(r => r.Salvar(modelComAlteracao), Times.Once);
        }

        [Fact]
        public void Salvar_QuandoOcorrerExcecao_DeveDefinirTempDataErroERetornarViewIndex()
        {
            // Arrange
            var parametrosAtuais = new ParametrosSistema { CapacidadeMinima = 10, CapacidadeMaxima = 100 };
            var model = new ParametrosSistema { CapacidadeMinima = 10, CapacidadeMaxima = 200 };

            _mockRepo.Setup(r => r.Buscar()).Returns(parametrosAtuais);
            _mockRepo.Setup(r => r.Salvar(It.IsAny<ParametrosSistema>())).Throws(new Exception("Erro de gravação"));

            // Act
            var result = _controller.Salvar(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Index", viewResult.ViewName);
            Assert.Equal("Erro interno ao salvar configurações. Tente novamente mais tarde.", _controller.TempData["Erro"]);
        }

        [Fact]
        public void IniciarCalibracao_ComSucesso_DeveAtivarCalibracaoESalvar()
        {
            // Arrange
            var parametros = new ParametrosSistema { AtivarSistemaCalibracao = false };
            _mockRepo.Setup(r => r.Buscar()).Returns(parametros);

            // Act
            var result = _controller.IniciarCalibracao();

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(ParametrosController.Index), redirectResult.ActionName);
            Assert.True(parametros.AtivarSistemaCalibracao);
            Assert.Equal("Nova calibração do Kinect iniciada com sucesso.", _controller.TempData["Sucesso"]);
            _mockRepo.Verify(r => r.Salvar(parametros), Times.Once);
        }

        [Fact]
        public void IniciarCalibracao_QuandoOcorrerExcecao_DeveDefinirTempDataErroERedirecionar()
        {
            // Arrange
            _mockRepo.Setup(r => r.Buscar()).Throws(new Exception("Falha no repositório"));

            // Act
            var result = _controller.IniciarCalibracao();

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(ParametrosController.Index), redirectResult.ActionName);
            Assert.Equal("Não foi possível iniciar a calibração do Kinect.", _controller.TempData["Erro"]);
        }

        [Fact]
        public void RestaurarPadroes_ComSucesso_DeveObterPadroesESalvar()
        {
            // Arrange
            var parametrosPadrao = new ParametrosSistema { CapacidadeMinima = 0, CapacidadeMaxima = 1000 };
            _mockRepo.Setup(r => r.ObterPadroes()).Returns(parametrosPadrao);

            // Act
            var result = _controller.RestaurarPadroes();

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(ParametrosController.Index), redirectResult.ActionName);
            Assert.Equal("Padrões globais restaurados com sucesso.", _controller.TempData["Sucesso"]);
            _mockRepo.Verify(r => r.Salvar(parametrosPadrao), Times.Once);
        }

        [Fact]
        public void RestaurarPadroes_QuandoOcorrerExcecao_DeveDefinirTempDataErroERedirecionar()
        {
            // Arrange
            _mockRepo.Setup(r => r.ObterPadroes()).Throws(new Exception("Falha ao obter padrões"));

            // Act
            var result = _controller.RestaurarPadroes();

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(ParametrosController.Index), redirectResult.ActionName);
            Assert.Equal("Não foi possível restaurar os padrões globais.", _controller.TempData["Erro"]);
        }
    }
}