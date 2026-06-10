using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using TCC_Inventory_Masters_Kinect.Command;
using TCC_Inventory_Masters_Kinect.Model;
using TCC_Inventory_Masters_Kinect.Repository;
using TCC_Inventory_Masters_Kinect.Service;

namespace TCC_Inventory_Masters_Kinect.ViewModel
{
    public class MainViewModel

    {
        private readonly KinectService _kinectService;
        private readonly KinectRepository _repository;

        public string StatusMessage { get; set; }
        public bool IsCalibrating { get; set; }

        public ICommand CalibrarCommand { get; }
        public ICommand MedirCommand { get; }

        public MainViewModel()
        {
            _kinectService = new KinectService();
            _repository = new KinectRepository();

            CalibrarCommand = new RelayCommand(ExecutarCalibracao);
            MedirCommand = new RelayCommand(ExecutarMedicao);

            StatusMessage = "Pronto";

            try
            {
                _kinectService.Start();
                StatusMessage = "Kinect conectado";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Erro ao conectar Kinect: {ex.Message}";
            }
        }

        private async void ExecutarCalibracao()
        {
            try
            {
                IsCalibrating = true;
                StatusMessage = "Calibrando...";

                // Chama sem Progress - apenas aguarda a conclusão
                await _kinectService.CalibrateAsync(CancellationToken.None);

                var novoEspaco = new Space
                {
                    Name = "Espaco Principal"
                };

                _repository.SalvarEspaco(novoEspaco);

                StatusMessage = "Calibracao concluida!";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Erro na calibracao: {ex.Message}";
            }
            finally
            {
                IsCalibrating = false;
            }
        }

        private async void ExecutarMedicao()
        {
            try
            {
                StatusMessage = "Medindo...";

                double volume = await _kinectService.MeasureCurrentVolumeAsync(CancellationToken.None);

                if (volume > 0)
                {
                    var medicao = new MedicaoVolume
                    {
                        VolumeCm3 = volume
                    };

                    _repository.SalvarMedicao(medicao);
                    StatusMessage = $"Medido: {volume:F0} cm3";
                }
                else
                {
                    StatusMessage = "Nenhum objeto detectado";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Erro na medicao: {ex.Message}";
            }
        }
    }
}
