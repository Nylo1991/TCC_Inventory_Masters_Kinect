using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MVC_InventoryMasters.Hubs;
using MVC_InventoryMasters.Repositories;
using MVC_InventoryMasters.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        /// <summary>
        /// Carrega a lista de notificações para exibição na view.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            try
            {
                var lista = await _repo.ListarTodos();
                return View(lista ?? new List<Notificacao>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar a lista de notificações.");
                return RedirectToAction("Error", "Home");
            }
        }

        /// <summary>
        /// Processa a aceitação de uma solicitação de coleta por um parceiro, 
        /// atualiza o status no banco de dados e notifica os clientes conectados via SignalR.
        /// </summary>
        /// <param name="id">O ID da notificação a ser aceita.</param>
        /// <returns>Retorna um resultado JSON indicando sucesso ou um status HTTP 500 em caso de erro.</returns>
        [HttpPost]
        public async Task<IActionResult> AceitarColeta(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                _logger.LogWarning("Tentativa de aceitar coleta com ID nulo ou vazio.");
                return BadRequest("ID da notificação não fornecido.");
            }

            try
            {
                bool sucesso = await _repo.AtualizarStatus(id, "Aceito");

                if (!sucesso)
                {
                    _logger.LogError("Falha ao atualizar status no repositório para o ID: {Id}", id);
                    return StatusCode(500, "Erro ao atualizar o banco de dados.");
                }

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
                _logger.LogError(ex, "Erro crítico ao processar a aceitação de coleta. ID: {Id}", id);
                return StatusCode(500, "Erro interno ao processar a solicitação.");
            }
        }

        // Método para notificar clientes conectados
        private async Task NotificarClientes(string mensagem)
        {
            try
            {
                await _hubContext.Clients.All.SendAsync("ReceberNotificacao", mensagem);
            }
            catch (Exception ex)
            {                
                _logger.LogError(ex, "Erro ao enviar notificação via SignalR.");
            }
        }
    }
}