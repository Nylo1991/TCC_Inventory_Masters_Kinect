using Microsoft.AspNetCore.Mvc;
using MVC_InventoryMasters.Repositories;
using MVC_InventoryMasters.Services;
using MVC_InventoryMasters.ViewModels;

namespace MVC_InventoryMasters.Controllers
{
    [ApiController]
    [Route("api/kinect")]
    public class KinectApiController : ControllerBase
    {
        private readonly TokenAcessoKinectService _tokenService;
        private readonly EmailTokenService _emailService;
        private readonly LogsSistemaRepository _logsRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<KinectApiController> _logger;

        public KinectApiController(
            TokenAcessoKinectService tokenService,
            EmailTokenService emailService,
            LogsSistemaRepository logsRepository,
            IConfiguration configuration,
            ILogger<KinectApiController> logger)
        {
            _tokenService = tokenService;
            _emailService = emailService;
            _logsRepository = logsRepository;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost("solicitar-token")]
        public async Task<IActionResult> SolicitarToken([FromBody] SolicitarTokenKinectRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    sucesso = false,
                    mensagem = "Informe um e-mail válido."
                });
            }

            try
            {
                var resultado = await _tokenService.GerarTokenParaEmail(request.Email!);

                if (!resultado.Sucesso || resultado.Token == null || resultado.Usuario == null)
                {
                    return NotFound(new
                    {
                        sucesso = false,
                        mensagem = resultado.Mensagem
                    });
                }

                int validadeMinutos = _configuration.GetValue<int?>("KinectAccess:TokenValidityMinutes") ?? 15;

                // O MVC continua sendo a autoridade do acesso: ele gera, registra e envia o token.
                await _emailService.EnviarTokenKinect(
                    resultado.Usuario.Email!,
                    resultado.Usuario.Nome ?? "usuário",
                    resultado.Token,
                    validadeMinutos);

                await _logsRepository.Registrar(
                    "TokenEnviadoKinect",
                    "Token solicitado pelo aplicativo Kinect e enviado ao e-mail cadastrado.",
                    "Informacao",
                    resultado.Usuario.Email,
                    resultado.Usuario.Id,
                    resultado.Usuario.EmpresaId);

                return Ok(new
                {
                    sucesso = true,
                    email = resultado.Usuario.Email,
                    mensagem = "Token enviado para o e-mail cadastrado."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao solicitar token pelo aplicativo Kinect.");

                return StatusCode(500, new
                {
                    sucesso = false,
                    mensagem = "Não foi possível solicitar o token no MVC."
                });
            }
        }

        [HttpPost("validar-token")]
        public async Task<IActionResult> ValidarToken([FromBody] ValidarTokenRequest request)
        {
            var resultado = await _tokenService.ValidarToken(request.Token);

            if (!resultado.TokenValido)
                return Unauthorized(resultado);

            return Ok(resultado);
        }
    }
}
