using Google.Cloud.Firestore;
using MVC_InventoryMasters.Models;
using MVC_InventoryMasters.Services;

namespace MVC_InventoryMasters.Repositories
{
    /// <summary>
    /// Responsável por gerenciar as medições do Kinect
    /// na coleção "Medicoes" do Firestore.
    /// </summary>
    public class MedicaoVolumeRepository
    {
        private readonly string _colecao = "Medicoes";
        private readonly FirestoreDb _db;

        public MedicaoVolumeRepository(FirebaseService firebaseService)
        {
            _db = firebaseService.Firestore;
        }

        /// <summary>
        /// Salva uma nova medição no Firestore.
        /// </summary>
        public async Task Adicionar(MedicaoVolume medicao)
        {
            medicao.DataHora = DateTime.UtcNow;

            await _db
                .Collection(_colecao)
                .AddAsync(medicao);
        }

        /// <summary>
        /// Retorna todas as medições registradas.
        /// </summary>
        public async Task<List<MedicaoVolume>> ListarTodos()
        {
            List<MedicaoVolume> lista = new();

            QuerySnapshot snapshot = await _db
                .Collection(_colecao)
                .GetSnapshotAsync();

            foreach (DocumentSnapshot doc in snapshot.Documents)
            {
                MedicaoVolume m = doc.ConvertTo<MedicaoVolume>();
                m.Id = doc.Id;
                lista.Add(m);
            }

            return lista;
        }

        /// <summary>
        /// Retorna resumo estatístico das medições.
        /// Usado no Dashboard (KPIs).
        /// </summary>
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
                MediaVolume = medicoes.Average(x => x.VolumeMedido ?? 0),
                MaxVolume = medicoes.Max(x => x.VolumeMedido ?? 0),
                MinVolume = medicoes.Min(x => x.VolumeMedido ?? 0)
            };
        }
    }

    /// <summary>
    /// Classe de resumo para Dashboard.
    /// </summary>
    public class MedicaoSummary
    {
        public int TotalMedicoes { get; set; }
        public double MediaVolume { get; set; }
        public double MaxVolume { get; set; }
        public double MinVolume { get; set; }
    }
}