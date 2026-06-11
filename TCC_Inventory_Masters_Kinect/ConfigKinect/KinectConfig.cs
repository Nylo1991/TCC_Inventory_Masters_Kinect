namespace TCC_Inventory_Masters_Kinect.ConfigKinect
{
    public static class KinectConfig
    {
       

        public const int MaxHistoricoVolume = 30;

        public const int LimiteMinimoAlturaMm = 30;

        public const double HorizontalFovGraus = 57.0;

        public const double VerticalFovGraus = 43.0;

        public const string UrlSignalR =
            "http://inventorymasters.runasp.net/medicaoHub";

        public const int IntervaloEnvioSignalRSegundos = 15;

        public const int DistanciaMaximaMm = 4000;

        public const int DistanciaMinimaMm = 500;

        public const int IntervaloSnapshotSegundos = 60;

        public const int IntervaloAtualizacaoVolumeMs = 500;

        public const double LimiteAlertaOcupacao = 80.0;

        public const double LimiteCriticoOcupacao = 95.0;

        public const int TempoMaximoMapeamentoSegundos = 30;

        public const int IntervaloSalvarSQLiteSegundos = 1;
    }
}