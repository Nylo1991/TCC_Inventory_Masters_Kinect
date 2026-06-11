using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MVC_InventoryMasters.Hubs;
using MVC_InventoryMasters.Repositories;
using MVC_InventoryMasters.Models;
using Microsoft.Extensions.Logging;

namespace MVC_InventoryMasters.Controllers
{
    public class NotificacoesController : Controller
    {
        private readonly NotificacaoRepository _repo;
        private readonly IHubContext<NotificacaoHub> _hubContext;
        private readonly ILogger<NotificacoesController> _logger;

        // O construtor injeta o Repositório, o Hub e o Logger
        public NotificacoesController(NotificacaoRepository repo,
            IHubContext<NotificacaoHub> hubContext, ILogger<NotificacoesController> logger)
        {
            _repo = repo;
            _hubContext = hubContext;
            _logger = logger;
        }

        // Carrega a lista de notificações
        public async Task<IActionResult> Index()
        {
            var lista = await _repo.ListarTodos();
            return View(lista ?? new List<Notificacao>());
        }

        /// <summary>
        /// Processa a aceitação de uma solicitação de coleta por um parceiro, 
        /// atualiza o status no banco de dados e notifica os clientes conectados via SignalR.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// Retorna um resultado HTTP indicando sucesso ou falha na operação.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> AceitarColeta(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("ID da notificação não fornecido.");

            try
            {
                bool sucesso = await _repo.AtualizarStatus(id, "Aceito");

                if (!sucesso)
                    return StatusCode(500, "Erro ao atualizar o banco de dados.");

                // Notifica clientes sobre a aceitação da coleta
                await NotificarClientes("Uma nova coleta foi aceita!");

                return Ok(new
                {
                    success = true,
                    message = "Coleta aceita com sucesso!"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Erro ao processar a solicitação de aceitação de coleta. ID: {Id}", id);
                return StatusCode(500,
                    "Erro interno ao processar a solicitação.");
            }
        }

        // Método para notificar clientes conectados
        private async Task NotificarClientes(string mensagem)
        {
            await _hubContext.Clients.All.SendAsync("ReceberNotificacao", mensagem);
        }
    }
}
