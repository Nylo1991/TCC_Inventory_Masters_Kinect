namespace MVC_InventoryMasters.Models
{
    public static class PermissoesSistema
    {
        public const string DashboardVisualizar = "Dashboard.Visualizar";
        public const string MedicoesVisualizar = "Medicoes.Visualizar";
        public const string NotificacoesVisualizar = "Notificacoes.Visualizar";
        public const string ParceirosVisualizar = "Parceiros.Visualizar";
        public const string ParceirosGerenciar = "Parceiros.Gerenciar";
        public const string UsuariosGerenciar = "Usuarios.Gerenciar";
        public const string PerfisGerenciar = "Perfis.Gerenciar";
        public const string ConfiguracoesGerenciar = "Configuracoes.Gerenciar";
        public const string KinectAcessar = "Kinect.Acessar";
        public const string LogsVisualizar = "Logs.Visualizar";

        // Lista centralizada para evitar permissões digitadas de formas diferentes nas próximas telas.
        public static readonly string[] Todas =
        {
            DashboardVisualizar,
            MedicoesVisualizar,
            NotificacoesVisualizar,
            ParceirosVisualizar,
            ParceirosGerenciar,
            UsuariosGerenciar,
            PerfisGerenciar,
            ConfiguracoesGerenciar,
            KinectAcessar,
            LogsVisualizar
        };
    }
}
