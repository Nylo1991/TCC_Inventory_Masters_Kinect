using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using TCC_Inventory_Masters_Kinect.Command;
using TCC_Inventory_Masters_Kinect.Service;

namespace TCC_Inventory_Masters_Kinect.ViewModel
{
    public class KinectMonitorWindowViewModel : BaseViewModel
    {
        private readonly KinectService _kinectService;
        private readonly SignalRService _signalRService;

        private DispatcherTimer _envioVolumeTimer; 
        private double _ultimoVolumeMaximo = 0;
        private DispatcherTimer _frameTimer;

        // ─── PROPRIEDADES DE IMAGEM ─────────────────────────────────────
        private BitmapSource _cameraImage;
        public BitmapSource CameraImage
        {
            get => _cameraImage;
            set => SetProperty(ref _cameraImage, value);
        }

        private BitmapSource _depthImage;
        public BitmapSource DepthImage
        {
            get => _depthImage;
            set => SetProperty(ref _depthImage, value);
        }

        // ─── STATUS DINÂMICOS (nenhum texto pré-definido) ───────────────
        private string _statusKinect;
        public string StatusKinect
        {
            get => _statusKinect;
            set => SetProperty(ref _statusKinect, value);
        }

        private string _statusSQLite;
        public string StatusSQLite
        {
            get => _statusSQLite;
            set => SetProperty(ref _statusSQLite, value);
        }

        private string _statusSignalR;
        public string StatusSignalR
        {
            get => _statusSignalR;
            set => SetProperty(ref _statusSignalR, value);
        }

        private string _statusMvcFirebase;
        public string StatusMvcFirebase
        {
            get => _statusMvcFirebase;
            set => SetProperty(ref _statusMvcFirebase, value);
        }

        private string _status;
        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        private string _mensagemEnvioAplicacao;
        public string MensagemEnvioAplicacao
        {
            get => _mensagemEnvioAplicacao;
            set => SetProperty(ref _mensagemEnvioAplicacao, value);
        }

        // ─── PROPRIEDADES DE MEDIÇÃO DINÂMICAS ─────────────────────────
        private string _volumeTexto;
        public string VolumeTexto
        {
            get => _volumeTexto;
            set => SetProperty(ref _volumeTexto, value);
        }

        private string _percentualOcupacaoTexto;
        public string PercentualOcupacaoTexto
        {
            get => _percentualOcupacaoTexto;
            set => SetProperty(ref _percentualOcupacaoTexto, value);
        }

        private string _espacoLivreTexto;
        public string EspacoLivreTexto
        {
            get => _espacoLivreTexto;
            set => SetProperty(ref _espacoLivreTexto, value);
        }

        private string _quantidadePontos3D;
        public string QuantidadePontos3D
        {
            get => _quantidadePontos3D;
            set => SetProperty(ref _quantidadePontos3D, value);
        }

        private string _ultimoSnapshot;
        public string UltimoSnapshot
        {
            get => _ultimoSnapshot;
            set => SetProperty(ref _ultimoSnapshot, value);
        }

        // ─── PROPRIEDADES DE CONFIGURAÇÃO DINÂMICAS ────────────────────
        private string _nomeEspaco;
        public string NomeEspaco
        {
            get => _nomeEspaco;
            set => SetProperty(ref _nomeEspaco, value);
        }

        private string _percentualAlerta;
        public string PercentualAlerta
        {
            get => _percentualAlerta;
            set => SetProperty(ref _percentualAlerta, value);
        }

        private string _volumeMaximo;
        public string VolumeMaximo
        {
            get => _volumeMaximo;
            set => SetProperty(ref _volumeMaximo, value);
        }

        // ─── COMANDOS ───────────────────────────────────────────────────
        public ICommand LigarKinectCommand { get; }
        public ICommand DesligarKinectCommand { get; }
        public ICommand CalibrarCommand { get; }
        public ICommand CalibrarEspacoCommand { get; }

        // ─── CONSTRUTOR ─────────────────────────────────────────────────
        public KinectMonitorWindowViewModel()
        {
            _kinectService = new KinectService();
            _signalRService = new SignalRService();

            LigarKinectCommand = new RelayCommand(LigarKinect);
            DesligarKinectCommand = new RelayCommand(DesligarKinect);
            CalibrarCommand = new RelayCommand(CalibrarChaoAsync);
            CalibrarEspacoCommand = new RelayCommand(CalibrarEspacoAsync);
        }

