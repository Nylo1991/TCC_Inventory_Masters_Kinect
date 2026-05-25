using Google.Cloud.Firestore;

namespace InventoryMaster.Models;

public class NotificacaoParceiros
{
    [FirestoreDocumentId]
    public string Id { get; set; }
    [FirestoreProperty]
    public string Status_Entrega { get; set; }
    [FirestoreProperty]
    public int fk_Parceiro_Id { get; set; }
    [FirestoreProperty]
    public string fk_Notificacao_Id { get; set; }
}