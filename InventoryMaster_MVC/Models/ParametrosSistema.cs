using Google.Cloud.Firestore;

namespace InventoryMaster.Models;

public class ParametrosSistema
{
    [FirestoreDocumentId]
    public string Id { get; set; }
    [FirestoreProperty]
    public decimal Volume_Maximo { get; set; }
    [FirestoreProperty]
    public decimal Volume_Minimo { get; set; }
    [FirestoreProperty]
    public DateTime? Data_Atualizacao { get; set; }
    [FirestoreProperty]
    public bool Email_Notificacao_Ativo { get; set; }
}