        // ─── LIGAR KINECT ───────────────────────────────────────────────
        private async void LigarKinect()
        {
            try
            {
                _kinectService.Start();
                StatusKinect = "Kinect conectado";
                Status = "Kinect iniciado com sucesso";

                // ==================== CONEXÃO SIGNALR ====================
                StatusSignalR = "SignalR: Conectando...";
                await _signalRService.ConectarAsync();

                if (_signalRService.EstaConectado)
                    StatusSignalR = "SignalR: Conectado";
                else
                    StatusSignalR = "SignalR: Falha na conexão";
                // =========================================================

                IniciarTimerFrames();
            }
            catch (Exception ex)
            {
                StatusKinect = "Kinect: erro ao conectar";
                Status = $"Erro ao iniciar Kinect: {ex.Message}";
            }
        }


        private void IniciarEnvioPeriodicoDeVolume()
        {
            _envioVolumeTimer?.Stop();

            _envioVolumeTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(15)
            };

            _envioVolumeTimer.Tick += async (s, e) =>
            {
                if (_signalRService.EstaConectado && _ultimoVolumeMaximo > 0)
                {
                    // Envia silenciosamente (sem atualizar a interface)
                    await _signalRService.EnviarVolumeAsync(_ultimoVolumeMaximo);

                    // REMOVIDO: não atualiza mais MensagemEnvioAplicacao aqui
                }
            };

            _envioVolumeTimer.Start();
        }



        // ─── TIMER DE FRAMES ────────────────────────────────────────────
        private void IniciarTimerFrames()
        {
            _frameTimer?.Stop();

            _frameTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(33)
            };

            _frameTimer.Tick += (s, e) =>
            {
                var cameraFrame = _kinectService.CapturarFrameCamera();
                if (cameraFrame != null)
                    CameraImage = cameraFrame;

                var depthFrame = _kinectService.CapturarDepthColorido();
                if (depthFrame != null)
                    DepthImage = depthFrame;
            };

            _frameTimer.Start();
        }

        // ─── DESLIGAR KINECT ────────────────────────────────────────────
        private void DesligarKinect()
        {
            _frameTimer?.Stop();
            _frameTimer = null;

            _kinectService.Stop();

            CameraImage = null;
            DepthImage = null;

            StatusKinect = "Kinect desligado";
            Status = "Kinect encerrado pelo usuario";

            _envioVolumeTimer?.Stop();     // ← Adicione
            _envioVolumeTimer = null;      // ← Adicione

        }

        // ─── CALIBRAÇÃO DE CHÃO ─────────────────────────────────────────
        private async Task CalibrarChaoAsync()
        {
            try
            {
                Status = "Calibrando chao, aguarde...";

                var resultado = await _kinectService.CalibrateAsync(CancellationToken.None);

                QuantidadePontos3D = resultado.TotalPointsFound.ToString();
                Status = $"Chao calibrado com sucesso. Pontos detectados: {resultado.TotalPointsFound}";

                MessageBox.Show(
                    $"Calibracao do chao concluida!\nPontos detectados: {resultado.TotalPointsFound}",
                    "Calibracao concluida");
            }
            catch (Exception ex)
            {
                Status = $"Erro na calibracao do chao: {ex.Message}";
                MessageBox.Show($"Falha na calibracao: {ex.Message}", "Erro");
            }
        }

        // ─── CALIBRAÇÃO DE ESPAÇO ───────────────────────────────────────
        private async Task CalibrarEspacoAsync()
        {
            try
            {
                Status = "Calibrando espaço, aguarde...";

                var resultado = await _kinectService.CalibrateAsync(CancellationToken.None);

                VolumeMaximo = resultado.MaxVolume.ToString("F0");
                VolumeTexto = $"{resultado.MaxVolume:F0} cm³";
                _ultimoVolumeMaximo = resultado.MaxVolume;   // Salva o valor para envio periódico

                Status = $"Espaço calibrado. Volume máximo: {resultado.MaxVolume:F0} cm³";

                // Envia imediatamente na primeira calibração
                if (_signalRService.EstaConectado)
                {
                    await _signalRService.EnviarVolumeAsync(resultado.MaxVolume);
                    MensagemEnvioAplicacao = $"Volume enviado com sucesso: {resultado.MaxVolume:F0} cm³";
                }

                else
                {
                    MensagemEnvioAplicacao = "SignalR não está conectado.";
                }
              

                MessageBox.Show(
                    $"Calibração concluída!\nVolume máximo: {resultado.MaxVolume:F0} cm³",
                    "Calibração concluída");
            }
            catch (Exception ex)
            {
                Status = $"Erro: {ex.Message}";
                MensagemEnvioAplicacao = $"Erro: {ex.Message}";
                MessageBox.Show($"Falha na calibração: {ex.Message}", "Erro");
            }
        }


    }
}
