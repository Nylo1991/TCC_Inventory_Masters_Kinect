using Google.Cloud.Firestore;

namespace InventoryMasters.Infrastructure.Firebase.Firebase;

public class FirestoreFactory
{
    private readonly FirestoreDb _db;

    public FirestoreFactory(
        FirebaseConfiguration config)
    {
        _db = FirestoreDb.Create(
            config.ProjectId);
    }

    public FirestoreDb GetDatabase()
    {
        return _db;
    }
}