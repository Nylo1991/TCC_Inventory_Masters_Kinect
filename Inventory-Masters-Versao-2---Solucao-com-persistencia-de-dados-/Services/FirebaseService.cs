using Google.Cloud.Firestore;
using InventoryMaster.Models;

namespace InventoryMasters.Services;

public class FirebaseService
{
    public FirestoreDb FirestoreDb { get; }

    public FirebaseService(IConfiguration configuration)
    {
        string path =
            configuration["Firebase:CredentialsPath"];

        Environment.SetEnvironmentVariable(
            "GOOGLE_APPLICATION_CREDENTIALS",
            path);

        FirestoreDb = FirestoreDb.Create(
            configuration["Firebase:ProjectId"]);
    }

    // =========================================
    // SALVAR MEDIÇÃO
    // =========================================

    public async Task SalvarMedicaoAsync(
        MedicaoVolume medicao)
    {
        CollectionReference collection =
            FirestoreDb.Collection("medicoes");

        await collection.AddAsync(medicao);
    }

    // =========================================
    // BUSCAR HISTÓRICO
    // =========================================

    public async Task<List<MedicaoVolume>>
        ObterMedicoesAsync()
    {
        Query query = FirestoreDb
            .Collection("medicoes")
            .OrderByDescending("Data_Hora")
            .Limit(20);

        QuerySnapshot snapshot =
            await query.GetSnapshotAsync();

        List<MedicaoVolume> lista = new();

        foreach (DocumentSnapshot document in
                 snapshot.Documents)
        {
            if (document.Exists)
            {
                lista.Add(
                    document.ConvertTo<MedicaoVolume>()
                );
            }
        }

        return lista
            .OrderBy(x => x.Data_Hora)
            .ToList();
    }
}