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
                    dados.ContainsKey("CapacidadeMaxima")
                        ? Convert.ToDouble(dados["CapacidadeMaxima"])
                        : 0,

                CapacidadeMinima =
                    dados.ContainsKey("CapacidadeMinima")
                        ? Convert.ToDouble(dados["CapacidadeMinima"])
                        : 0,

                PercentualAlerta =
                    dados.ContainsKey("PercentualAlerta")
                        ? Convert.ToInt32(dados["PercentualAlerta"])
                        : 80,

                UnidadeMedida =
                    dados.ContainsKey("UnidadeMedida")
                        ? dados["UnidadeMedida"]?.ToString() ?? "m³"
                        : "m³",

                DataAtualizacao =
                    dados.ContainsKey("DataAtualizacao")
                        ? ((Timestamp)dados["DataAtualizacao"]).ToDateTime()
                        : DateTime.MinValue,

                NotificacaoAutomatica =
                    dados.ContainsKey("NotificacaoAutomatica")
                        ? Convert.ToBoolean(dados["NotificacaoAutomatica"])
                        : true,

                ExibirAlertaDashboard =
                    dados.ContainsKey("ExibirAlertaDashboard")
                        ? Convert.ToBoolean(dados["ExibirAlertaDashboard"])
                        : true,

                ParceiroPadraoId =
                    dados.ContainsKey("ParceiroPadraoId")
                        ? dados["ParceiroPadraoId"]?.ToString()
                        : null,

                DiasSemColetaAlerta =
                    dados.ContainsKey("DiasSemColetaAlerta")
                        ? Convert.ToInt32(dados["DiasSemColetaAlerta"])
                        : 15
            };
        }

        /// <summary>
        /// Salva as configurações do sistema.
        /// </summary>
        public void Salvar(ParametrosSistema parametros)
        {
            var dados = new Dictionary<string, object>
            {
                { "CapacidadeMaxima", parametros.CapacidadeMaxima },
                { "CapacidadeMinima", parametros.CapacidadeMinima },
                { "PercentualAlerta", parametros.PercentualAlerta },
                { "UnidadeMedida", parametros.UnidadeMedida ?? "m³" },
                { "DataAtualizacao", DateTime.UtcNow },

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