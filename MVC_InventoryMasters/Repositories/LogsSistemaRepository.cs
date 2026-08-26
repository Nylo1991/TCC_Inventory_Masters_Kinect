using Google.Cloud.Firestore;
using Microsoft.Extensions.Logging;
using MVC_InventoryMasters.Models;
using MVC_InventoryMasters.Services;

namespace MVC_InventoryMasters.Repositories
{
    public class LogsSistemaRepository : ILogsSistemaRepository
    {
        private readonly FirestoreDb _db;
        private readonly ILogger<LogsSistemaRepository> _logger;
        private readonly ContextoUsuarioService _contextoUsuario;
        private readonly string _colecao = "LogsSistema";

        public LogsSistemaRepository(
            FirebaseService firebaseService,
            ILogger<LogsSistemaRepository> logger,
            ContextoUsuarioService contextoUsuario)
        {
            _db = firebaseService.Firestore;
            _logger = logger;
            _contextoUsuario = contextoUsuario;
        }

        public async Task Registrar(
            string acao,
            string mensagem,
            string nivel = "Informacao",
            string? email = null,
            string? usuarioId = null,
            string? empresaId = null)
        {
            try
            {
                var log = new LogSistema
                {
                    Acao = acao,
                    Mensagem = mensagem,
                    Nivel = nivel,
                    Email = email,
                    UsuarioId = usuarioId,
                    EmpresaId = string.IsNullOrWhiteSpace(empresaId)
                        ? _contextoUsuario.ObterEmpresaId()
                        : empresaId,
                    DataHora = DateTime.UtcNow
                };

                await _db.Collection(_colecao).AddAsync(log);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao registrar log do sistema para a ação {Acao}.", acao);
            }
        }
    }
}
