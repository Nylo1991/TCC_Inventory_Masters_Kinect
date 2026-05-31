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
        // API MVC
        // ==========================================

        public const string UrlApiMedicoes =
            "http://inventorymasters.runasp.net/api/medicoes";

        // ==========================================
        // MAPEAMENTO ESPACIAL
        // ==========================================

        // Distância máxima de leitura do Kinect
        public const int DistanciaMaximaMm = 4000;

        // Distância mínima de leitura
        public const int DistanciaMinimaMm = 500;

        // ==========================================
        // POINT CLOUD
        // ==========================================

        // Quantidade máxima de pontos 3D
        public const int MaxPontos3D = 100000;

        // ==========================================
        // SNAPSHOT
        // ==========================================

        // Intervalo para snapshots automáticos
        public const int IntervaloSnapshotSegundos = 60;

        // ==========================================
        // MONITORAMENTO
        // ==========================================

        // Intervalo de atualização do volume
        public const int IntervaloAtualizacaoVolumeMs = 500;

        // Intervalo de envio para API MVC
        public const int IntervaloEnvioApiSegundos = 15;

        // ==========================================
        // OCUPAÇÃO
        // ==========================================

        // Alerta de ocupação
        public const double LimiteAlertaOcupacao = 80.0;

        // Limite crítico
        public const double LimiteCriticoOcupacao = 95.0;

        // ==========================================
        // MAPEAMENTO
        // ==========================================

        // Tempo máximo de escaneamento
        public const int TempoMaximoMapeamentoSegundos = 30;

        // ==========================================
        // SQLITE
        // ==========================================

        // Intervalo de gravação no banco
        public const int IntervaloSalvarSQLiteSegundos = 1;
    }
}