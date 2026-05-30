using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

namespace InventoryMasters.Infrastructure.Firebase.Firebase;

public static class FirebaseInitializer
{
    public static void Initialize(
        FirebaseConfiguration config)
    {
        if (FirebaseApp.DefaultInstance != null)
            return;

        FirebaseApp.Create(
            new AppOptions
            {
                Credential =
                    GoogleCredential.FromFile(
                        config.CredentialsPath)
            });
    }
}