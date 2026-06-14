using Google.Cloud.Firestore;
using MVC_InventoryMasters.Models;
using MVC_InventoryMasters.Services;

namespace MVC_InventoryMasters.Repositories
{
    public class MedicaoVolumeRepository
    {
        private readonly string _colecao = "Medicoes";
        private readonly FirestoreDb _db;

        public MedicaoVolumeRepository(FirebaseService firebaseService)
        {
            _db = firebaseService.Firestore;
        }

        public async Task Adicionar(MedicaoVolume medicao)
        {
            medicao.DataHora = DateTime.UtcNow;

            await _db
                .Collection(_colecao)
                .AddAsync(medicao);
        }

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

    public class MedicaoSummary
    {
        public int TotalMedicoes { get; set; }

        public double MediaVolume { get; set; }

        public double MaxVolume { get; set; }

        public double MinVolume { get; set; }
    }
}