using Google.Cloud.Firestore;
using System;

namespace InventoryMaster.Models;

[FirestoreData]
public class Parceiro
{
    [FirestoreDocumentId]
    public string Id { get; set; }

    [FirestoreProperty]
    public string Nome { get; set; } = string.Empty;

    [FirestoreProperty]
    public string Empresa { get; set; } = string.Empty;

    [FirestoreProperty]
    public string Telefone { get; set; } = string.Empty;

    [FirestoreProperty]
    public string Email { get; set; } = string.Empty;

    [FirestoreProperty]
    public string Endereco { get; set; } = string.Empty;

    [FirestoreProperty]
    public DateTime Data_Cadastro { get; set; } = DateTime.UtcNow;

    [FirestoreProperty]
    public bool Ativo { get; set; } = true;
}