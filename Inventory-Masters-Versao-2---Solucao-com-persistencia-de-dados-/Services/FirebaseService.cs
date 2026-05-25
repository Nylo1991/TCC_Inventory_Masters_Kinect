using Google.Cloud.Firestore;

namespace InventoryMasters.Services;

public class FirebaseService
{
    public FirestoreDb FirestoreDb { get; }

    public FirebaseService(IConfiguration configuration)
    {
        string path = configuration["Firebase:CredentialsPath"];

        Environment.SetEnvironmentVariable(
            "GOOGLE_APPLICATION_CREDENTIALS",
            path);

        FirestoreDb = FirestoreDb.Create(
            configuration["Firebase:ProjectId"]);
    }
}