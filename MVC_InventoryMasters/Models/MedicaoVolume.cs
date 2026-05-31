using Google.Cloud.Firestore;
using System;

namespace MVC_InventoryMasters.Models
{
    [FirestoreData]
    public class MedicaoVolume
    {
        [FirestoreDocumentId]
        public string? Id { get; set; }

        [FirestoreProperty]
        public string? OrigemLeitura { get; set; }

        [FirestoreProperty]
        public string? Status { get; set; }

        [FirestoreProperty]
        public double? VolumeMedido { get; set; }

        [FirestoreProperty]
        public DateTime? DataHora { get; set; }
    }
}