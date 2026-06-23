using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MVC_InventoryMasters.Hubs;
using MVC_InventoryMasters.Filters;
using MVC_InventoryMasters.Repositories;
using MVC_InventoryMasters.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
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
    [PermissaoAuthorize(PermissoesSistema.NotificacoesVisualizar)]
    public class NotificacoesController : Controller
    {
        private readonly NotificacaoRepository _repo;
        private readonly ParceirosRepository _parceirosRepository;
        private readonly IHubContext<NotificacaoHub> _hubContext;
        private readonly ILogger<NotificacoesController> _logger;
        
        public NotificacoesController(
            NotificacaoRepository repo,
            ParceirosRepository parceirosRepository,
            IHubContext<NotificacaoHub> hubContext,
            ILogger<NotificacoesController> logger)
        {
            _repo = repo;
            _parceirosRepository = parceirosRepository;
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
        public async Task<IActionResult> Index(
            int pagina = 1,
            DateTime? dataInicio = null,
            DateTime? dataFim = null,
            string parceiroId = null,
            string status = null,
            string tipo = null)
        {
            try
            {
                const int itensPorPagina = 10;

                var parceiros = await _parceirosRepository.ListarPorEmpresa();
                var lista = await _repo.ListarPorEmpresa() ?? new List<Notificacao>();

                if (dataInicio.HasValue)
                    lista = lista.Where(n => n.DataHora.Date >= dataInicio.Value.Date).ToList();

                if (dataFim.HasValue)
                    lista = lista.Where(n => n.DataHora.Date <= dataFim.Value.Date).ToList();

                if (!string.IsNullOrWhiteSpace(parceiroId))
                    lista = lista.Where(n => n.ParceiroId == parceiroId).ToList();

                if (!string.IsNullOrWhiteSpace(status))
                    lista = lista.Where(n => string.Equals(n.StatusEnvio, status, StringComparison.OrdinalIgnoreCase)).ToList();

                if (!string.IsNullOrWhiteSpace(tipo))
                    lista = lista.Where(n => string.Equals(n.Tipo, tipo, StringComparison.OrdinalIgnoreCase)).ToList();

                lista = lista.OrderByDescending(n => n.DataHora).ToList();

                int totalRegistros = lista.Count;
                int totalPaginas = (int)Math.Ceiling(totalRegistros / (double)itensPorPagina);
                pagina = Math.Clamp(pagina, 1, Math.Max(1, totalPaginas));

                ViewBag.Parceiros = parceiros;
                ViewBag.DataInicio = dataInicio?.ToString("yyyy-MM-dd");
                ViewBag.DataFim = dataFim?.ToString("yyyy-MM-dd");
                ViewBag.ParceiroId = parceiroId;
                ViewBag.Status = status;
                ViewBag.Tipo = tipo;
                ViewBag.TotalRegistros = totalRegistros;
                ViewBag.TotalPaginas = totalPaginas;
                ViewBag.PaginaAtual = pagina;
                ViewBag.TotalSucesso = lista.Count(n => n.StatusEnvio == "Aceito" || n.StatusEnvio == "Sucesso" || n.StatusEnvio == "Resolvido");
                ViewBag.TotalErro = lista.Count(n => n.StatusEnvio == "Erro");
                ViewBag.TotalPendente = lista.Count(n => n.StatusEnvio == "Pendente");

                var paginaLista = lista
                    .Skip((pagina - 1) * itensPorPagina)
                    .Take(itensPorPagina)
                    .ToList();

                return View(paginaLista);
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
