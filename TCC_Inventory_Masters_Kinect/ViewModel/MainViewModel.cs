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
        // 1. CAMPOS PRIVADOS
        // ==========================================
        private readonly KinectService _kinectService;
        private readonly IKinectRepository _repository;

        // Controle para não inundar o banco de dados (Salva a cada 1 segundo)
        private DateTime _proximaGravacaoPermitida = DateTime.MinValue;

        // ==========================================
        // 2. PROPRIEDADES (NOTIFY PROPERTY CHANGED)
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
        // 3. COMANDOS
        // ==========================================
        public ICommand LigarKinectCommand { get; }
        public ICommand DesligarKinectCommand { get; }

        // ==========================================
        // 4. CONSTRUTOR
        // ==========================================
        public MainViewModel()
        {
            // Inicializa Serviços e Repositório
            _kinectService = new KinectService();
            _repository = new KinectRepository();

            // Assina os eventos do Kinect
            _kinectService.MedidaAtualizada += AtualizarMedida;
            _kinectService.StatusAtualizado += AtualizarStatus;

            // Estado Inicial
            Status = "Kinect desligado";
            VolumeTexto = "Medida: 0 mm";

            // Inicializa Comandos
            LigarKinectCommand = new RelayCommand(LigarKinect);
            DesligarKinectCommand = new RelayCommand(DesligarKinect);
        }

        // ==========================================
        // 5. MÉTODOS DE AÇÃO
        // ==========================================
        private void LigarKinect()
        {
            try
            {
                Status = "Tentando ligar Kinect...";
                bool conectado = _kinectService.InicializarKinect();
                Status = conectado ? "Kinect conectado e lendo..." : "Kinect não encontrado.";
            }
            catch (Exception ex)
            {
                Status = "Erro ao ligar: " + ex.Message;
            }
        }

        private void DesligarKinect()
        {
            _kinectService.DesligarKinect();
            Status = "Kinect desligado";
            VolumeTexto = "Medida: 0 mm";
        }

        // ==========================================
        // 6. ATUALIZAÇÃO DA UI E GRAVAÇÃO
        // ==========================================
        private void AtualizarMedida(double medidaMm)
        {
            // 1. Atualiza o que o usuário vê na tela imediatamente
            VolumeTexto = $"Medida atual: {medidaMm:F0} mm";

            // 2. Lógica de Gravação no SQLite
            // Verificamos se já passou 1 segundo desde a última gravação
            if (DateTime.Now >= _proximaGravacaoPermitida)
            {
                _proximaGravacaoPermitida = DateTime.Now.AddSeconds(1);

                var novaMedicao = new MedicaoVolume
                {
                    DataHora = DateTime.Now,
                    VolumeCm3 = medidaMm,
                    KinectLigado = true,
                    Calibrado = true,
                    Status = "Captura Automática"
                };

                // Task.Run garante que a gravação no banco não "congele" a imagem do Kinect
                Task.Run(() =>
                {
                    try
                    {
                        _repository.SalvarMedicao(novaMedicao);
                    }
                    catch (Exception ex)
                    {
                        // Opcional: Logar erro de banco se necessário
                        System.Diagnostics.Debug.WriteLine("Erro ao salvar: " + ex.Message);
                    }
                });
            }
        }

        private void AtualizarStatus(string mensagem)
        {
            Status = mensagem;
        }
    }
}