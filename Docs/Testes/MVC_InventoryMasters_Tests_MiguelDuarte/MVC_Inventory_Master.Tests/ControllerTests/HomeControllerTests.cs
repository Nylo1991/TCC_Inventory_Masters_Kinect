using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using MVC_InventoryMasters.Controllers;
using MVC_InventoryMasters.Models;
using System;
using Xunit;

namespace MVC_InventoryMasters.Tests.Controllers
{
    public class HomeControllerTests
    {
        [Fact]
        public void Index_DeveRedirecionarParaDashboard()
        {
            // Arrange
            var controller = new HomeController();

            // Act
            var resultado = controller.Index();

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(resultado);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Dashboard", redirectResult.ControllerName);
        }

        [Fact]
        public void Error_DeveRetornarViewComModel_ERegistrarLog_QuandoHouverExcecao()
        {
            // Arrange
            var exceptionFeatureMock = new Mock<IExceptionHandlerPathFeature>();
            exceptionFeatureMock.Setup(f => f.Error).Returns(new Exception("Erro de teste simulado"));
            exceptionFeatureMock.Setup(f => f.Path).Returns("/alguma-rota");

            var featuresMock = new Mock<IFeatureCollection>();
            featuresMock.Setup(f => f.Get<IExceptionHandlerPathFeature>()).Returns(exceptionFeatureMock.Object);

            var loggerMock = new Mock<ILogger<HomeController>>();
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(x => x.GetService(typeof(ILogger<HomeController>))).Returns(loggerMock.Object);

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(c => c.Features).Returns(featuresMock.Object);
            httpContextMock.Setup(c => c.RequestServices).Returns(serviceProviderMock.Object);
            httpContextMock.Setup(c => c.TraceIdentifier).Returns("trace-id-123");

            // 2. Controller recebe o TempData falso para não quebrar a geração da View
            var controller = new HomeController()
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = httpContextMock.Object
                },
                TempData = new Mock<ITempDataDictionary>().Object
            };

            // Act
            var resultado = controller.Error();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(resultado);
            var model = Assert.IsType<ErrorViewModel>(viewResult.Model);

            Assert.NotNull(model.RequestId);

            loggerMock.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Error),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Once);
        }

        [Fact]
        public void Error_DeveRetornarViewComModel_SemRegistrarLog_QuandoNaoHouverExcecao()
        {
            // Arrange
            var featuresMock = new Mock<IFeatureCollection>();
            featuresMock.Setup(f => f.Get<IExceptionHandlerPathFeature>()).Returns((IExceptionHandlerPathFeature?)null);

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(c => c.Features).Returns(featuresMock.Object);
            httpContextMock.Setup(c => c.TraceIdentifier).Returns("trace-id-456");

            // 3. O mesmo ajuste é feito no segundo teste
            var controller = new HomeController()
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = httpContextMock.Object
                },
                TempData = new Mock<ITempDataDictionary>().Object
            };

            // Act
            var resultado = controller.Error();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(resultado);
            var model = Assert.IsType<ErrorViewModel>(viewResult.Model);

            Assert.Equal("trace-id-456", model.RequestId);
        }
    }
}