using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        // STATUS GERAL
        // ==========================================

        private string _status = string.Empty;

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
        // STATUS VISUAL DE CONEXÃO
        // ==========================================

        private string _statusKinect =
            "Kinect: Desconectado";

        public string StatusKinect
        {
            get => _statusKinect;

            set
            {
                _statusKinect = value;
                OnPropertyChanged();
            }
        }

        private string _statusSQLite =
            "SQLite: Aguardando";

        public string StatusSQLite
        {
            get => _statusSQLite;

            set
            {
                _statusSQLite = value;
                OnPropertyChanged();
            }
        }

        private string _statusSignalR =
            "SignalR: Desconectado";

        public string StatusSignalR
        {
            get => _statusSignalR;

            set
            {
                _statusSignalR = value;
                OnPropertyChanged();
            }
        }

        private string _statusMvcFirebase =
            "MVC/Firebase: Aguardando";

        public string StatusMvcFirebase
        {
            get => _statusMvcFirebase;

            set
            {
                _statusMvcFirebase = value;
                OnPropertyChanged();
            }
        }

        // ==========================================
        // VOLUME
        // ==========================================

        private string _volumeTexto =
            "Volume: 0 cm³";

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
        // DADOS DO ESPAÇO
        // ==========================================

        private string _nomeEspaco = string.Empty;

        public string NomeEspaco
        {
            get => _nomeEspaco;

            set
            {
                _nomeEspaco = value;
                OnPropertyChanged();
            }
        }

        private string _volumeMaximo = string.Empty;

        public string VolumeMaximo
        {
            get => _volumeMaximo;

            set
            {
                _volumeMaximo = value;
                OnPropertyChanged();
            }
        }

        private string _percentualAlerta = string.Empty;

        public string PercentualAlerta
        {
            get => _percentualAlerta;

            set
            {
                _percentualAlerta = value;
                OnPropertyChanged();
            }
        }

        // ==========================================
        // INFORMAÇÕES CAPTURADAS
        // ==========================================

        private string _quantidadePontos3D =
            "Pontos 3D: 0";

        public string QuantidadePontos3D
        {
            get => _quantidadePontos3D;

            set
            {
                _quantidadePontos3D = value;
                OnPropertyChanged();
            }
        }

        private string _ultimoSnapshot =
            "Snapshot: nenhum";

        public string UltimoSnapshot
        {
            get => _ultimoSnapshot;

            set
            {
                _ultimoSnapshot = value;
                OnPropertyChanged();
            }
        }

        private string _percentualOcupacaoTexto =
            "Ocupação: 0%";

        public string PercentualOcupacaoTexto
        {
            get => _percentualOcupacaoTexto;

            set
            {
                _percentualOcupacaoTexto = value;
                OnPropertyChanged();
            }
        }

        private string _espacoLivreTexto =
            "Espaço livre: 0 cm³";

        public string EspacoLivreTexto
        {
            get => _espacoLivreTexto;

            set
            {
                _espacoLivreTexto = value;
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
        // HISTÓRICO DE MEDIÇÕES
        // ==========================================

        public ObservableCollection<MedicaoVolume> HistoricoMedicoes
        {
            get;
            set;
        } =
        new ObservableCollection<MedicaoVolume>();

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
                    "Serviços inicializados.");

                // ==========================================
                // EVENTOS SIGNALR
                // ==========================================

                _signalRService.StatusSignalRAtualizado +=
                    AtualizarStatus;

                // ==========================================
                // EVENTOS KINECT
                // ==========================================

                _kinectService.MedidaAtualizada +=
                    ProcessarNovaMedida;

                _kinectService.StatusAtualizado +=
                    AtualizarStatus;

                _kinectService.CameraAtualizada +=
                    AtualizarCamera;

                _kinectService.PointCloudAtualizada +=
                    AtualizarPointCloud;

                _kinectService.SnapshotCriado +=
                    AtualizarSnapshot;

                // ==========================================
                // TEXTO INICIAL
                // ==========================================

                Status =
                    "Sistema aguardando inicialização...";

                VolumeTexto =
                    "Volume: 0 cm³";

                StatusKinect =
                    "Kinect: Desconectado";

                StatusSQLite =
                    "SQLite: Aguardando";

                StatusSignalR =
                    "SignalR: Desconectado";

                StatusMvcFirebase =
                    "MVC/Firebase: Aguardando";

                QuantidadePontos3D =
                    "Pontos 3D: 0";

                PercentualOcupacaoTexto =
                    "Ocupação: 0%";

                EspacoLivreTexto =
                    "Espaço livre: 0 cm³";

                UltimoSnapshot =
                    "Snapshot: nenhum";

                // ==========================================
                // COMANDOS
                // ==========================================

                LigarKinectCommand =
                    new RelayCommand(LigarKinect);

                DesligarKinectCommand =
                    new RelayCommand(DesligarKinect);

                CalibrarCommand =
                    new RelayCommand(CalibrarChao);

                LoggerService.Info(
                    "Comandos configurados.");

                // ==========================================
                // HISTÓRICO
                // ==========================================

                CarregarHistoricoMedicoes();

                // ==========================================
                // CONECTAR SIGNALR
                // ==========================================

                ConectarSignalR();
            }
            catch (Exception ex)
            {
                LoggerService.Erro(
                    "Erro ao inicializar MainViewModel.",
                    ex);

                Status =
                    "Erro ao inicializar sistema: " + ex.Message;
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

                await _signalRService
                    .ConectarAsync();

                _signalRConectado =
                    true;

                StatusSignalR =
                    "SignalR: Conectado";

                Status =
                    "Conectado ao MVC via SignalR.";

                LoggerService.Info(
                    "Conectado ao SignalR com sucesso.");

                await _signalRService
                    .EnviarStatusAsync(
                        "Aplicação Kinect conectada ao MVC.");
            }
            catch (Exception ex)
            {
                _signalRConectado =
                    false;

                StatusSignalR =
                    "SignalR: Sem conexão";

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

                StatusKinect =
                    "Kinect: Inicializando";

                CriarEspacoMapeadoSePossivel();

                bool sucesso =
                    _kinectService
                        .InicializarKinect();

                if (sucesso)
                {
                    Status =
                        "Kinect iniciado.";

                    StatusKinect =
                        "Kinect: Conectado";

                    LoggerService.Info(
                        "Kinect iniciado com sucesso.");

                    if (_signalRConectado)
                    {
                        await _signalRService
                            .EnviarStatusAsync(
                                "Kinect iniciado.");
                    }
                }
                else
                {
                    Status =
                        "Falha ao iniciar Kinect.";

                    StatusKinect =
                        "Kinect: Erro ao iniciar";

                    LoggerService.Info(
                        "Falha ao iniciar Kinect.");

                    if (_signalRConectado)
                    {
                        await _signalRService
                            .EnviarStatusAsync(
                                "Falha ao iniciar Kinect.");
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerService.Erro(
                    "Erro ao iniciar Kinect.",
                    ex);

                StatusKinect =
                    "Kinect: Erro";

                Status =
                    "Erro ao iniciar Kinect: " + ex.Message;
            }
        }

        // ==========================================
        // CRIAR ESPAÇO MAPEADO
        // ==========================================

        private void CriarEspacoMapeadoSePossivel()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(
                    NomeEspaco))
                {
                    LoggerService.Info(
                        "Espaço não definido. Kinect será iniciado sem cadastro de espaço.");

                    return;
                }

                double volumeMaximoCm3 =
                    0;

                double percentualAlerta =
                    0;

                double.TryParse(
                    VolumeMaximo,
                    out volumeMaximoCm3);

                double.TryParse(
                    PercentualAlerta,
                    out percentualAlerta);

                var espaco =
                    new EspacoMapeado
                    {
                        NomeEspaco =
                            NomeEspaco,

                        VolumeMaximoPermitidoCm3 =
                            volumeMaximoCm3,

                        VolumeAtualCm3 =
                            0,

                        PercentualOcupacao =
                            0,

                        EspacoLivreCm3 =
                            volumeMaximoCm3,

                        Ativo =
                            true,

                        MapeamentoConcluido =
                            false,

                        Status =
                            "Mapeamento iniciado",

                        DataCriacao =
                            DateTime.Now
                    };

                _kinectService
                    .DefinirEspaco(
                        espaco);

                LoggerService.Info(
                    "Espaço enviado ao KinectService: " + NomeEspaco);
            }
            catch (Exception ex)
            {
                LoggerService.Erro(
                    "Erro ao criar espaço mapeado.",
                    ex);
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

                _kinectService
                    .DesligarKinect();

                Status =
                    "Kinect desligado.";

                StatusKinect =
                    "Kinect: Desconectado";

                VolumeTexto =
                    "Volume: 0 cm³";

                CameraImage =
                    null;

                QuantidadePontos3D =
                    "Pontos 3D: 0";

                PercentualOcupacaoTexto =
                    "Ocupação: 0%";

                EspacoLivreTexto =
                    "Espaço livre: 0 cm³";

                UltimoSnapshot =
                    "Snapshot: nenhum";

                StatusMvcFirebase =
                    "MVC/Firebase: Aguardando";

                if (_signalRConectado)
                {
                    await _signalRService
                        .EnviarStatusAsync(
                            "Kinect desligado.");

                    await _signalRService
                        .DesconectarAsync();

                    _signalRConectado =
                        false;

                    StatusSignalR =
                        "SignalR: Desconectado";

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

                Status =
                    "Erro ao desligar Kinect: " + ex.Message;
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

                _kinectService
                    .CalibrarChao();

                Status =
                    "Chão calibrado.";

                LoggerService.Info(
                    "Chão calibrado com sucesso.");

                if (_signalRConectado)
                {
                    await _signalRService
                        .EnviarStatusAsync(
                            "Chão calibrado.");
                }
            }
            catch (Exception ex)
            {
                LoggerService.Erro(
                    "Erro ao calibrar chão.",
                    ex);

                Status =
                    "Erro ao calibrar chão: " + ex.Message;
            }
        }

        // ==========================================
        // STATUS
        // ==========================================

        private void AtualizarStatus(
            string msg)
        {
            Status =
                msg;

            LoggerService.Info(
                "Status atualizado: " + msg);
        }

        // ==========================================
        // CAMERA
        // ==========================================

        private void AtualizarCamera(
            ImageSource imagem)
        {
            CameraImage =
                imagem;
        }

        // ==========================================
        // POINT CLOUD
        // ==========================================

        private void AtualizarPointCloud(
            List<Point3DData> pontos)
        {
            try
            {
                QuantidadePontos3D =
                    $"Pontos 3D: {pontos.Count}";

                LoggerService.Info(
                    $"Point Cloud atualizada com {pontos.Count} pontos.");
            }
            catch (Exception ex)
            {
                LoggerService.Erro(
                    "Erro ao atualizar Point Cloud na interface.",
                    ex);
            }
        }

        // ==========================================
        // SNAPSHOT
        // ==========================================

        private void AtualizarSnapshot(
            SnapshotEspacial snapshot)
        {
            try
            {
                UltimoSnapshot =
                    $"Snapshot: {snapshot.NomeSnapshot}";

                PercentualOcupacaoTexto =
                    $"Ocupação: {snapshot.PercentualOcupacao:F2}%";

                EspacoLivreTexto =
                    $"Espaço livre: {snapshot.EspacoLivreCm3:F0} cm³";

                LoggerService.Info(
                    "Snapshot recebido na interface: " + snapshot.NomeSnapshot);
            }
            catch (Exception ex)
            {
                LoggerService.Erro(
                    "Erro ao atualizar snapshot na interface.",
                    ex);
            }
        }

        // ==========================================
        // CARREGAR HISTÓRICO
        // ==========================================

        private void CarregarHistoricoMedicoes()
        {
            try
            {
                HistoricoMedicoes.Clear();

                var medicoes =
                    _repository
                        .ObterUltimasMedicoes(
                            50);

                foreach (var medicao in medicoes)
                {
                    HistoricoMedicoes.Add(
                        medicao);
                }

                LoggerService.Info(
                    "Histórico de medições carregado.");
            }
            catch (Exception ex)
            {
                LoggerService.Erro(
                    "Erro ao carregar histórico de medições.",
                    ex);
            }
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

            if (DateTime.Now >=
                _proximaGravacao)
            {
                _proximaGravacao =
                    DateTime.Now.AddSeconds(
                        1);

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
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            StatusSQLite =
                                "SQLite: Salvando";
                        });

                        _repository
                            .SalvarMedicao(
                                medicao);

                        LoggerService.Info(
                            $"Medição salva no SQLite. Volume: {volumeCm3:F0} cm³");

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            HistoricoMedicoes.Insert(
                                0,
                                medicao);

                            if (HistoricoMedicoes.Count > 50)
                            {
                                HistoricoMedicoes.RemoveAt(
                                    HistoricoMedicoes.Count - 1);
                            }

                            StatusSQLite =
                                "SQLite: Salvo com sucesso";
                        });
                    }
                    catch (Exception ex)
                    {
                        LoggerService.Erro(
                            "Erro ao salvar medição no SQLite.",
                            ex);

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            StatusSQLite =
                                "SQLite: Erro ao salvar";
                        });
                    }
                });
            }

            // ==========================================
            // ENVIAR MVC VIA SIGNALR
            // ==========================================

            if (_signalRConectado &&
                DateTime.Now >=
                _proximoEnvioSignalR)
            {
                _proximoEnvioSignalR =
                    DateTime.Now.AddSeconds(
                        15);

                Task.Run(async () =>
                {
                    try
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            StatusMvcFirebase =
                                "MVC/Firebase: Enviando";
                        });

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
                            StatusMvcFirebase =
                                "MVC/Firebase: Enviado";

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
                            StatusMvcFirebase =
                                "MVC/Firebase: Falha no envio";

                            Status =
                                "Falha ao enviar informações ao MVC. Dados mantidos no SQLite. Erro: " + ex.Message;
                        });
                    }
                });
            }
        }
    }
}