using Google.Cloud.Firestore;
using Microsoft.Extensions.Logging;
using MVC_InventoryMasters.Models;
using MVC_InventoryMasters.Services;

namespace MVC_InventoryMasters.Repositories
{
    /// <summary>
    /// Repositório responsável por perfis e permissões por empresa.
    /// </summary>
    public class PerfisRepository
    {
        private readonly FirestoreDb _firestore;
        private readonly ILogger<PerfisRepository> _logger;
        private readonly ContextoUsuarioService _contextoUsuario;
        private readonly PermissaoService _permissaoService;
        private readonly string _colecao = "PerfilUsuario";

        public PerfisRepository(
            FirebaseService firebaseService,
            ILogger<PerfisRepository> logger,
            ContextoUsuarioService contextoUsuario,
            PermissaoService permissaoService)
        {
            _firestore = firebaseService.Firestore;
            _logger = logger;
            _contextoUsuario = contextoUsuario;
            _permissaoService = permissaoService;
        }

        public async Task<List<Perfil>> ListarTodos()
        {
            try
            {
                var snapshot = await _firestore
                    .Collection(_colecao)
                    .GetSnapshotAsync();

                return snapshot.Documents.Select(doc =>
                {
                    var perfil = doc.ConvertTo<Perfil>();
                    perfil.Id = doc.Id;
                    return NormalizarPermissoesPadrao(perfil);
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar perfis.");
                return new List<Perfil>();
            }
        }

        public async Task<List<Perfil>> ListarPorEmpresa(string? empresaId = null)
        {
            string empresa = string.IsNullOrWhiteSpace(empresaId)
                ? _contextoUsuario.ObterEmpresaId()
                : empresaId;

            var perfis = await ListarTodos();

            return perfis
                .Where(p => p.EmpresaId == empresa ||
                            (empresa == ContextoUsuarioService.EmpresaPadraoId &&
                             string.IsNullOrWhiteSpace(p.EmpresaId)))
                .ToList();
        }

        public async Task<Perfil?> BuscarPorId(string id)
        {
            try
            {
                var doc = await _firestore
                    .Collection(_colecao)
                    .Document(id)
                    .GetSnapshotAsync();

                if (!doc.Exists)
                    return null;

                var perfil = doc.ConvertTo<Perfil>();
                perfil.Id = doc.Id;
                return NormalizarPermissoesPadrao(perfil);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar perfil {PerfilId}.", id);
                return null;
            }
        }

        public async Task Adicionar(Perfil perfil)
        {
            perfil.EmpresaId = string.IsNullOrWhiteSpace(perfil.EmpresaId)
                ? _contextoUsuario.ObterEmpresaId()
                : perfil.EmpresaId;

            perfil.Permissoes = NormalizarPermissoesPadrao(perfil).Permissoes;
            perfil.Data_Cadastro = DateTime.UtcNow;

            await _firestore.Collection(_colecao).AddAsync(perfil);
        }

        public async Task Atualizar(Perfil perfil)
        {
            perfil.EmpresaId = string.IsNullOrWhiteSpace(perfil.EmpresaId)
                ? _contextoUsuario.ObterEmpresaId()
                : perfil.EmpresaId;

            perfil.Permissoes = NormalizarPermissoesPadrao(perfil).Permissoes;

            await _firestore
                .Collection(_colecao)
                .Document(perfil.Id)
                .SetAsync(perfil, SetOptions.MergeAll);
        }

        public async Task Inativar(string id)
        {
            await _firestore
                .Collection(_colecao)
                .Document(id)
                .UpdateAsync("Ativo", false);
        }

        private Perfil NormalizarPermissoesPadrao(Perfil perfil)
        {
            if (perfil.Permissoes.Any())
                return perfil;

            perfil.Permissoes = _permissaoService
                .ObterPermissoesPadrao(perfil.Nome)
                .ToList();

            return perfil;
        }
    }
}
