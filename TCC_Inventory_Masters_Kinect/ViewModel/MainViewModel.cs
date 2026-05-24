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

        private DateTime _proximaGravacao =
            DateTime.MinValue;

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

            // EVENTOS

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
        }

        // ==========================================
        // LIGAR KINECT
        // ==========================================

        private void LigarKinect()
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
                }
                else
                {
                    Status =
                        "Falha ao iniciar Kinect.";
                }
            }
            catch (Exception ex)
            {
                Status =
                    ex.Message;
            }
        }

        // ==========================================
        // DESLIGAR
        // ==========================================

        private void DesligarKinect()
        {
            _kinectService.DesligarKinect();

            Status =
                "Kinect desligado.";

            VolumeTexto =
                "Volume: 0 cm³";

            CameraImage = null;
        }

        // ==========================================
        // CALIBRAR
        // ==========================================

        private void CalibrarChao()
        {
            _kinectService.CalibrarChao();

            Status =
                "Chão calibrado.";
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

        private void ProcessarNovaMedida(
            double volumeCm3)
        {
            VolumeTexto =
                $"Volume: {volumeCm3:F0} cm³";

            // SALVAR SQLITE

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
                    }
                    catch
                    {
                    }
                });
            }
        }
    }
}