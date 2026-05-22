using System;
using System.Windows.Input;
using TCC_Inventory_Masters_Kinect.Repository;
using TCC_Inventory_Masters_Kinect.Repository.Interface;
using TCC_Inventory_Masters_Kinect.Model;

namespace TCC_Inventory_Masters_Kinect.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        // ==========================================
        // 1. CAMPOS PRIVADOS
        // ==========================================

        private readonly KinectService _kinectService;

        // ==========================================
        // 2. PROPRIEDADE STATUS
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
        // 3. PROPRIEDADE VOLUME
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
        // 4. COMANDOS
        // ==========================================

        public ICommand LigarKinectCommand { get; }
        public ICommand DesligarKinectCommand { get; }

        // ==========================================
        // 5. CONSTRUTOR
        // ==========================================

        public MainViewModel()
        {
            _kinectService = new KinectService();

            _kinectService.MedidaAtualizada += AtualizarMedida;
            _kinectService.StatusAtualizado += AtualizarStatus;

            Status = "Kinect desligado";
            VolumeTexto = "Medida: 0 mm";

            LigarKinectCommand = new RelayCommand(LigarKinect);
            DesligarKinectCommand = new RelayCommand(DesligarKinect);
        }

        // ==========================================
        // 6. LIGAR KINECT
        // ==========================================

        private void LigarKinect()
        {
            try
            {
                Status = "Tentando ligar Kinect...";

                bool conectado = _kinectService.InicializarKinect();

                if (conectado)
                {
                    Status = "Kinect conectado.";
                }
                else
                {
                    Status = "Kinect não encontrado. Verifique USB, energia e driver.";
                }
            }
            catch (Exception ex)
            {
                Status = "Erro ao ligar Kinect: " + ex.Message;
            }
        }

        // ==========================================
        // 7. DESLIGAR KINECT
        // ==========================================

        private void DesligarKinect()
        {
            try
            {
                _kinectService.DesligarKinect();

                Status = "Kinect desligado";
                VolumeTexto = "Medida: 0 mm";
            }
            catch (Exception ex)
            {
                Status = "Erro ao desligar Kinect: " + ex.Message;
            }
        }

        // ==========================================
        // 8. ATUALIZAR MEDIDA
        // ==========================================

        private void AtualizarMedida(double medidaMm)
        {
            VolumeTexto = "Medida média: " + medidaMm.ToString("F0") + " mm";
        }

        // ==========================================
        // 9. ATUALIZAR STATUS
        // ==========================================

        private void AtualizarStatus(string mensagem)
        {
            Status = mensagem;
        }
    }
}