using MVC_InventoryMasters.Models;
using System.Collections.Generic;
using System.Linq;

namespace MVC_InventoryMasters.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        public List<MedicaoVolume> Medicoes { get; set; } = new();
        public List<Notificacao> Alertas { get; set; } = new();
        public List<Parceiro> Parceiros { get; set; } = new();
        public List<Usuario> Usuarios { get; set; } = new();

        public decimal PercentualOcupacao { get; set; }
        public ParametrosSistema Parametros { get; set; } = new();
        public string MensagemErro { get; set; }

        public int TotalParceiros => Parceiros?.Count ?? 0;
        public int TotalMedicoes => Medicoes?.Count ?? 0;
        public int TotalAlertas => Alertas?.Count ?? 0;
        public int TotalUsuarios => Usuarios?.Count ?? 0;

        public List<Notificacao> UltimasNotificacoes =>
            Alertas?
                .OrderByDescending(n => n.DataHora)
                .Take(5)
                .ToList();
    }
}
