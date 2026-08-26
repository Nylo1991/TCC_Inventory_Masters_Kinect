using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MVC_InventoryMasters.Controllers;
using MVC_InventoryMasters.Repositories;
using MVC_InventoryMasters.Services;
using MVC_InventoryMasters.ViewModels;
using System.Runtime.Serialization;

namespace MVC_InventoryMasters.Tests.Controllers
{
    public class AcessoControllerTests
    {
        private readonly TokenAcessoKinectService _tokenService;
        private readonly EmailTokenService _emailService;
        private readonly UsuariosRepository _usuariosRepository;
        private readonly LogsSistemaRepository _logsRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AcessoController> _logger;

        private readonly AcessoController _controller;

        public AcessoControllerTests()
        {
            // Cria os objetos sem executar os construtores
            // pois os testes abaixo não utilizam esses serviços.
            _tokenService = (TokenAcessoKinectService)
                FormatterServices.GetUninitializedObject(
                    typeof(TokenAcessoKinectService));

            _emailService = (EmailTokenService)
                FormatterServices.GetUninitializedObject(
                    typeof(EmailTokenService));

            _usuariosRepository = (UsuariosRepository)
                FormatterServices.GetUninitializedObject(
                    typeof(UsuariosRepository));

            _logsRepository = (LogsSistemaRepository)
                FormatterServices.GetUninitializedObject(
                    typeof(LogsSistemaRepository));

            _configuration = new ConfigurationBuilder()
                .Build();

            _logger = LoggerFactory
                .Create(builder => { })
                .CreateLogger<AcessoController>();

            // Cria a controller
            _controller = new AcessoController(
                _tokenService,
                _emailService,
                _usuariosRepository,
                _logsRepository,
                _configuration,
                _logger);

            // Configura o HttpContext
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }

        #region Login

        /// <summary>
        /// Testa se a tela de login é retornada corretamente.
        /// </summary>
        [Fact]
        public void Login_DeveRetornarView()
        {
            // Arrange

            // Act
            var resultado = _controller.Login();

            // Assert
            Assert.IsType<ViewResult>(resultado);

            var view = (ViewResult)resultado;

            Assert.NotNull(view.Model);
            Assert.IsType<LoginEmailViewModel>(view.Model);
        }

        #endregion

        #region Validar Token GET

        /// <summary>
        /// Testa se a tela de validação de token é retornada corretamente.
        /// </summary>
        [Fact]
        public void ValidarToken_DeveRetornarView()
        {
            // Arrange

            // Act
            var resultado = _controller.ValidarToken();

            // Assert
            Assert.IsType<ViewResult>(resultado);

            var view = (ViewResult)resultado;

            Assert.NotNull(view.Model);
            Assert.IsType<ValidarTokenViewModel>(view.Model);
        }

        #endregion

        #region Negado

        /// <summary>
        /// Testa se a tela de acesso negado é retornada corretamente.
        /// </summary>
        [Fact]
        public void Negado_DeveRetornarView()
        {
            // Arrange

            // Act
            var resultado = _controller.Negado();

            // Assert
            Assert.IsType<ViewResult>(resultado);
        }

        #endregion
    }
}