using Microsoft.Extensions.Configuration;
using MVC_InventoryMasters.Models;
using MVC_InventoryMasters.Repositories;
using MVC_InventoryMasters.ViewModels;
using System.Security.Cryptography;
using System.Text;

namespace MVC_InventoryMasters.Services
{
    public class TokenAcessoKinectService : ITokenAcessoKinectService
    {
        private readonly TokensAcessoKinectRepository _tokensRepository;
        private readonly UsuariosRepository _usuariosRepository;
        private readonly LogsSistemaRepository _logsRepository;
        private readonly int _validadeMinutos;

        public TokenAcessoKinectService(
            TokensAcessoKinectRepository tokensRepository,
            UsuariosRepository usuariosRepository,
            LogsSistemaRepository logsRepository,
            IConfiguration configuration)
        {
            _tokensRepository = tokensRepository;
            _usuariosRepository = usuariosRepository;
            _logsRepository = logsRepository;
            _validadeMinutos = configuration.GetValue<int?>("KinectAccess:TokenValidityMinutes") ?? 15;
        }

        public async Task<(bool Sucesso, string Mensagem, string? Token, Usuario? Usuario)> GerarTokenParaEmail(string email)
        {
            var usuario = await _usuariosRepository.BuscarPorEmail(email);

            if (usuario == null || !usuario.Ativo)
            {
                await _logsRepository.Registrar(
                    "TokenSolicitado",
                    "Solicitação de token recusada: e-mail não encontrado ou usuário inativo.",
                    "Aviso",
                    email);

                return (false, "E-mail não encontrado ou usuário inativo.", null, null);
            }

            string token = GerarTokenNumerico();

            var tokenAcesso = new TokenAcessoKinect
            {
                UsuarioId = usuario.Id,
                UsuarioNome = usuario.Nome,
                Email = usuario.Email,
                EmpresaId = string.IsNullOrWhiteSpace(usuario.EmpresaId)
                    ? ContextoUsuarioService.EmpresaPadraoId
                    : usuario.EmpresaId,
                Empresa = usuario.Empresa,
                Perfil = usuario.Perfil,
                TokenHash = GerarHash(token),
                CriadoEm = DateTime.UtcNow,
                ExpiraEm = DateTime.UtcNow.AddMinutes(_validadeMinutos)
            };

            await _tokensRepository.Adicionar(tokenAcesso);

            await _logsRepository.Registrar(
                "TokenSolicitado",
                "Token de acesso ao Kinect solicitado com sucesso.",
                "Informacao",
                usuario.Email,
                usuario.Id,
                tokenAcesso.EmpresaId);

            return (true, "Token gerado com sucesso.", token, usuario);
        }

        public async Task<ValidacaoTokenResultadoViewModel> ValidarToken(string? token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                await _logsRepository.Registrar("TokenInvalido", "Tentativa de validação sem token.", "Aviso");

                return new ValidacaoTokenResultadoViewModel
                {
                    TokenValido = false,
                    EmailValidado = false,
                    Mensagem = "Informe o token de acesso."
                };
            }

            string hash = GerarHash(token.Trim());
            var tokenAcesso = await _tokensRepository.BuscarAtivoPorHash(hash);

            if (tokenAcesso == null)
            {
                await _logsRepository.Registrar("TokenInvalido", "Tentativa de validação com token inexistente.", "Aviso");

                return new ValidacaoTokenResultadoViewModel
                {
                    TokenValido = false,
                    EmailValidado = false,
                    Mensagem = "Token inválido."
                };
            }

            if (tokenAcesso.ExpiraEm < DateTime.UtcNow)
            {
                await _logsRepository.Registrar(
                    "TokenExpirado",
                    "Tentativa de validação com token expirado.",
                    "Aviso",
                    tokenAcesso.Email,
                    tokenAcesso.UsuarioId,
                    tokenAcesso.EmpresaId);

                return new ValidacaoTokenResultadoViewModel
                {
                    TokenValido = false,
                    EmailValidado = true,
                    Email = tokenAcesso.Email,
                    Mensagem = "Token expirado."
                };
            }

            await _tokensRepository.MarcarComoUtilizado(tokenAcesso);

            await _logsRepository.Registrar(
                "TokenValidado",
                "Token de acesso ao Kinect validado com sucesso.",
                "Informacao",
                tokenAcesso.Email,
                tokenAcesso.UsuarioId,
                tokenAcesso.EmpresaId);

            return new ValidacaoTokenResultadoViewModel
            {
                TokenValido = true,
                EmailValidado = true,
                Usuario = tokenAcesso.UsuarioNome,
                Empresa = tokenAcesso.Empresa,
                EmpresaId = tokenAcesso.EmpresaId,
                Email = tokenAcesso.Email,
                Mensagem = "Token validado com sucesso."
            };
        }

        public static string GerarHash(string valor)
        {
            byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(valor));
            return Convert.ToHexString(bytes);
        }

        private static string GerarTokenNumerico()
        {
            return RandomNumberGenerator.GetInt32(100000, 999999).ToString();
        }
    }
}
