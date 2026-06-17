using Google.Cloud.Firestore;
using MVC_InventoryMasters.Models;
using MVC_InventoryMasters.Services;
using Microsoft.Extensions.Logging;

namespace MVC_InventoryMasters.Repositories
{
    /// <summary>
    /// Repositório responsável pelo gerenciamento das configurações
    /// e parâmetros gerais do sistema.
    /// </summary>
    /// <remarks>
    /// Centraliza o acesso às configurações armazenadas no Firebase Firestore,
    /// permitindo consulta, atualização e cálculos relacionados aos parâmetros
    /// operacionais do sistema.
    /// </remarks>
    public class ParametrosSistemaRepository
    {
        private readonly string _colecao = "parametrosSistema";
        private readonly FirestoreDb _db;
        private readonly ILogger<ParametrosSistemaRepository> _logger;

        /// <summary>
        /// Inicializa uma nova instância do repositório de parâmetros do sistema.
        /// </summary>
        /// <param name="firebaseService">
        /// Serviço responsável por fornecer a conexão com o Firebase Firestore.
        /// </param>
        public ParametrosSistemaRepository(
            FirebaseService firebaseService,
            ILogger<ParametrosSistemaRepository> logger)
        {
            _db = firebaseService.Firestore;
            _logger = logger;
        }

        /// <summary>
        /// Obtém as configurações atualmente cadastradas no sistema.
        /// </summary>
        /// <remarks>
        /// Caso não exista uma configuração cadastrada ou ocorra alguma falha
        /// durante a consulta, será retornada uma instância padrão de
        /// <see cref="ParametrosSistema"/>.
        /// </remarks>
        /// <returns>
        /// Objeto contendo os parâmetros configurados no sistema.
        /// </returns>
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
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Erro ao recuperar os parâmetros do sistema.");

                return new ParametrosSistema();
            }
        }

        /// <summary>
        /// Salva as configurações do sistema.
        /// </summary>
        public void Salvar(ParametrosSistema parametros)
        {
            try
            {
                parametros.DataAtualizacao = DateTime.UtcNow;

                var dados = new Dictionary<string, object>
        {
            { "CapacidadeMaxima", parametros.CapacidadeMaxima },
            { "CapacidadeMinima", parametros.CapacidadeMinima },
            { "PercentualAlerta", parametros.PercentualAlerta },
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

                _logger.LogInformation(
                    "Parâmetros do sistema atualizados com sucesso.");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Erro ao salvar os parâmetros do sistema.");

                throw new Exception(
                    "Não foi possível salvar os parâmetros do sistema.");
            }
        }

        /// <summary>
        /// Calcula o percentual de ocupação do estoque.
        /// </summary>
        /// <param name="volumeAtual">
        /// Volume atualmente ocupado no estoque.
        /// </param>
        /// <param name="capacidadeMaxima">
        /// Capacidade máxima configurada para o estoque.
        /// </param>
        /// <returns>
        /// Percentual de ocupação calculado.
        /// Caso a capacidade máxima seja menor ou igual a zero,
        /// será retornado o valor zero.
        /// </returns>
        public double CalcularPercentualOcupacao(
            double volumeAtual,
            double capacidadeMaxima)
        {
            if (capacidadeMaxima <= 0)
                return 0;

            return (volumeAtual / capacidadeMaxima) * 100;
        }
    }
}