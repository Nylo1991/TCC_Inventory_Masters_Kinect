using Google.Cloud.Firestore;
using MVC_InventoryMasters.Models;
using MVC_InventoryMasters.Services;
using System.Collections.Generic;

namespace MVC_InventoryMasters.Repositories
{
    public class ParceirosRepository
    {
        // Define o nome da coleção no Firebase onde os parceiros estão armazenados
        private readonly string _colecao = "Parceiros";
        private readonly FirestoreDb _db;

        // O construtor recebe o ID do projeto para abrir a conexão
        public ParceirosRepository(FirebaseService firebaseService)
        {
            _db = firebaseService.Firestore;
        }

        public async Task<List<Parceiro>> ListarTodos()
        {
            // Cria uma lista vazia para armazenar os parceiros
            List<Parceiro> lista = new List<Parceiro>();

            // Busca os documentos da coleção "Parceiros"
            var documentos = await _db
                .Collection(_colecao)
                .GetSnapshotAsync();

            foreach (var doc in documentos.Documents)
            {
                // Converte o documento para a classe Parceiro
                Parceiro p = doc.ConvertTo<Parceiro>();

                // Salva o ID do documento
                p.Id = doc.Id;

                lista.Add(p);
            }

            return lista;
        }
    }
}