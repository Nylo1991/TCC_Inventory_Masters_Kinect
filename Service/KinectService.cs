using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using Microsoft.Kinect;
using TCC_Inventory_Masters_Kinect.Logs;

namespace TCC_Inventory_Masters_Kinect.Service
{
    /// <summary>
    /// Serviço responsável por controlar o Kinect v1.
    /// Este arquivo mantém o ciclo de vida principal do sensor: iniciar, parar e armazenar estados compartilhados.
    /// </summary>
    public partial class KinectService
    {
        private KinectSensor _sensor;
        private short[] _depthCalibrado;
        private int _larguraDepth;
        private int _alturaDepth;

        private bool _calibrado = false;
        private double _ultimoVolumeSuavizado = 0;

        private readonly Queue<double> _historicoVolumes = new Queue<double>();

        private const int MAX_HISTORICO_VOLUME = 10;

        private const int ANGULO_MIN = -27;
        private const int ANGULO_MAX = 27;
        private const int PASSO_ANGULO = 5;
        private const int FRAMES_POR_ANGULO = 10;
        private const int ESPERA_MOTOR_MS = 1500;

        private const int DEPTH_MIN_MM = 1200;
        private const int DEPTH_MAX_MM = 3500;
        private const int ALTURA_MINIMA_OBJETO_MM = 30;
        private const int ALTURA_MAXIMA_OBJETO_MM = 1800;
        private const int PONTOS_MINIMOS_VOLUME = 1000;

        private const double FOV_HORIZONTAL_GRAUS = 57.0;
        private const double FOV_VERTICAL_GRAUS = 43.0;
        private const double PESO_SUAVIZACAO = 0.7;

        /// <summary>
        /// Evento disparado sempre que um novo frame da câmera RGB é capturado.
        /// </summary>
        public event Action<BitmapSource> CameraFrameAtualizado;

        /// <summary>
        /// Indica se o Kinect está conectado e disponível para uso.
        /// </summary>
        public bool IsConnected =>
            _sensor != null && _sensor.Status == KinectStatus.Connected;

        /// <summary>
        /// Inicializa o serviço tentando localizar o primeiro Kinect conectado ao computador.
        /// </summary>
        public KinectService()
        {
            if (KinectSensor.KinectSensors.Count > 0)
            {
                _sensor = KinectSensor.KinectSensors[0];
            }
        }

        /// <summary>
        /// Inicia o Kinect, habilitando câmera RGB e profundidade.
        /// </summary>
        public void Start()
        {
            try
            {
                if (_sensor == null)
                {
                    LoggerService.Erro("Nenhum Kinect foi encontrado no computador.");
                    throw new InvalidOperationException("Nenhum Kinect foi encontrado no computador.");
                }

                if (_sensor.Status != KinectStatus.Connected)
                {
                    LoggerService.Erro("Kinect v1 nao esta conectado.");
                    throw new InvalidOperationException("Kinect v1 nao esta conectado.");
                }

                _sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
                _sensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);

                _sensor.ColorFrameReady -= Sensor_ColorFrameReady;
                _sensor.ColorFrameReady += Sensor_ColorFrameReady;

                _sensor.Start();

                LoggerService.Info("Kinect iniciado com camera RGB e profundidade.");
            }
            catch
            {
                LoggerService.Erro("Erro ao iniciar Kinect com camera RGB e profundidade.");
                throw;
            }
        }

        /// <summary>
        /// Finaliza o Kinect e remove eventos ativos.
        /// </summary>
        public void Stop()
        {
            try
            {
                if (_sensor != null)
                {
                    _sensor.ColorFrameReady -= Sensor_ColorFrameReady;

                    if (_sensor.IsRunning)
                    {
                        _sensor.Stop();
                    }

                    LoggerService.Info("Kinect finalizado.");
                }
            }
            catch
            {
                LoggerService.Erro("Erro ao finalizar Kinect.");
            }
        }
    }
}