<<<<<<< HEAD
using MVC_InventoryMasters.Models;
=======
﻿using MVC_InventoryMasters.Models;
>>>>>>> 69278f70785abed625eb15930bd6564a7fd280ec

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
