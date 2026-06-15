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
    /// <summary>
    /// Controlador responsável por gerenciar as ações relacionadas às notificações do sistema,
    /// </summary>
    /// <remarks>Este controlador permite listar as notificações, processar a aceitação 
    /// de solicitações de coleta por parceiros,</remarks>
    /// <param></param>
    /// <return></return>
    public class NotificacoesController : Controller
    {
        private readonly NotificacaoRepository _repo;
        private readonly IHubContext<NotificacaoHub> _hubContext;
        private readonly ILogger<NotificacoesController> _logger;
        
        public NotificacoesController(NotificacaoRepository repo,
            IHubContext<NotificacaoHub> hubContext, ILogger<NotificacoesController> logger)
        {
            _repo = repo;
            _hubContext = hubContext;
            _logger = logger;
        }

        /// <summary>
        /// Exibe a lista de notificações para o usuário.
        /// </summary>
        /// <remarks>Este método busca todas as notificações do repositório e as exibe na view.
        /// </remarks>
        /// <param></param>
        /// <returns></returns>
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
        /// <remarks>Este método é acionado quando um parceiro aceita uma solicitação de coleta. Ele atualiza o status da notificação 
        /// para "Aceito" no banco de dados e envia uma notificação em tempo real para os clientes conectados usando SignalR.</remarks>
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

        /// <summary>
        /// Envia uma notificação em tempo real para todos os clientes conectados usando SignalR.
        /// </summary>
        /// remarks>Este método é responsável por enviar uma mensagem de notificação para todos os clientes conectados ao hub de notificações.
        /// Ele é chamado após a aceitação de uma coleta para informar os usuários sobre a atualização.</remarks>
        /// <param name="mensagem"></param>
        /// <returns></returns>
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