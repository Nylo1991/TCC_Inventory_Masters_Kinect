namespace TCC_Inventory_Masters_Kinect.ConfigKinect
{
    public static class KinectConfig
    {
        // ==========================================
        // HISTÓRICO
        // ==========================================

        public const int MaxHistoricoVolume = 30;

        // ==========================================
        // ALTURA MÍNIMA
        // ==========================================

        public const int LimiteMinimoAlturaMm = 30;

        // ==========================================
        // CAMPO DE VISÃO DO KINECT
        // ==========================================

        public const double HorizontalFovGraus = 57.0;

        public const double VerticalFovGraus = 43.0;

        // ==========================================
        // SIGNALR
        // ==========================================

        public const string UrlSignalR =
            "https://inventorymasters.runasp.net/medicaohub";

        public const int IntervaloEnvioSignalRSegundos = 15;

        // ==========================================
        // MAPEAMENTO ESPACIAL
        // ==========================================

        public const int DistanciaMaximaMm = 4000;

        public const int DistanciaMinimaMm = 500;

        // ==========================================
        // POINT CLOUD
        // ==========================================

        public const int MaxPontos3D = 100000;

        // ==========================================
        // SNAPSHOT
        // ==========================================

        public const int IntervaloSnapshotSegundos = 60;

        // ==========================================
        // MONITORAMENTO
        // ==========================================

        public const int IntervaloAtualizacaoVolumeMs = 500;

        // ==========================================
        // OCUPAÇÃO
        // ==========================================

        public const double LimiteAlertaOcupacao = 80.0;

        public const double LimiteCriticoOcupacao = 95.0;

        // ==========================================
        // MAPEAMENTO
        // ==========================================

        public const int TempoMaximoMapeamentoSegundos = 30;

        // ==========================================
        // SQLITE
        // ==========================================

        public const int IntervaloSalvarSQLiteSegundos = 1;
    }
}