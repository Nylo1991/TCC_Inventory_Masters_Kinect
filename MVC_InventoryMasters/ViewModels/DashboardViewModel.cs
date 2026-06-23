using MVC_InventoryMasters.Models;
using System.Collections.Generic;
using System.Linq;

namespace MVC_InventoryMasters.ViewModels
{
    /// <summary>
    /// ViewModel responsável por consolidar os dados
    /// exibidos na tela principal do Dashboard.
    /// </summary>
    /// <remarks>
    /// Reúne informações de medições, notificações,
    /// parceiros, usuários e parâmetros do sistema,
    /// permitindo a exibição centralizada dos indicadores.
    /// </remarks>
    public class DashboardViewModel : BaseViewModel
    {
        /// <summary>
        /// Lista de medições de volume registradas.
        /// </summary>
        public List<MedicaoVolume> Medicoes { get; set; } = new();

        /// <summary>
        /// Lista de notificações e alertas do sistema.
        /// </summary>
        public List<Notificacao> Alertas { get; set; } = new();

        /// <summary>
        /// Lista de parceiros cadastrados.
        /// </summary>
        public List<Parceiro> Parceiros { get; set; } = new();

        /// <summary>
        /// Lista de usuários cadastrados.
        /// </summary>
        public List<Usuario> Usuarios { get; set; } = new();

        /// <summary>
        /// Percentual atual de ocupação do estoque.
        /// </summary>
        public decimal PercentualOcupacao { get; set; }

        /// <summary>
        /// Parâmetros de configuração do sistema.
        /// </summary>
        public ParametrosSistema Parametros { get; set; } = new();

        /// <summary>
        /// Mensagem de erro utilizada para exibição
        /// de falhas controladas na interface.
        /// </summary>
        public string MensagemErro { get; set; } = string.Empty;

        /// <summary>
        /// Quantidade total de parceiros cadastrados.
        /// </summary>
        public int TotalParceiros => Parceiros?.Count ?? 0;

        /// <summary>
        /// Quantidade total de medições registradas.
        /// </summary>
        public int TotalMedicoes => Medicoes?.Count ?? 0;

        /// <summary>
        /// Quantidade total de notificações registradas.
        /// </summary>
        public int TotalAlertas => Alertas?.Count ?? 0;

        /// <summary>
        /// Quantidade total de usuários cadastrados.
        /// </summary>
        public int TotalUsuarios => Usuarios?.Count ?? 0;

        /// <summary>
        /// Retorna as cinco notificações mais recentes.
        /// </summary>
        public List<Notificacao> UltimasNotificacoes =>
            Alertas?
                .OrderByDescending(n => n.DataHora)
                .Take(5)
                .ToList() ?? new List<Notificacao>();
    }
}