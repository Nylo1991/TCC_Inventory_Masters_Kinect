using MVC_InventoryMasters.Models;
using MVC_InventoryMasters.ViewModels;

namespace MVC_InventoryMasters.Services;

public interface ITokenAcessoKinectService
{
    Task<(bool Sucesso, string Mensagem, string? Token, Usuario? Usuario)> GerarTokenParaEmail(string email);
    Task<ValidacaoTokenResultadoViewModel> ValidarToken(string? token);
}

public interface IEmailTokenService
{
    Task EnviarTokenKinect(string email, string nomeUsuario, string token, int validadeMinutos);
}
