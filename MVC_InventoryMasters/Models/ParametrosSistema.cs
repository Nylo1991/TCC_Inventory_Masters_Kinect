using Google.Cloud.Firestore;
using System.ComponentModel.DataAnnotations;

namespace MVC_InventoryMasters.Models
{
    [FirestoreData]
    public class ParametrosSistema
    {
        [FirestoreProperty]
        [Display(Name = "Capacidade Máxima")]
        public double CapacidadeMaxima { get; set; }

        [FirestoreProperty]
        [Display(Name = "Capacidade Mínima")]
        public double CapacidadeMinima { get; set; }

        [FirestoreProperty]
        [Display(Name = "Percentual de Alerta")]
        [Range(1, 100)]
        public int PercentualAlerta { get; set; }

        [FirestoreProperty]
        [Display(Name = "Unidade de Medida")]
        public string UnidadeMedida { get; set; } = "m³";

        [FirestoreProperty]
        public DateTime DataAtualizacao { get; set; }

        // ===========================
        // ABA NOTIFICAÇÕES (FUTURO)
        // ===========================

        [FirestoreProperty]
        public bool NotificacaoAutomatica { get; set; } = true;

        [FirestoreProperty]
        public bool ExibirAlertaDashboard { get; set; } = true;

        [FirestoreProperty]
        public string? ParceiroPadraoId { get; set; }

        [FirestoreProperty]
        public int DiasSemColetaAlerta { get; set; } = 15;
    }
}