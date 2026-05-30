using Google.Cloud.Firestore;
using MVC_InventoryMasters.Models;
using System.Collections.Generic;

namespace MVC_InventoryMasters.Repositories
{
    public class ParceirosRepository
    {
        // Define o nome da coleção no Firebase onde os parceiros estão armazenados
        private readonly string _colecao = "Parceiros";
        private readonly FirestoreDb _db;

        // O construtor recebe o ID do projeto para abrir a conexão
        public ParceirosRepository(string projectId)
        {
            _db = FirestoreDb.Create(projectId);
        }
        
        public List<Parceiro> ListarTodos()
        {
            // Cria uma lista vazia para armazenar os parceiros que serão buscados do banco
            List<Parceiro> lista = new List<Parceiro>();

            // Busca os documentos da coleção "Parceiros" de forma assíncrona 
            var documentos = _db.Collection(_colecao).GetSnapshotAsync().Result;
            
            foreach (var doc in documentos.Documents)
            {
                // Converte o documento do Firebase para a classe Parceiro usando o método ConvertTo<T>()
                Parceiro p = doc.ConvertTo<Parceiro>();

                // Converte de string para int.
                p.Id = int.Parse(doc.Id);
                                
                lista.Add(p);
            }
                        
            return lista;
        }
    }
}