using Google.Cloud.Firestore;
using Microsoft.Extensions.Logging;
using MVC_InventoryMasters.Models;
using MVC_InventoryMasters.Services;

namespace MVC_InventoryMasters.Repositories
{
    public class TokensAcessoKinectRepository
    {
        private readonly FirestoreDb _db;
        private readonly ILogger<TokensAcessoKinectRepository> _logger;
        private readonly string _colecao = "TokensAcessoKinect";

        public TokensAcessoKinectRepository(
            FirebaseService firebaseService,
            ILogger<TokensAcessoKinectRepository> logger)
        {
            _db = firebaseService.Firestore;
            _logger = logger;
        }

        public async Task Adicionar(TokenAcessoKinect token)
        {
            await _db.Collection(_colecao).AddAsync(token);
        }

        public async Task<TokenAcessoKinect?> BuscarAtivoPorHash(string tokenHash)
        {
            try
            {
                var snapshot = await _db
                    .Collection(_colecao)
                    .WhereEqualTo("TokenHash", tokenHash)
                    .WhereEqualTo("Utilizado", false)
                    .WhereEqualTo("Revogado", false)
                    .GetSnapshotAsync();

                var doc = snapshot.Documents.FirstOrDefault();

                if (doc == null)
                    return null;

                var token = doc.ConvertTo<TokenAcessoKinect>();
                token.Id = doc.Id;
                return token;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar token ativo.");
                return null;
            }
        }

        public async Task MarcarComoUtilizado(TokenAcessoKinect token)
        {
            if (string.IsNullOrWhiteSpace(token.Id))
                return;

            await _db
                .Collection(_colecao)
                .Document(token.Id)
                .UpdateAsync(new Dictionary<string, object>
                {
                    { "Utilizado", true },
                    { "ValidadoEm", DateTime.UtcNow }
                });
        }
    }
}
