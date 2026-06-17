using Google.Cloud.Firestore;
using MVC_InventoryMasters.Models;
using MVC_InventoryMasters.Services;

namespace MVC_InventoryMasters.Repositories
{
    /// <summary>
    /// Repositório responsável pelo gerenciamento das medições de volume,
    /// realizando operações de cadastro, consulta, filtragem e geração
    /// de indicadores estatísticos das medições armazenadas no Firestore.
    /// </summary>
    /// <remarks>
    /// Esta classe centraliza o acesso à coleção de medições,
    /// abstraindo a comunicação com o banco de dados Firebase Firestore.
    /// </remarks>
    public class MedicaoVolumeRepository
    {
        private readonly string _colecao = "Medicoes";
        private readonly FirestoreDb _db;

        /// <summary>
        /// Inicializa uma nova instância do repositório de medições.
        /// </summary>
        /// <param name="firebaseService">
        /// Serviço responsável por fornecer a conexão com o Firebase Firestore.
        /// </param>
        public MedicaoVolumeRepository(FirebaseService firebaseService)
        {
            _db = firebaseService.Firestore;
        }

        /// <summary>
        /// Adiciona uma nova medição de volume na base de dados.
        /// </summary>
        /// <param name="medicao">
        /// Objeto contendo os dados da medição a ser armazenada.
        /// </param>
        /// <returns>
        /// Tarefa assíncrona responsável pela persistência da medição.
        /// </returns>
        public async Task Adicionar(MedicaoVolume medicao)
        {
            medicao.DataHora = DateTime.UtcNow;

            await _db
                .Collection(_colecao)
                .AddAsync(medicao);
        }

        /// <summary>
        /// Recupera todas as medições cadastradas no sistema.
        /// </summary>
        /// <returns>
        /// Lista contendo todas as medições encontradas no Firestore.
        /// </returns>
        public async Task<List<MedicaoVolume>> ListarTodos()
        {
            List<MedicaoVolume> lista = new();

            QuerySnapshot snapshot = await _db
                .Collection(_colecao)
                .GetSnapshotAsync();

            foreach (DocumentSnapshot doc in snapshot.Documents)
            {
                MedicaoVolume medicao = doc.ConvertTo<MedicaoVolume>();

                medicao.Id = doc.Id;

                lista.Add(medicao);
            }

            return lista;
        }

        /// <summary>
        /// Realiza a filtragem das medições utilizando múltiplos critérios.
        /// </summary>
        ///  <returns>
        /// Lista contendo as medições que atendem aos filtros informados.
        /// </returns>

        public async Task<List<MedicaoVolume>> FiltrarAvancado(
            string origem,
            string status,
            DateTime? dataInicio,
            DateTime? dataFim)
        {
            var lista = await ListarTodos();

            if (!string.IsNullOrWhiteSpace(origem))
            {
                lista = lista
                    .Where(x =>
                        (x.OrigemLeitura ?? "")
                        .Contains(origem, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                lista = lista
                    .Where(x =>
                        (x.Status ?? "")
                        .Contains(status, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (dataInicio.HasValue)
            {
                lista = lista
                    .Where(x =>
                        x.DataHora.HasValue &&
                        x.DataHora.Value.Date >= dataInicio.Value.Date)
                    .ToList();
            }

            if (dataFim.HasValue)
            {
                lista = lista
                    .Where(x =>
                        x.DataHora.HasValue &&
                        x.DataHora.Value.Date <= dataFim.Value.Date)
                    .ToList();
            }

            return lista;
        }

        /// <summary>
        /// Obtém um resumo estatístico das medições cadastradas.
        /// </summary>
        /// <remarks>
        /// O resumo inclui quantidade total de medições,
        /// média de volume, maior volume e menor volume registrado.
        /// </remarks>
        public async Task<MedicaoSummary> ObterSummary()
        {
            var medicoes = await ListarTodos();

            if (!medicoes.Any())
            {
                return new MedicaoSummary();
            }

            return new MedicaoSummary
            {
                TotalMedicoes = medicoes.Count,

                MediaVolume = medicoes
                    .Average(x => x.VolumeMedido ?? 0),

                MaxVolume = medicoes
                    .Max(x => x.VolumeMedido ?? 0),

                MinVolume = medicoes
                    .Min(x => x.VolumeMedido ?? 0)
            };
        }
    }


    /// <summary>
    /// Representa um resumo estatístico das medições cadastradas.
    /// </summary>
    /// <remarks>
    /// Utilizada para exibição de indicadores e métricas
    /// no Dashboard e relatórios do sistema.
    /// </remarks>
    public class MedicaoSummary
    {
        public int TotalMedicoes { get; set; }

        public double MediaVolume { get; set; }

        public double MaxVolume { get; set; }

        public double MinVolume { get; set; }
    }
}