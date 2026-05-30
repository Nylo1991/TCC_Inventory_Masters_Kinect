using MVC_InventoryMasters.Models;
using System.Collections.Generic;

namespace MVC_InventoryMasters.ViewModels
{
    // Herda de BaseViewModel para ter acesso aos recursos de notificação de mudança
    public class DashboardViewModel : BaseViewModel
    {
        public List<MedicaoVolume> Medicoes { get; set; } = new List<MedicaoVolume>();
        public List<Notificacao> Alertas { get; set; } = new List<Notificacao>();
        public List<Parceiro> Parceiros { get; set; } = new List<Parceiro>();
        public ParametrosSistema Parametros { get; set; } = new ParametrosSistema();

        // Adiciona uma propriedade para mensagens de erro, caso o Firebase falhe
        public string MensagemErro { get; set; }
    }
}