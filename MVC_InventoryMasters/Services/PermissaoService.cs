using MVC_InventoryMasters.Models;

namespace MVC_InventoryMasters.Services
{
    public class PermissaoService
    {
        public bool PerfilPossuiPermissao(string? nomePerfil, string permissao)
        {
            var permissoes = ObterPermissoesPadrao(nomePerfil);
            return permissoes.Contains(permissao, StringComparer.OrdinalIgnoreCase);
        }

        public IReadOnlyCollection<string> ObterPermissoesPadrao(string? nomePerfil)
        {
            return (nomePerfil ?? string.Empty).Trim().ToLowerInvariant() switch
            {
                "administrador" => PermissoesSistema.Todas,
                "gestor" => new[]
                {
                    PermissoesSistema.DashboardVisualizar,
                    PermissoesSistema.MedicoesVisualizar,
                    PermissoesSistema.NotificacoesVisualizar,
                    PermissoesSistema.ParceirosVisualizar
                },
                "operador" => new[]
                {
                    PermissoesSistema.DashboardVisualizar,
                    PermissoesSistema.MedicoesVisualizar,
                    PermissoesSistema.KinectAcessar
                },
                "visualizador" => new[]
                {
                    PermissoesSistema.DashboardVisualizar,
                    PermissoesSistema.MedicoesVisualizar,
                    PermissoesSistema.NotificacoesVisualizar
                },
                _ => Array.Empty<string>()
            };
        }
    }
}
