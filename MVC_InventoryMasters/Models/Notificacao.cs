using Google.Cloud.Firestore;

namespace MVC_InventoryMasters.Models
{
    /// <summary>
    /// Representa uma notificação/alerta gerado pelo sistema.
    /// Pode ser usado para eventos do Kinect, alertas e mensagens do dashboard.
    /// </summary>
    [FirestoreData]
    public class Notificacao
    {
        /// <summary>
        /// ID do documento no Firestore.
        /// (substitui o uso de int Id tradicional)
        /// </summary>
        [FirestoreDocumentId]
        public string? Id { get; set; }

        /// <summary>
        /// Volume medido pelo Kinect no momento do evento.
        /// </summary>
        [FirestoreProperty]
        public double? VolumeMedido { get; set; }

        /// <summary>
        /// ID do parceiro relacionado ao evento.
        /// </summary>
        [FirestoreProperty]
        public string? ParceiroId { get; set; }

        /// <summary>
        /// Data e hora do evento (para dashboard em tempo real).
        /// </summary>
        [FirestoreProperty]
        public DateTime DataHora { get; set; }

        /// <summary>
        /// Status do envio ou do evento.
        /// Ex: Sucesso, Alerta, Erro.
        /// </summary>
        [FirestoreProperty]
        public string? StatusEnvio { get; set; }

        [FirestoreProperty]
        public string? ParceiroQueAceitouId { get; set; } 

        /// <summary>
        /// Mensagem descritiva do evento.
        /// </summary>
        [FirestoreProperty]
        public string? Mensagem { get; set; }

    }


}