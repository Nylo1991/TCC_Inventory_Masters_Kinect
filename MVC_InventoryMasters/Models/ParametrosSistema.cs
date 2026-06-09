using Google.Cloud.Firestore;
using System.ComponentModel.DataAnnotations;

namespace MVC_InventoryMasters.Models
{
    [FirestoreData]
    public class ParametrosSistema
    {
        /// <summary>
        /// Volume máximo suportado pelo armazenamento.
        /// </summary>
        [FirestoreProperty]
        [Display(Name = "Capacidade Máxima")]
        public double CapacidadeMaxima { get; set; }

        /// <summary>
        /// Volume mínimo considerado para alerta.
        /// </summary>
        [FirestoreProperty]
        [Display(Name = "Capacidade Mínima")]
        public double CapacidadeMinima { get; set; }

        /// <summary>
        /// Percentual para disparo de alertas.
        /// Ex.: 80%
        /// </summary>
        [FirestoreProperty]
        [Display(Name = "Percentual de Alerta")]
        [Range(1, 100)]
        public int PercentualAlerta { get; set; }

        /// <summary>
        /// Unidade utilizada nas medições.
        /// </summary>
        [FirestoreProperty]
        [Display(Name = "Unidade de Medida")]
        public string UnidadeMedida { get; set; } = "m³";

        /// <summary>
        /// Data da última atualização.
        /// </summary>
        [FirestoreProperty]
        public DateTime DataAtualizacao { get; set; }

        // ===========================
        // ABA NOTIFICAÇÕES (FUTURO)
        // ===========================

        /// <summary>
        /// Habilita notificações automáticas.
        /// </summary>
        [FirestoreProperty]
        public bool NotificacaoAutomatica { get; set; } = true;

        /// <summary>
        /// Exibe alertas diretamente no Dashboard.
        /// </summary>
        [FirestoreProperty]
        public bool ExibirAlertaDashboard { get; set; } = true;

        /// <summary>
        /// Parceiro padrão para notificações automáticas.
        /// </summary>
        [FirestoreProperty]
        public string ParceiroPadraoId { get; set; }

        /// <summary>
        /// Quantidade de dias sem coleta para gerar alerta.
        /// </summary>
        [FirestoreProperty]
        public int DiasSemColetaAlerta { get; set; } = 15;
    }
}