using MVC_InventoryMasters.Models;

namespace MVC_InventoryMasters.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        public List<MedicaoVolume> Medicoes { get; set; } = new();
        public List<Notificacao> Alertas { get; set; } = new();
        public List<Parceiro> Parceiros { get; set; } = new();
        public List<Usuario> Usuarios { get; set; } = new();
        public List<Notificacao> Notificacoes { get; set; } = new();

        public ParametrosSistema Parametros { get; set; } = new();
        public string MensagemErro { get; set; }

        // KPIs
        public int TotalParceiros => Parceiros?.Count ?? 0;
        public int TotalMedicoes => Medicoes?.Count ?? 0;
        public int TotalAlertas => Alertas?.Count ?? 0;
        public int TotalUsuarios => Usuarios?.Count ?? 0;

        // Últimos registros (ORDENAÇÃO CORRETA)
        public List<Parceiro> UltimosParceiros =>
            Parceiros?
                .OrderByDescending(p => p.Data_Cadastro)
                .Take(5)
                .ToList();

        /*public List<MedicaoVolume> UltimasMedicoes =>
            Medicoes?
                .OrderByDescending(m => m.Data_Cadastro)
                .Take(5)
                .ToList();*/
    }
}