using Google.Cloud.Firestore;
using MVC_InventoryMasters.Models;
using System.Collections.Generic;

namespace MVC_InventoryMasters.Repositories
{
    public class MedicaoVolumeRepository
    {        
        private readonly string _colecao = "Medicoes";
        private readonly FirestoreDb _db;

        public MedicaoVolumeRepository(string projectId)
        {
            _db = FirestoreDb.Create(projectId);
        }

        public List<MedicaoVolume> ListarTodas()
        {
            List<MedicaoVolume> lista = new List<MedicaoVolume>();

            // Obtém todos os documentos da coleção "Medicoes"
            var documentos = _db.Collection(_colecao).GetSnapshotAsync().Result;

            // Converte cada documento para o modelo MedicaoVolume e adiciona à lista
            foreach (var doc in documentos.Documents)
            {
                MedicaoVolume m = doc.ConvertTo<MedicaoVolume>();                

                lista.Add(m);
            }

            return lista;
        }

        public void Adicionar(MedicaoVolume medicao)
        {
            // Adiciona um novo documento à coleção "Medicoes" com as medições vindas do kinect
            _db.Collection(_colecao).AddAsync(medicao).Wait();
        }
    }
}