using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace MVC_InventoryMasters.Services
{
    public class FirebaseService
    {
        private readonly FirestoreDb _firestore;
        public FirestoreDb Firestore => _firestore;

        public FirebaseService(IConfiguration configuration)
        {
            string projectId = configuration["Firebase:ProjectId"];
            string credentialFileName = configuration["Firebase:CredentialPath"];

            // Resolve o caminho completo a partir da pasta base da aplicação (onde está o executável)
            string credentialPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, credentialFileName);

            // Verifica se o arquivo existe antes de tentar carregar
            if (!File.Exists(credentialPath))
            {
                throw new FileNotFoundException($"Arquivo de credenciais não encontrado em: {credentialPath}");
            }

            // Define a variável de ambiente para o SDK do Google
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialPath);

            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions
                {
                    Credential = GoogleCredential.FromFile(credentialPath)
                });
            }

            _firestore = FirestoreDb.Create(projectId);
        }
    }
}