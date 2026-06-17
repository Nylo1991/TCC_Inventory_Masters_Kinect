using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using Microsoft.Extensions.Logging;

namespace MVC_InventoryMasters.Services
{
    /// <summary>
    /// Serviço responsável pela configuração e inicialização
    /// da conexão com o Firebase Firestore.
    ///
    /// Esta classe centraliza a autenticação e o acesso ao banco
    /// de dados utilizado pela aplicação.
    /// </summary>
    /// <remarks>
    /// A conexão é estabelecida utilizando as credenciais
    /// configuradas no arquivo de configuração da aplicação.
    /// </remarks>
    public class FirebaseService
    {
        /// <summary>
        /// Instância do Firestore utilizada pelos repositórios
        /// para acesso aos dados da aplicação.
        /// </summary>
        private readonly FirestoreDb _firestore;
        private readonly ILogger<FirebaseService> _logger;
        public FirestoreDb Firestore => _firestore;

        /// <summary>
        /// Inicializa a conexão com o Firebase Firestore.
        /// </summary>
        /// <param name="configuration">
        /// Configurações da aplicação contendo os parâmetros
        /// necessários para autenticação no Firebase.
        /// </param>
        /// <exception cref="FileNotFoundException">
        /// Lançada quando o arquivo de credenciais não é encontrado.
        /// </exception>
        public FirebaseService(IConfiguration configuration)
        {
            string projectId = configuration["Firebase:ProjectId"];
            string credentialFileName = configuration["Firebase:CredentialPath"];
            
            string credentialPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, credentialFileName);

            try
            {
                if (!File.Exists(credentialPath))
                {
                    throw new FileNotFoundException(
                        "Arquivo de credenciais do Firebase não encontrado.");
                }

                Environment.SetEnvironmentVariable(
                    "GOOGLE_APPLICATION_CREDENTIALS",
                    credentialPath);

                if (FirebaseApp.DefaultInstance == null)
                {
                    FirebaseApp.Create(new AppOptions
                    {
                        Credential = GoogleCredential.FromFile(
                            credentialPath)
                    });
                }

                _firestore = FirestoreDb.Create(projectId);

                _logger.LogInformation(
                    "Conexão com Firebase inicializada com sucesso.");
            }
            catch (Exception ex)
            {
                _logger.LogCritical(
                    ex,
                    "Falha crítica ao inicializar a conexão com o Firebase.");

                throw new Exception(
                    "Não foi possível inicializar a conexão com o banco de dados.");
            }

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