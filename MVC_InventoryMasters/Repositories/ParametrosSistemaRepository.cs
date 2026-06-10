using Google.Cloud.Firestore;
using MVC_InventoryMasters.Models;
using MVC_InventoryMasters.Services;

namespace MVC_InventoryMasters.Repositories
{
    /// <summary>
    /// Repositório responsável pelas configurações
    /// gerais do sistema.
    /// </summary>
    public class ParametrosSistemaRepository
    {
        private readonly string _colecao = "parametrosSistema";
        private readonly FirestoreDb _db;

        public ParametrosSistemaRepository(
            FirebaseService firebaseService)
        {
            _db = firebaseService.Firestore;
        }

        /// <summary>
        /// Retorna as configurações atuais do sistema.
        /// </summary>
        public ParametrosSistema Buscar()
        {
            try
            {
                var docRef = _db
                    .Collection(_colecao)
                    .Document("configuracao");

                var snapshot = docRef
                    .GetSnapshotAsync()
                    .Result;

                if (!snapshot.Exists)
                {
                    return new ParametrosSistema();
                }

                var dados = snapshot.ToDictionary();

                return new ParametrosSistema
                {
                    CapacidadeMaxima =
                        dados.TryGetValue("CapacidadeMaxima", out var capacidadeMaxima)
                            ? Convert.ToDouble(capacidadeMaxima)
                            : 0,

                    CapacidadeMinima =
                        dados.TryGetValue("CapacidadeMinima", out var capacidadeMinima)
                            ? Convert.ToDouble(capacidadeMinima)
                            : 0,

                    PercentualAlerta =
                        dados.TryGetValue("PercentualAlerta", out var percentualAlerta)
                            ? Convert.ToInt32(percentualAlerta)
                            : 80,

                    DataAtualizacao =
                        dados.TryGetValue("DataAtualizacao", out var dataAtualizacao)
                        && dataAtualizacao is Timestamp timestamp
                            ? timestamp.ToDateTime()
                            : DateTime.MinValue,

                    NotificacaoAutomatica =
                        dados.TryGetValue("NotificacaoAutomatica", out var notificacaoAutomatica)
                            ? Convert.ToBoolean(notificacaoAutomatica)
                            : true,

                    ExibirAlertaDashboard =
                        dados.TryGetValue("ExibirAlertaDashboard", out var exibirAlertaDashboard)
                            ? Convert.ToBoolean(exibirAlertaDashboard)
                            : true,

                    ParceiroPadraoId =
                        dados.TryGetValue("ParceiroPadraoId", out var parceiroPadraoId)
                            ? parceiroPadraoId?.ToString()
                            : null,

                    DiasSemColetaAlerta =
                        dados.TryGetValue("DiasSemColetaAlerta", out var diasSemColeta)
                            ? Convert.ToInt32(diasSemColeta)
                            : 15
                };
            }
            catch
            {
                return new ParametrosSistema();
            }
        }

        /// <summary>
        /// Salva as configurações do sistema.
        /// </summary>
        public void Salvar(ParametrosSistema parametros)
        {
            parametros.DataAtualizacao = DateTime.UtcNow;

            var dados = new Dictionary<string, object>
            {
                { "CapacidadeMaxima", parametros.CapacidadeMaxima },
                { "CapacidadeMinima", parametros.CapacidadeMinima },
                { "PercentualAlerta", parametros.PercentualAlerta },
                //{ "UnidadeMedida", parametros.UnidadeMedida ?? "m³" },
                { "DataAtualizacao", parametros.DataAtualizacao },

                { "NotificacaoAutomatica", parametros.NotificacaoAutomatica },
                { "ExibirAlertaDashboard", parametros.ExibirAlertaDashboard },
                { "ParceiroPadraoId", parametros.ParceiroPadraoId ?? string.Empty },
                { "DiasSemColetaAlerta", parametros.DiasSemColetaAlerta }
            };

            _db
                .Collection(_colecao)
                .Document("configuracao")
                .SetAsync(dados)
                .Wait();
        }
    }
}