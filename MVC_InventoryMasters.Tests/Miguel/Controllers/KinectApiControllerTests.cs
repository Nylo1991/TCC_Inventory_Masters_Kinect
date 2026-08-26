using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using MVC_InventoryMasters.Controllers;
using MVC_InventoryMasters.Models;
using MVC_InventoryMasters.Repositories;
using MVC_InventoryMasters.Services;
using MVC_InventoryMasters.ViewModels;
using System.Threading.Tasks;
using Xunit;

namespace MVC_InventoryMasters.Tests.Controllers
{
    [Trait("Integrante", "Miguel")]
    public class KinectApiControllerTests
    {
        private readonly Mock<ITokenAcessoKinectService> _tokenServiceMock;
        private readonly Mock<IEmailTokenService> _emailServiceMock;
        private readonly Mock<ILogsSistemaRepository> _logsRepositoryMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<ILogger<KinectApiController>> _loggerMock;
        private readonly KinectApiController _controller;

        public KinectApiControllerTests()
        {
            _tokenServiceMock = new Mock<ITokenAcessoKinectService>();
            _emailServiceMock = new Mock<IEmailTokenService>();
            _logsRepositoryMock = new Mock<ILogsSistemaRepository>();
            _configurationMock = new Mock<IConfiguration>();
            _loggerMock = new Mock<ILogger<KinectApiController>>();

            var configSectionMock = new Mock<IConfigurationSection>();
            configSectionMock.Setup(c => c.Value).Returns("15");
            _configurationMock.Setup(c => c.GetSection("KinectAccess:TokenValidityMinutes")).Returns(configSectionMock.Object);

            _controller = new KinectApiController(
                _tokenServiceMock.Object,
                _emailServiceMock.Object,
                _logsRepositoryMock.Object,
                _configurationMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task SolicitarToken_DeveRetornarBadRequest_QuandoModelStateInvalido()
        {
            // Arrange
            _controller.ModelState.AddModelError("Email", "E-mail é obrigatório");
            var request = new SolicitarTokenKinectRequest();

            // Act
            var resultado = await _controller.SolicitarToken(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(resultado);
            Assert.NotNull(badRequestResult.Value);
        }

        [Fact]
        public async Task SolicitarToken_DeveRetornarOkEEnviarEmail_QuandoSucesso()
        {
            // Arrange
            var request = new SolicitarTokenKinectRequest { Email = "teste@empresa.com" };
            var usuarioFake = new Usuario
            {
                Id = "1",
                Nome = "Teste",
                Email = "teste@empresa.com",
                EmpresaId = "1"
            };

            // Retorno usando a Tupla esperada pelo ITokenAcessoKinectService
            _tokenServiceMock.Setup(s => s.GerarTokenParaEmail(request.Email))
                .ReturnsAsync((true, "Token gerado com sucesso.", "123456", usuarioFake));

            // Act
            var resultado = await _controller.SolicitarToken(request);

            // Assert
            Assert.IsType<OkObjectResult>(resultado);

            _emailServiceMock.Verify(s => s.EnviarTokenKinect(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once);
            _logsRepositoryMock.Verify(r => r.Registrar(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ValidarToken_DeveRetornarOk_QuandoTokenValido()
        {
            // Arrange
            var request = new ValidarTokenRequest { Token = "123456" };
            var retornoViewModel = new ValidacaoTokenResultadoViewModel
            {
                TokenValido = true,
                EmailValidado = true,
                Mensagem = "Token válido."
            };

            _tokenServiceMock.Setup(s => s.ValidarToken(request.Token))
                .ReturnsAsync(retornoViewModel);

            // Act
            var resultado = await _controller.ValidarToken(request);

            // Assert
            Assert.IsType<OkObjectResult>(resultado);
        }

        [Fact]
        public async Task ValidarToken_DeveRetornarUnauthorized_QuandoTokenInvalido()
        {
            // Arrange
            var request = new ValidarTokenRequest { Token = "000000" };
            var retornoViewModel = new ValidacaoTokenResultadoViewModel
            {
                TokenValido = false,
                EmailValidado = false,
                Mensagem = "Token inválido."
            };

            _tokenServiceMock.Setup(s => s.ValidarToken(request.Token))
                .ReturnsAsync(retornoViewModel);

            // Act
            var resultado = await _controller.ValidarToken(request);

            // Assert
            Assert.IsType<UnauthorizedObjectResult>(resultado);
        }
    }
}
