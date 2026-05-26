using System;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

using TCC_Inventory_Masters_Kinect.Command;
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
        // Usado para evitar salvar muitas medições por segundo.
        private DateTime _proximaGravacao =
            DateTime.MinValue;

        // Controla quando será permitido o próximo envio ao MVC via SignalR.
        // Usado como um timer manual para enviar os dados a cada 15 segundos.
        private DateTime _proximoEnvioSignalR =
            DateTime.MinValue;

        // Indica se a aplicação WPF conseguiu se conectar ao Hub SignalR do MVC.
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
            _kinectService =
                new KinectService();

            _repository =
                new KinectRepository();

            _signalRService =
                new SignalRService();

            // Recebe mensagens do SignalR,
            // como reconectando, reconectado ou conexão encerrada.
            _signalRService.StatusSignalRAtualizado +=
                AtualizarStatus;

            // EVENTOS DO KINECT

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

            // COMANDOS DA TELA

            LigarKinectCommand =
                new RelayCommand(LigarKinect);

            DesligarKinectCommand =
                new RelayCommand(DesligarKinect);

            CalibrarCommand =
                new RelayCommand(CalibrarChao);

            // CONEXÃO COM O MVC VIA SIGNALR

            ConectarSignalR();
        }

        // ==========================================
        // SIGNALR
        // ==========================================

        private async void ConectarSignalR()
        {
            try
            {
                await _signalRService.ConectarAsync();

                _signalRConectado = true;

                Status =
                    "Conectado ao MVC via SignalR.";

                await _signalRService.EnviarStatusAsync(
                    "Aplicação Kinect conectada ao MVC.");
            }
            catch (Exception ex)
            {
                _signalRConectado = false;

                Status =
                    "Erro ao conectar SignalR: " + ex.Message;
            }
        }

        // ==========================================
        // LIGAR KINECT
        // ==========================================

        private async void LigarKinect()
        {
            try
            {
                Status =
                    "Inicializando Kinect...";

                bool sucesso =
                    _kinectService
                        .InicializarKinect();

                if (sucesso)
                {
                    Status =
                        "Kinect iniciado.";

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

                    if (_signalRConectado)
                    {
                        await _signalRService.EnviarStatusAsync(
                            "Falha ao iniciar Kinect.");
                    }
                }
            }
            catch (Exception ex)
            {
                Status =
                    ex.Message;
            }
        }

        // ==========================================
        // DESLIGAR KINECT
        // ==========================================

        private async void DesligarKinect()
        {
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

                Status =
                    "Kinect desligado e conexão com MVC encerrada.";
            }
        }

        // ==========================================
        // CALIBRAR CHÃO
        // ==========================================

        private async void CalibrarChao()
        {
            _kinectService.CalibrarChao();

            Status =
                "Chão calibrado.";

            if (_signalRConectado)
            {
                await _signalRService.EnviarStatusAsync(
                    "Chão calibrado.");
            }
        }

        // ==========================================
        // STATUS
        // ==========================================

        private void AtualizarStatus(
            string msg)
        {
            Status = msg;
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

        private void ProcessarNovaMedida(double volumeCm3)
        {
            // Atualiza o texto exibido na tela sempre que uma nova medida chega do Kinect.
            // Essa atualização acontece em tempo real, conforme o Kinect calcula o volume.
            VolumeTexto =
                $"Volume: {volumeCm3:F0} cm³";

            // ==========================================================
            // CONTROLE DE GRAVAÇÃO NO SQLITE
            // ==========================================================
            // O Kinect gera muitas medições por segundo.
            // Se salvarmos todas as medições no banco, o SQLite pode ficar pesado
            // e o sistema pode perder desempenho.
            //
            // Por isso usamos a variável _proximaGravacao como um controle de tempo.
            // Ela indica quando será permitido salvar novamente no banco.
            //
            // Exemplo:
            // - Salvou agora às 10:00:00
            // - A próxima gravação só será permitida às 10:00:01
            //
            // Assim, o sistema salva no SQLite apenas 1 vez por segundo.

            if (DateTime.Now >= _proximaGravacao)
            {
                // Define o próximo horário permitido para salvar no SQLite.
                // Aqui configuramos para salvar novamente somente depois de 1 segundo.
                _proximaGravacao =
                    DateTime.Now.AddSeconds(1);

                // Cria o objeto de medição que será gravado no banco SQLite.
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

                // A gravação no banco é feita em segundo plano com Task.Run.
                // Isso evita travar a tela enquanto o SQLite salva os dados.
                Task.Run(() =>
                {
                    try
                    {
                        _repository
                            .SalvarMedicao(
                                medicao);
                    }
                    catch
                    {
                        // Evita que uma falha de gravação derrube o sistema.
                        // Se quiser, depois podemos registrar esse erro em log.
                    }
                });
            }

            // ==========================================================
            // CONTROLE DE ENVIO PARA O MVC VIA SIGNALR
            // ==========================================================
            // Além de salvar no SQLite, o sistema também envia o volume
            // para o site MVC publicado:
            //
            // http://inventorymasters.runasp.net/residuosHub
            //
            // Esse envio não precisa acontecer a cada medição do Kinect,
            // porque o Kinect gera várias leituras por segundo.
            //
            // Por isso usamos a variável _proximoEnvioSignalR.
            // Ela funciona como um timer de envio.
            //
            // Exemplo:
            // - Enviou agora às 10:00:00
            // - O próximo envio só será permitido às 10:00:15
            //
            // Assim, o MVC recebe atualizações a cada 15 segundos.

            if (_signalRConectado &&
                DateTime.Now >= _proximoEnvioSignalR)
            {
                // Define o próximo horário permitido para enviar ao MVC.
                // Aqui configuramos o envio para ocorrer a cada 15 segundos.
                _proximoEnvioSignalR =
                    DateTime.Now.AddSeconds(15);

                // O envio é feito em segundo plano para não travar a interface.
                Task.Run(async () =>
                {
                    try
                    {
                        // Envia o volume calculado para o Hub SignalR do MVC.
                        await _signalRService
                            .EnviarVolumeAsync(
                                volumeCm3);

                        // Envia também uma mensagem de status para o MVC.
                        await _signalRService
                            .EnviarStatusAsync(
                                "Volume enviado pelo Kinect.");
                    }
                    catch
                    {
                        // Evita que uma falha de conexão com o MVC derrube o sistema.
                        // Se o site estiver fora do ar ou sem internet,
                        // o WPF continua funcionando e salvando localmente.
                    }
                });
            }
        }
    }
}