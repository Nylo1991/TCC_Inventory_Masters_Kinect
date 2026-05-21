using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TCC_Inventory_Masters_Kinect.Command;
using TCC_Inventory_Masters_Kinect.Service;

namespace TCC_Inventory_Masters_Kinect.ViewModel
{
    public class MainViewModel
    {
        private readonly KinectService _kinectService;

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

        public ICommand LigarKinectCommand { get; }
        public ICommand DesligarKinectCommand { get; }

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

        private void AtualizarMedida(double medidaMm)
        {
            VolumeTexto = "Medida média: " + medidaMm.ToString("F0") + " mm";
        }

        private void AtualizarStatus(string mensagem)
        {
            Status = mensagem;
        }
    }
}
