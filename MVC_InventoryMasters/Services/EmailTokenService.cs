using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace MVC_InventoryMasters.Services
{
    public class EmailTokenService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailTokenService> _logger;

        public EmailTokenService(IConfiguration configuration, ILogger<EmailTokenService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task EnviarTokenKinect(string email, string nomeUsuario, string token, int validadeMinutos)
        {
            string? host = _configuration["Smtp:Host"];
            string? remetente = _configuration["Smtp:From"];
            string? usuario = _configuration["Smtp:User"];
            string? senha = _configuration["Smtp:Password"];

            if (string.IsNullOrWhiteSpace(host) ||
                string.IsNullOrWhiteSpace(remetente) ||
                string.IsNullOrWhiteSpace(usuario) ||
                string.IsNullOrWhiteSpace(senha))
            {
                // Fallback de desenvolvimento: mantém o fluxo testável sem expor senha de e-mail no código.
                _logger.LogWarning(
                    "SMTP não configurado. Token Kinect para {Email}: {Token}. Validade: {Validade} minutos.",
                    email,
                    token,
                    validadeMinutos);

                await Task.CompletedTask;
                return;
            }

            using var mensagem = new MailMessage();
            mensagem.From = new MailAddress(remetente, "Inventory Masters");
            mensagem.To.Add(email);
            mensagem.Subject = "Token de acesso ao Kinect";
            mensagem.SubjectEncoding = Encoding.UTF8;
            mensagem.BodyEncoding = Encoding.UTF8;
            mensagem.IsBodyHtml = true;
            mensagem.Body =
                $"""
                <div style="font-family:Arial,sans-serif;line-height:1.5;color:#111827">
                    <h2 style="color:#15803d;margin-bottom:8px">Inventory Masters</h2>
                    <p>Olá, <strong>{WebUtility.HtmlEncode(nomeUsuario)}</strong>.</p>
                    <p>Seu token de acesso ao Kinect é:</p>
                    <p style="font-size:28px;font-weight:700;letter-spacing:4px;color:#15803d">
                        {WebUtility.HtmlEncode(token)}
                    </p>
                    <p>Validade: <strong>{validadeMinutos} minutos</strong>.</p>
                    <p style="color:#6b7280">Se você não solicitou este acesso, ignore esta mensagem.</p>
                </div>
                """;

            using var smtp = new SmtpClient(host)
            {
                Port = _configuration.GetValue<int?>("Smtp:Port") ?? 587,
                EnableSsl = _configuration.GetValue<bool?>("Smtp:EnableSsl") ?? true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(usuario, senha)
            };

            await smtp.SendMailAsync(mensagem);
        }
    }
}
