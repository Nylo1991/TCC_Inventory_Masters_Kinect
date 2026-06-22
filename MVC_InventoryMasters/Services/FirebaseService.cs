using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace MVC_InventoryMasters.Services
{
    /// <summary>
    /// Serviço responsável por inicializar e fornecer
    /// acesso ao Firebase Firestore.
    /// </summary>
    public class FirebaseService
    {
        /// <summary>
        /// Instância do Firestore utilizada pela aplicação.
        /// </summary>
        private readonly FirestoreDb _firestore;

        /// <summary>
        /// Propriedade de acesso ao Firestore.
        /// </summary>
        public FirestoreDb Firestore => _firestore;

        /// <summary>
        /// Construtor da classe FirebaseService,
        /// responsável por inicializar a conexão
        /// com o Firebase Firestore.
        /// </summary>
        /// <param name="configuration">
        /// Configurações da aplicação.
        /// </param>
        /// <exception cref="FileNotFoundException">
        /// Lançada quando o arquivo de credenciais
        /// do Firebase não é encontrado.
        /// </exception>
        public FirebaseService(
            IConfiguration configuration)
        {
            string projectId =
                configuration["Firebase:ProjectId"];

            string credentialFileName =
                configuration["Firebase:CredentialPath"];

            string credentialPath =
                Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    credentialFileName);

            if (!File.Exists(credentialPath))
            {
                throw new FileNotFoundException(
                    $"Arquivo de credenciais não encontrado em: {credentialPath}");
            }

            Environment.SetEnvironmentVariable(
                "GOOGLE_APPLICATION_CREDENTIALS",
                credentialPath);

            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(
                    new AppOptions
                    {
                        Credential =
                            GoogleCredential.FromFile(
                                credentialPath)
                    });
            }

            _firestore =
                FirestoreDb.Create(projectId);
        }
    }
}