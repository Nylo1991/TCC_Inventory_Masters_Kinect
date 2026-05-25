using Google.Cloud.Firestore;

namespace InventoryMaster.Models;

[FirestoreData]
public class MedicaoVolume
{
    [FirestoreDocumentId]
    public string Id { get; set; }

    [FirestoreProperty]
    public string Origem_Leitura { get; set; }

    [FirestoreProperty]
    public double Volume_Medido { get; set; }

    [FirestoreProperty]
    public DateTime Data_Hora { get; set; }

    [FirestoreProperty]
    public int fk_Usuario_Id { get; set; }
}