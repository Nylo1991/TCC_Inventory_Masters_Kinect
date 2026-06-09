using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using TCC_Inventory_Masters_Kinect.Command.TCC_Inventory_Masters_Kinect.Command;
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
        }

        private async void ExecutarCalibracao()
        {
            try
            {
                IsCalibrating = true;
                StatusMessage = "Calibrando...";

                var progress = new Progress<Model.CalibrationProgress>(p =>
                {
                    StatusMessage = p.Status;
                });

                await _kinectService.CalibrateAsync(progress, CancellationToken.None);

                var novoEspaco = new Space
                {
                    Name = "Espaço Principal"
                };

                _repository.SalvarEspaco(novoEspaco);

                StatusMessage = "Calibração concluída!";
            }
            catch (Exception ex)
            {
                StatusMessage = "Erro na calibração";
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
                        // Removido DataMedicao e EspacoId (não existem no seu modelo atual)
                    };

                    _repository.SalvarMedicao(medicao);
                    StatusMessage = $"Medido: {volume:F0} cm³";
                }
            }
            catch
            {
                StatusMessage = "Erro na medição";
            }
        }
    }
}
