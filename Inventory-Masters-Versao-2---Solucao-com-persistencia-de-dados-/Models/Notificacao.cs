using Google.Cloud.Firestore;

namespace InventoryMaster.Models;

public class Notificacao
{
    [FirestoreDocumentId]
    public string Id { get; set; }

    [FirestoreProperty]
    public int Quantidade_destinatario { get; set; }
    [FirestoreProperty]
    public string Mensagem { get; set; }
    [FirestoreProperty]
    public string Status_Envio { get; set; }
    [FirestoreProperty]
    public decimal Volume_Momento { get; set; }
    [FirestoreProperty]
    public DateTime? Data_Envio { get; set; }
    [FirestoreProperty]
    public string fk_Usuario_Id { get; set; }
}