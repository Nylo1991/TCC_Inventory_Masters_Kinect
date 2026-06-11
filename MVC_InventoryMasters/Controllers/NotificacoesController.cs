using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MVC_InventoryMasters.Hubs;
using MVC_InventoryMasters.Repositories;
using MVC_InventoryMasters.Models;

namespace MVC_InventoryMasters.Controllers
{
    public class NotificacoesController : Controller
    {
        private readonly NotificacaoRepository _repo;
        private readonly IHubContext<NotificacaoHub> _hubContext;

        // O construtor injeta tanto o Repositório (dados) quanto o Hub (tempo real)
        public NotificacoesController(NotificacaoRepository repo, IHubContext<NotificacaoHub> hubContext)
        {
            _repo = repo;
            _hubContext = hubContext;
        }

        // 1. Ação para carregar a lista de notificações
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
                // Tenta atualizar no banco de dados
                bool sucesso = await _repo.AtualizarStatus(id, "Aceito");

                if (!sucesso)
                    return StatusCode(500, "Erro ao atualizar o banco de dados.");
               
                await _hubContext.Clients.All.SendAsync("RecarregarTabela");

                return Ok(new { success = true, message = "Coleta aceita com sucesso!" });
            }
            catch (Exception ex)
            {
                // Log do erro para depuração
                Console.WriteLine($"[Erro na Action AceitarColeta] {ex.Message}");

                return StatusCode(500, "Erro interno ao processar a solicitação.");
            }
        }
    }
}