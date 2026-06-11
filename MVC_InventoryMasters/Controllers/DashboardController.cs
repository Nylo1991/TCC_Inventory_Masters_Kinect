using Microsoft.AspNetCore.Mvc;
using MVC_InventoryMasters.Repositories;
using MVC_InventoryMasters.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MVC_InventoryMasters.Controllers
{
    public class DashboardController : Controller
    {
        private readonly MedicaoVolumeRepository _medicaoRepo;
        private readonly NotificacaoRepository _notificacaoRepo;
        private readonly ParceirosRepository _parceirosRepo;
        private readonly ParametrosSistemaRepository _parametrosRepo;
        private readonly UsuariosRepository _usuariosRepo;

        public DashboardController(
            MedicaoVolumeRepository medicaoRepo,
            NotificacaoRepository notificacaoRepo,
            ParceirosRepository parceirosRepo,
            ParametrosSistemaRepository parametrosRepo,
            UsuariosRepository usuariosRepo)
        {
            _medicaoRepo = medicaoRepo;
            _notificacaoRepo = notificacaoRepo;
            _parceirosRepo = parceirosRepo;
            _parametrosRepo = parametrosRepo;
            _usuariosRepo = usuariosRepo;
        }

        public async Task<IActionResult> Index()
        {
            // Busca dos dados básicos nos repositórios
            var parceiros = await _parceirosRepo.ListarTodos();
            var usuarios = await _usuariosRepo.ListarTodos();
            var medicoes = await _medicaoRepo.ListarTodos();
            var alertas = await _notificacaoRepo.ListarTodos();
            
            var parametros = _parametrosRepo.Buscar();
           
            var ultimaMedicao = medicoes.OrderByDescending(m => m.DataHora).FirstOrDefault()?.VolumeMedido ?? 0;

            // Se a capacidade não estiver configurada, usa 10000.0 como fallback de segurança
            double capacidade = parametros.CapacidadeMaxima > 0 ? parametros.CapacidadeMaxima : 10000.0;

            // Cálculo garantindo conversão para decimal para o ViewModel
            decimal percentual = capacidade > 0 ? (decimal)((double)ultimaMedicao / capacidade) * 100 : 0;
          
            var model = new DashboardViewModel
            {
                Parceiros = parceiros,
                Usuarios = usuarios,
                Medicoes = medicoes,
                Alertas = alertas,
                PercentualOcupacao = Math.Min(percentual, 100),
                Parametros = parametros 
            };

            return View(model);
        }
    }
}