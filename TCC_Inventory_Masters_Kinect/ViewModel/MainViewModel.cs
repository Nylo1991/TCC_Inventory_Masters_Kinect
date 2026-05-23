using System;
using System.Windows.Input;
using System.Threading.Tasks;
using TCC_Inventory_Masters_Kinect.Repository;
using TCC_Inventory_Masters_Kinect.Repository.Interface;
using TCC_Inventory_Masters_Kinect.Model;
using TCC_Inventory_Masters_Kinect.Service;
using TCC_Inventory_Masters_Kinect.Command;

namespace TCC_Inventory_Masters_Kinect.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        // ==========================================
        // CAMPOS PRIVADOS
        // ==========================================
        private readonly KinectService _kinectService;
        private readonly IKinectRepository _repository;
        private DateTime _proximaGravacao = DateTime.MinValue;

        // ==========================================
        // PROPRIEDADES PARA A INTERFACE (WPF)
        // ==========================================
        private string _status;
        public string Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(); }
        }

        private string _volumeTexto;
        public string VolumeTexto
        {
            get => _volumeTexto;
            set { _volumeTexto = value; OnPropertyChanged(); }
        }

        // ==========================================
        // COMANDOS
        // ==========================================
        public ICommand LigarKinectCommand { get; }
        public ICommand DesligarKinectCommand { get; }

        // ==========================================
        // CONSTRUTOR
        // ==========================================
        public MainViewModel()
        {
            _kinectService = new KinectService();
            _repository = new KinectRepository(); // Repositório que usa o AppDbContext

            // Subscreve os eventos vindos do sensor
            _kinectService.MedidaAtualizada += ProcessarNovaMedida;
            _kinectService.StatusAtualizado += (msg) => Status = msg;

            Status = "Kinect aguardando...";
            VolumeTexto = "Medida: 0 mm";

            LigarKinectCommand = new RelayCommand(LigarKinect);
            DesligarKinectCommand = new RelayCommand(DesligarKinect);
        }

        private void LigarKinect()
        {
            try
            {
                Status = "Inicializando sensor...";
                bool sucesso = _kinectService.InicializarKinect();
                Status = sucesso ? "Kinect ativo e gravando!" : "Falha ao ligar o sensor.";
            }
            catch (Exception ex)
            {
                Status = "Erro crítico: " + ex.Message;
            }
        }

        private void DesligarKinect()
        {
            _kinectService.DesligarKinect();
            Status = "Kinect parado.";
            VolumeTexto = "Medida: 0 mm";
        }

        // ==========================================
        // PROCESSAMENTO E GRAVAÇÃO SQLITE
        // ==========================================
        private void ProcessarNovaMedida(double medidaMm)
        {
            // 1. Atualiza o ecrã imediatamente
            VolumeTexto = $"Medida Média: {medidaMm:F0} mm";

            // 2. Gravação Automática (Filtro de 1 segundo para não travar o PC)
            if (DateTime.Now >= _proximaGravacao)
            {
                _proximaGravacao = DateTime.Now.AddSeconds(1);

                var medicao = new MedicaoVolume
                {
                    DataHora = DateTime.Now,
                    VolumeCm3 = medidaMm,
                    KinectLigado = true,
                    Calibrado = true,
                    Status = "Auto-Save"
                };

                // Executa a gravação em segundo plano (async) para a tela não "congelar"
                Task.Run(() =>
                {
                    try
                    {
                        _repository.SalvarMedicao(medicao);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("Erro SQLite: " + ex.Message);
                    }
                });
            }
        }
    }
}