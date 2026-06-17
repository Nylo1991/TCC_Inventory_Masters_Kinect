using Google.Cloud.Firestore;
using System.ComponentModel.DataAnnotations;

namespace MVC_InventoryMasters.Models
{
    /// <summary>
    /// Representa os parâmetros globais do sistema.
    /// Utilizado para controle de capacidade, alertas
    /// e notificações automáticas.
    /// </summary>
    [FirestoreData]
    public class ParametrosSistema
    {
        /// <summary>
        /// Capacidade máxima do reservatório.
        /// </summary>
        [FirestoreProperty]
        [Display(Name = "Capacidade Máxima (m³)")]
        [Required(ErrorMessage = "Informe a capacidade máxima.")]
        [Range(1, double.MaxValue,
            ErrorMessage = "A capacidade máxima deve ser maior que zero.")]
        public double CapacidadeMaxima { get; set; }

        /// <summary>
        /// Capacidade mínima utilizada para alertas.
        /// </summary>
        [FirestoreProperty]
        [Display(Name = "Capacidade Mínima (m³)")]
        [Required(ErrorMessage = "Informe a capacidade mínima.")]
        [Range(0, double.MaxValue,
            ErrorMessage = "A capacidade mínima não pode ser negativa.")]
        public double CapacidadeMinima { get; set; }

        /// <summary>
        /// Percentual para disparo de alertas.
        /// Exemplo: 80%.
        /// </summary>
        [FirestoreProperty]
        [Display(Name = "Percentual de Alerta (%)")]
        [Required(ErrorMessage = "Informe o percentual de alerta.")]
        [Range(1, 100,
            ErrorMessage = "O percentual deve estar entre 1 e 100.")]
        public int PercentualAlerta { get; set; }

        /// <summary>
        /// Data da última atualização das configurações.
        /// </summary>
        [FirestoreProperty]
        public DateTime DataAtualizacao { get; set; }

        /// <summary>
        /// Habilita envio automático de notificações.
        /// </summary>
        [FirestoreProperty]
        public bool NotificacaoAutomatica { get; set; } = true;

        /// <summary>
        /// Exibe alertas visuais no Dashboard.
        /// </summary>
        [FirestoreProperty]
        public bool ExibirAlertaDashboard { get; set; } = true;

        /// <summary>
        /// Parceiro padrão que receberá notificações automáticas.
        /// </summary>
        [FirestoreProperty]
        public string? ParceiroPadraoId { get; set; }

        /// <summary>
        /// Quantidade de dias sem coleta para gerar alerta.
        /// </summary>
        [FirestoreProperty]
        [Range(1, 365,
            ErrorMessage = "Informe um valor entre 1 e 365 dias.")]
        public int DiasSemColetaAlerta { get; set; } = 15;

        /// <summary>
        /// Retorna o percentual atual do estoque.
        /// Utilizado em validações e alertas.
        /// </summary>
        
        public double PercentualAtual { get; set; }
    }
}