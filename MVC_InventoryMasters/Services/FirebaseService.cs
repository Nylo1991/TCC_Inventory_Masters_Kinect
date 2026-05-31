using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Builder.Extensions;

namespace MVC_InventoryMasters.Services
{
    public class FirebaseService
    {
        private readonly FirestoreDb _firestore;

        public FirestoreDb Firestore => _firestore;

        public FirebaseService(IConfiguration configuration)
        {
            string projectId =
                configuration["Firebase:ProjectId"];

            string credentialPath =
                configuration["Firebase:CredentialPath"];

            Environment.SetEnvironmentVariable(
                "GOOGLE_APPLICATION_CREDENTIALS",
                Path.GetFullPath(credentialPath));

            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions
                {
                    Credential =
                        GoogleCredential.FromFile(
                            credentialPath)
                });
            }

            _firestore = FirestoreDb.Create(projectId);
        }
    }
}