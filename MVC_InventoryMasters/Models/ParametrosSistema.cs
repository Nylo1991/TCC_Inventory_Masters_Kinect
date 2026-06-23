using Google.Cloud.Firestore;
using System;
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

        [FirestoreProperty]
        public string? EmpresaId { get; set; }

        [FirestoreProperty]
        public bool AtivarSistemaCalibracao { get; set; }

        [FirestoreProperty]
        [Display(Name = "Raio de Detecção Kinect (m)")]
        [Range(0, 100,
            ErrorMessage = "Informe um raio entre 0 e 100 metros.")]
        public double RaioDeteccaoKinect { get; set; }

        [FirestoreProperty]
        public bool HabilitarZonaExclusaoDeteccao { get; set; }

        [FirestoreProperty]
        [Display(Name = "Taxa de Amostragem de Volume (minutos)")]
        [Range(1, 1440,
            ErrorMessage = "Informe uma taxa entre 1 e 1440 minutos.")]
        public int TaxaAmostragemVolumeMinutos { get; set; } = 10;

        [FirestoreProperty]
        [Display(Name = "Duração Máxima de Medição (segundos)")]
        [Range(1, 86400,
            ErrorMessage = "Informe uma duração entre 1 e 86400 segundos.")]
        public int DuracaoMaximaMedicaoSegundos { get; set; } = 2000;

        [FirestoreProperty]
        [Display(Name = "Tipo de Alerta")]
        public string TipoAlertaPadrao { get; set; } = "Critico";

        [FirestoreProperty]
        [Display(Name = "Template de Mensagem")]
        [StringLength(1000,
            ErrorMessage = "O template deve ter no máximo 1000 caracteres.")]
        public string TemplateMensagemPadrao { get; set; } =
            "Olá, {{Parceiro}}.\n\nO estoque em {{EspacoID}} atingiu {{VolumePercentual}}% da capacidade crítica às {{DataHora}}. Por favor, realize a coleta imediata.\n\nAcompanhe no painel.";

        [FirestoreProperty]
        public bool CanalEmailAtivo { get; set; } = true;

        [FirestoreProperty]
        public bool CanalWhatsAppAtivo { get; set; } = true;

        [FirestoreProperty]
        public bool CanalDashboardPushAtivo { get; set; } = true;

        [FirestoreProperty]
        [Display(Name = "Nome do Remetente no WhatsApp")]
        [StringLength(80,
            ErrorMessage = "O nome do remetente deve ter no máximo 80 caracteres.")]
        public string? NomeRemetenteWhatsApp { get; set; }

        [FirestoreProperty]
        [Display(Name = "Minutos para Escalonamento")]
        [Range(1, 1440,
            ErrorMessage = "Informe um tempo entre 1 e 1440 minutos.")]
        public int EscalonamentoMinutos { get; set; } = 10;

        [FirestoreProperty]
        [Display(Name = "Canal de Escalonamento")]
        public string CanalEscalonamento { get; set; } = "E-mail";

        /// <summary>
        /// Retorna o percentual atual do estoque.
        /// Utilizado em validações e alertas.
        /// </summary>

        public double PercentualAtual { get; set; }
    }
}
