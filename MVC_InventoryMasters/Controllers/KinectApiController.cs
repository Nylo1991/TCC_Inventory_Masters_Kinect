using Microsoft.AspNetCore.Mvc;
using MVC_InventoryMasters.Services;
using MVC_InventoryMasters.ViewModels;

namespace MVC_InventoryMasters.Controllers
{
    [ApiController]
    [Route("api/kinect")]
    public class KinectApiController : ControllerBase
    {
        private readonly TokenAcessoKinectService _tokenService;

        public KinectApiController(TokenAcessoKinectService tokenService)
        {
            _tokenService = tokenService;
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
