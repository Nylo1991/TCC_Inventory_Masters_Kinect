namespace TCC_Inventory_Masters_Kinect.ConfigKinect
{
    /// <summary>
    /// Classe responsável por centralizar os parâmetros de configuração do Kinect
    /// e da aplicação Inventory Masters.
    /// Evita valores fixos espalhados pelo código e facilita futuras alterações.
    /// </summary>
    public static class KinectConfig
    {
        /// <summary>
        /// Quantidade máxima de leituras mantidas no histórico para suavização do volume.
        /// </summary>
        public const int MaxHistoricoVolume = 30;

        /// <summary>
        /// Altura mínima, em milímetros, para considerar que houve presença real de objeto.
        /// Ajuda a ignorar ruídos do sensor.
        /// </summary>
        public const int LimiteMinimoAlturaMm = 30;

        /// <summary>
        /// Campo de visão horizontal aproximado do Kinect v1.
        /// </summary>
        public const double HorizontalFovGraus = 57.0;

        /// <summary>
        /// Campo de visão vertical aproximado do Kinect v1.
        /// </summary>
        public const double VerticalFovGraus = 43.0;

        /// <summary>
        /// URL do Hub SignalR responsável por receber as medições volumétricas.
        /// </summary>
        public const string UrlSignalR =
            "http://inventorymasters.runasp.net/medicaoHub";

        /// <summary>
        ///  conexão com MVC para validação do token 
        /// </summary>
        public const string UrlValidarTokenMvc =
       "http://inventorymasters.runasp.net/api/kinect/validar-token";

        /// <summary>
        /// Intervalo, em segundos, para envio periódico do volume ao sistema web.
        /// </summary>
        public const int IntervaloEnvioSignalRSegundos = 15;

        /// <summary>
        /// Distância mínima confiável para leitura de profundidade do Kinect, em milímetros.
        /// </summary>
        public const int DistanciaMinimaMm = 1200;

        /// <summary>
        /// Distância máxima confiável para leitura de profundidade do Kinect, em milímetros.
        /// </summary>
        public const int DistanciaMaximaMm = 3500;

        /// <summary>
        /// Intervalo, em segundos, para captura automática de snapshot.
        /// </summary>
        public const int IntervaloSnapshotSegundos = 60;

        /// <summary>
        /// Intervalo, em milissegundos, para atualização visual do volume na interface.
        /// </summary>
        public const int IntervaloAtualizacaoVolumeMs = 500;

        /// <summary>
        /// Percentual de ocupação considerado como nível de alerta.
        /// </summary>
        public const double LimiteAlertaOcupacao = 80.0;

        /// <summary>
        /// Percentual de ocupação considerado como nível crítico.
        /// </summary>
        public const double LimiteCriticoOcupacao = 95.0;

        /// <summary>
        /// Tempo máximo permitido para mapeamento/calibração do ambiente.
        /// </summary>
        public const int TempoMaximoMapeamentoSegundos = 30;

        /// <summary>
        /// Intervalo, em segundos, para salvar medições no SQLite.
        /// </summary>
        public const int IntervaloSalvarSQLiteSegundos = 15;
    }
}