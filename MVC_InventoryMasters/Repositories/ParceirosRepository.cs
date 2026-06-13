using Google.Cloud.Firestore;
using MVC_InventoryMasters.Models;
using MVC_InventoryMasters.Services;

namespace MVC_InventoryMasters.Repositories
{
    public class ParceirosRepository
    {
        private readonly string _colecao = "Parceiros";
        private readonly FirestoreDb _db;

        public ParceirosRepository(FirebaseService firebaseService)
        {
            _db = firebaseService.Firestore;
        }

        public async Task<List<Parceiro>> ListarTodos()
        {
            List<Parceiro> lista = new();
            QuerySnapshot documentos = await _db.Collection(_colecao).GetSnapshotAsync();
            foreach (DocumentSnapshot doc in documentos.Documents)
            {
                Parceiro parceiro = doc.ConvertTo<Parceiro>();
                parceiro.Id = doc.Id;
                lista.Add(parceiro);
            }
            return lista;
        }

        public async Task<Parceiro?> BuscarPorId(string id)
        {
            DocumentSnapshot documento = await _db.Collection(_colecao).Document(id).GetSnapshotAsync();
            if (!documento.Exists) return null;
            Parceiro parceiro = documento.ConvertTo<Parceiro>();
            parceiro.Id = documento.Id;
            return parceiro;
        }

        public async Task<List<Parceiro>> Pesquisar(string termo)
        {
            var parceiros = await ListarTodos();
            if (string.IsNullOrWhiteSpace(termo)) return parceiros;
            termo = termo.ToLower();
            return parceiros.Where(p =>
                (p.Id ?? "").ToLower().Contains(termo) ||
                (p.Nome ?? "").ToLower().Contains(termo) ||
                (p.Email ?? "").ToLower().Contains(termo) ||
                (p.Empresa ?? "").ToLower().Contains(termo) ||
                (p.Telefone ?? "").ToLower().Contains(termo))
            .ToList();
        }

        // --- NOVO MÉTODO DE FILTROS AVANÇADOS ---
        public async Task<List<Parceiro>> FiltrarAvancado(string termo, DateTime? dataInicio, DateTime? dataFim, bool? ativo)
        {
            var lista = await ListarTodos();

            // 1. Filtro Geral
            if (!string.IsNullOrWhiteSpace(termo))
            {
                var t = termo.ToLower();
                lista = lista.Where(p =>
                    (p.Id ?? "").ToLower().Contains(t) ||
                    (p.Nome ?? "").ToLower().Contains(t) ||
                    (p.Email ?? "").ToLower().Contains(t) ||
                    (p.Empresa ?? "").ToLower().Contains(t) ||
                    (p.Telefone ?? "").ToLower().Contains(t)
                ).ToList();
            }

            // 2. Filtro de Data Início
            if (dataInicio.HasValue)
                lista = lista.Where(p => p.Data_Cadastro.Date >= dataInicio.Value.Date).ToList();

            // 3. Filtro de Data Fim
            if (dataFim.HasValue)
                lista = lista.Where(p => p.Data_Cadastro.Date <= dataFim.Value.Date).ToList();

            // 4. Filtro de Status
            if (ativo.HasValue)
                lista = lista.Where(p => p.Ativo == ativo.Value).ToList();

            return lista;
        }

        public async Task Adicionar(Parceiro parceiro)
        {
            var dados = new Dictionary<string, object>
            {
                { "Nome", parceiro.Nome ?? string.Empty },
                { "Email", parceiro.Email ?? string.Empty },
                { "Telefone", parceiro.Telefone ?? string.Empty },
                { "Empresa", parceiro.Empresa ?? string.Empty },
                { "Endereco", parceiro.Endereco ?? string.Empty },
                { "Data_Cadastro", DateTime.UtcNow },
                { "Ativo", parceiro.Ativo }
            };
            await _db.Collection(_colecao).AddAsync(dados);
        }

        public async Task Atualizar(Parceiro parceiro)
        {
            await _db.Collection(_colecao).Document(parceiro.Id).UpdateAsync(new Dictionary<string, object>
            {
                { "Nome", parceiro.Nome ?? string.Empty },
                { "Email", parceiro.Email ?? string.Empty },
                { "Telefone", parceiro.Telefone ?? string.Empty },
                { "Empresa", parceiro.Empresa ?? string.Empty },
                { "Endereco", parceiro.Endereco ?? string.Empty },
                { "Ativo", parceiro.Ativo }
            });
        }

        public async Task Excluir(string id)
        {
            await _db.Collection(_colecao).Document(id).DeleteAsync();
        }
    }
}