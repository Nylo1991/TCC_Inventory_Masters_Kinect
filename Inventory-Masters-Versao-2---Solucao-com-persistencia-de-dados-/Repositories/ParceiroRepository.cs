using Google.Cloud.Firestore;
using InventoryMaster.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryMaster.Data;

public class ParceiroRepository
{
    private readonly FirestoreDb _firestoreDb;

  
    private const string Colecao = "parceiros";


    public ParceiroRepository(FirestoreDb firestoreDb)
    {
        _firestoreDb = firestoreDb;
    }

  
    public async Task InserirAsync(Parceiro parceiro)
    {
        
        DocumentReference docRef = _firestoreDb.Collection(Colecao).Document();

        
        Dictionary<string, object> dados = new Dictionary<string, object>
        {
            { "Nome", parceiro.Nome ?? "" },
            { "Empresa", parceiro.Empresa ?? "" },
            { "Telefone", parceiro.Telefone ?? "" },
            { "Email", parceiro.Email ?? "" },
            { "Endereco", parceiro.Endereco ?? "" },
            { "Data_Cadastro", DateTime.UtcNow },
            { "Ativo", true }
        };

        
        await docRef.SetAsync(dados);
    }

 
    public async Task<List<Parceiro>> ListarParceiroAsync()
    {
        var lista = new List<Parceiro>();

        Query query = _firestoreDb.Collection(Colecao);
        QuerySnapshot snapshot = await query.GetSnapshotAsync();

        foreach (DocumentSnapshot document in snapshot.Documents)
        {
            if (document.Exists)
            {
                
                var dados = document.ToDictionary();

                Parceiro parceiro = new Parceiro
                {
                    Id = document.Id,
                    Nome = dados.ContainsKey("Nome") ? dados["Nome"]?.ToString() ?? "" : "",
                    Empresa = dados.ContainsKey("Empresa") ? dados["Empresa"]?.ToString() ?? "" : "",
                    Telefone = dados.ContainsKey("Telefone") ? dados["Telefone"]?.ToString() ?? "" : "",
                    Email = dados.ContainsKey("Email") ? dados["Email"]?.ToString() ?? "" : "",
                    Endereco = dados.ContainsKey("Endereco") ? dados["Endereco"]?.ToString() ?? "" : ""
                };

                lista.Add(parceiro);
            }
        }

        return lista;
    }
}