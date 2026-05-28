
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

using TCC_Inventory_Masters_Kinect.Command;
using TCC_Inventory_Masters_Kinect.Logs;
using TCC_Inventory_Masters_Kinect.Model;
using TCC_Inventory_Masters_Kinect.Repository;
using TCC_Inventory_Masters_Kinect.Repository.Interface;
using TCC_Inventory_Masters_Kinect.Service;

namespace TCC_Inventory_Masters_Kinect.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        // ==========================================
        // CAMPOS
        // ==========================================

        private readonly KinectService _kinectService;

        private readonly IKinectRepository _repository;

        private readonly SignalRService _signalRService;

        // Controla quando será permitida a próxima gravação no SQLite.
        private DateTime _proximaGravacao =
            DateTime.MinValue;

        // Controla quando será permitido o próximo envio ao MVC via SignalR.
        private DateTime _proximoEnvioSignalR =
            DateTime.MinValue;

        // Indica se está conectado ao SignalR.
        private bool _signalRConectado = false;

        // ==========================================
        // STATUS
        // ==========================================

        private string _status;

        public string Status
        {
            get => _status;

            set
            {
                _status = value;
                OnPropertyChanged();
            }
        }

        // ==========================================
        // VOLUME
        // ==========================================

        private string _volumeTexto;

        public string VolumeTexto
        {
            get => _volumeTexto;

            set
            {
                _volumeTexto = value;
                OnPropertyChanged();
            }
        }

        // ==========================================
        // CAMERA
        // ==========================================

        private ImageSource _cameraImage;

        public ImageSource CameraImage
        {
            get => _cameraImage;

            set
            {
                _cameraImage = value;
                OnPropertyChanged();
            }
        }

        // ==========================================
        // COMANDOS
        // ==========================================

        public ICommand LigarKinectCommand
        {
            get;
        }

        public ICommand DesligarKinectCommand
        {
            get;
        }

        public ICommand CalibrarCommand
        {
            get;
        }

        // ==========================================
        // CONSTRUTOR
        // ==========================================

        public MainViewModel()
        {
            try
            {
                LoggerService.Info(
                    "Inicializando MainViewModel.");

                _kinectService =
                    new KinectService();

                _repository =
                    new KinectRepository();

                _signalRService =
                    new SignalRService();

                LoggerService.Info(
                    "*******************      Serviços inicializados.**********************");

                // EVENTOS SIGNALR

                _signalRService.StatusSignalRAtualizado +=
                    AtualizarStatus;

                // EVENTOS KINECT

                _kinectService.MedidaAtualizada +=
                    ProcessarNovaMedida;

                _kinectService.StatusAtualizado +=
                    AtualizarStatus;

                _kinectService.CameraAtualizada +=
                    AtualizarCamera;

                // TEXTO INICIAL

                Status =
                    "Sistema aguardando inicialização...";

                VolumeTexto =
                    "Volume: 0 cm³";

                // COMANDOS

                LigarKinectCommand =
                    new RelayCommand(LigarKinect);

                DesligarKinectCommand =
                    new RelayCommand(DesligarKinect);

                CalibrarCommand =
                    new RelayCommand(CalibrarChao);

                LoggerService.Info(
                    "Comandos configurados.");

                // CONECTAR SIGNALR

                ConectarSignalR();
            }
            catch (Exception ex)
            {
                LoggerService.Erro(
                    "Erro ao inicializar MainViewModel.",
                    ex);
            }
        }

        // ==========================================
        // SIGNALR
        // ==========================================

        private async void ConectarSignalR()
        {
            try
            {
                LoggerService.Info(
                    "Tentando conectar ao SignalR.");

                await _signalRService.ConectarAsync();

                _signalRConectado = true;

                Status =
                    "Conectado ao MVC via SignalR.";

                LoggerService.Info(
                    "Conectado ao SignalR com sucesso.");

                await _signalRService.EnviarStatusAsync(
                    "Aplicação Kinect conectada ao MVC.");
            }
            catch (Exception ex)
            {
                _signalRConectado = false;

                Status =
                    "Erro ao conectar SignalR: " + ex.Message;

                LoggerService.Erro(
                    "Erro ao conectar SignalR.",
                    ex);
            }
        }

        // ==========================================
        // LIGAR KINECT
        // ==========================================

        private async void LigarKinect()
        {
            try
            {
                LoggerService.Info(
                    "Inicializando Kinect.");

                Status =
                    "Inicializando Kinect...";

                bool sucesso =
                    _kinectService
                        .InicializarKinect();

                if (sucesso)
                {
                    Status =
                        "Kinect iniciado.";

                    LoggerService.Info(
                        "Kinect iniciado com sucesso.");

                    if (_signalRConectado)
                    {
                        await _signalRService.EnviarStatusAsync(
                            "Kinect iniciado.");
                    }
                }
                else
                {
                    Status =
                        "Falha ao iniciar Kinect.";

                    LoggerService.Info(
                        "Falha ao iniciar Kinect.");

                    if (_signalRConectado)
                    {
                        await _signalRService.EnviarStatusAsync(
                            "Falha ao iniciar Kinect.");
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerService.Erro(
                    "Erro ao iniciar Kinect.",
                    ex);

                Status =
                    ex.Message;
            }
        }

        // ==========================================
        // DESLIGAR KINECT
        // ==========================================

        private async void DesligarKinect()
        {
            try
            {
                LoggerService.Info(
                    "Desligando Kinect.");

                _kinectService.DesligarKinect();

                Status =
                    "Kinect desligado.";

                VolumeTexto =
                    "Volume: 0 cm³";

                CameraImage = null;

                if (_signalRConectado)
                {
                    await _signalRService.EnviarStatusAsync(
                        "Kinect desligado.");

                    await _signalRService.DesconectarAsync();

                    _signalRConectado = false;

                    LoggerService.Info(
                        "SignalR desconectado.");

                    Status =
                        "Kinect desligado e conexão com MVC encerrada.";
                }
            }
            catch (Exception ex)
            {
                LoggerService.Erro(
                    "Erro ao desligar Kinect.",
                    ex);
            }
        }

        // ==========================================
        // CALIBRAR CHÃO
        // ==========================================

        private async void CalibrarChao()
        {
            try
            {
                LoggerService.Info(
                    "Iniciando calibração do chão.");

                _kinectService.CalibrarChao();

                Status =
                    "Chão calibrado.";

                LoggerService.Info(
                    "Chão calibrado com sucesso.");

                if (_signalRConectado)
                {
                    await _signalRService.EnviarStatusAsync(
                        "Chão calibrado.");
                }
            }
            catch (Exception ex)
            {
                LoggerService.Erro(
                    "Erro ao calibrar chão.",
                    ex);
            }
        }

        // ==========================================
        // STATUS
        // ==========================================

        private void AtualizarStatus(
            string msg)
        {
            Status = msg;

            LoggerService.Info(
                "Status atualizado: " + msg);
        }

        // ==========================================
        // CAMERA
        // ==========================================

        private void AtualizarCamera(
            ImageSource imagem)
        {
            CameraImage = imagem;
        }

        // ==========================================
        // MEDIÇÃO
        // ==========================================

        private void ProcessarNovaMedida(
            double volumeCm3)
        {
            VolumeTexto =
                $"Volume: {volumeCm3:F0} cm³";

            // ==========================================
            // SALVAR SQLITE
            // ==========================================

            if (DateTime.Now >= _proximaGravacao)
            {
                _proximaGravacao =
                    DateTime.Now.AddSeconds(1);

                var medicao =
                    new MedicaoVolume
                    {
                        DataHora =
                            DateTime.Now,

                        VolumeCm3 =
                            volumeCm3,

                        KinectLigado =
                            true,

                        Calibrado =
                            true,

                        Status =
                            "AutoSave"
                    };

                Task.Run(() =>
                {
                    try
                    {
                        _repository
                            .SalvarMedicao(
                                medicao);

                        LoggerService.Info(
                            $"Medição salva no SQLite. Volume: {volumeCm3:F0} cm³");
                    }
                    catch (Exception ex)
                    {
                        LoggerService.Erro(
                            "Erro ao salvar medição no SQLite.",
                            ex);
                    }
                });
            }

            // ==========================================
            // ENVIAR MVC VIA SIGNALR
            // ==========================================

            if (_signalRConectado &&
                DateTime.Now >= _proximoEnvioSignalR)
            {
                _proximoEnvioSignalR =
                    DateTime.Now.AddSeconds(15);

                Task.Run(async () =>
                {
                    try
                    {
                        await _signalRService
                            .EnviarVolumeAsync(
                                volumeCm3);

                        await _signalRService
                            .EnviarStatusAsync(
                                "Volume enviado pelo Kinect.");

                        LoggerService.Info(
                            $"Volume enviado ao MVC: {volumeCm3:F0} cm³");

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            Status =
                                $"Informações enviadas ao MVC com sucesso às {DateTime.Now:HH:mm:ss}.";
                        });
                    }
                    catch (Exception ex)
                    {
                        LoggerService.Erro(
                            "Erro ao enviar dados ao MVC.",
                            ex);

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            Status =
                                "Falha ao enviar informações ao MVC. Dados mantidos no SQLite. Erro: " + ex.Message;
                        });
                    }
                });
            }
        }
    }
}

