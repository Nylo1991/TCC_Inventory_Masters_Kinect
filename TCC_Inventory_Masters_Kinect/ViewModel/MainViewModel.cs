using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using TCC_Inventory_Masters_Kinect.Command.TCC_Inventory_Masters_Kinect.Command;
using TCC_Inventory_Masters_Kinect.ConfigKinect;
using TCC_Inventory_Masters_Kinect.Logs;
using TCC_Inventory_Masters_Kinect.Model;
using TCC_Inventory_Masters_Kinect.Repository;
using TCC_Inventory_Masters_Kinect.Repository.Interface;
using TCC_Inventory_Masters_Kinect.Service;

namespace TCC_Inventory_Masters_Kinect.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        private readonly KinectService _kinectService;
        private readonly IKinectRepository _repository;
        private readonly SignalRService _signalRService;

        // ==================== PROPRIEDADE PÚBLICA ====================
        public KinectService KinectService => _kinectService;

        private DateTime _proximaGravacao = DateTime.MinValue;
        private DateTime _proximoEnvioSignalR = DateTime.MinValue;
        private bool _signalRConectado = false;
        private bool _encerrando = false;

        private double _volumeMaximoValor = 0;
        private int _limitePercentual = 80;

        private string _status = string.Empty;
        public string Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(); }
        }

        private string _statusKinect = "Kinect: Desconectado";
        public string StatusKinect
        {
            get => _statusKinect;
            set { _statusKinect = value; OnPropertyChanged(); }
        }

        private string _statusSQLite = "SQLite: Aguardando";
        public string StatusSQLite
        {
            get => _statusSQLite;
            set { _statusSQLite = value; OnPropertyChanged(); }
        }

        private string _statusSignalR = "SignalR: Desconectado";
        public string StatusSignalR
        {
            get => _statusSignalR;
            set { _statusSignalR = value; OnPropertyChanged(); }
        }

        private string _statusMvcFirebase = "MVC/Firebase: Aguardando";
        public string StatusMvcFirebase
        {
            get => _statusMvcFirebase;
            set { _statusMvcFirebase = value; OnPropertyChanged(); }
        }

        private string _mensagemEnvioAplicacao = "Envio externo: aguardando comunicação com o MVC.";
        public string MensagemEnvioAplicacao
        {
            get => _mensagemEnvioAplicacao;
            set { _mensagemEnvioAplicacao = value; OnPropertyChanged(); }
        }

        private string _volumeTexto = "Volume: 0 cm³";
        public string VolumeTexto
        {
            get => _volumeTexto;
            set { _volumeTexto = value; OnPropertyChanged(); }
        }

        private string _nomeEspaco = string.Empty;
        public string NomeEspaco
        {
            get => _nomeEspaco;
            set { _nomeEspaco = value; OnPropertyChanged(); }
        }

        private string _volumeMaximo = string.Empty;
        public string VolumeMaximo
        {
            get => _volumeMaximo;
            set { _volumeMaximo = value; OnPropertyChanged(); }
        }

        private string _percentualAlerta = string.Empty;
        public string PercentualAlerta
        {
            get => _percentualAlerta;
            set { _percentualAlerta = value; OnPropertyChanged(); }
        }

        private string _quantidadePontos3D = "Pontos 3D: 0";
        public string QuantidadePontos3D
        {
            get => _quantidadePontos3D;
            set { _quantidadePontos3D = value; OnPropertyChanged(); }
        }

        private string _ultimoSnapshot = "Snapshot: nenhum";
        public string UltimoSnapshot
        {
            get => _ultimoSnapshot;
            set { _ultimoSnapshot = value; OnPropertyChanged(); }
        }

        private string _percentualOcupacaoTexto = "Ocupação: 0%";
        public string PercentualOcupacaoTexto
        {
            get => _percentualOcupacaoTexto;
            set { _percentualOcupacaoTexto = value; OnPropertyChanged(); }
        }

        private string _espacoLivreTexto = "Espaço livre: 0 cm³";
        public string EspacoLivreTexto
        {
            get => _espacoLivreTexto;
            set { _espacoLivreTexto = value; OnPropertyChanged(); }
        }

        private ImageSource _cameraImage;
        public ImageSource CameraImage
        {
            get => _cameraImage;
            set { _cameraImage = value; OnPropertyChanged(); }
        }

        public ObservableCollection<MedicaoVolume> HistoricoMedicoes { get; set; } = new ObservableCollection<MedicaoVolume>();

        public ICommand LigarKinectCommand { get; }
        public ICommand DesligarKinectCommand { get; }
        public ICommand CalibrarCommand { get; }
        public ICommand CalibrarEspacoCommand { get; }

        public MainViewModel()
        {
            try
            {
                LoggerService.Info("Inicializando MainViewModel.");

                _kinectService = new KinectService();
                _repository = new KinectRepository();
                _signalRService = new SignalRService();

                _signalRService.StatusSignalRAtualizado += AtualizarStatusSignalR;

                _kinectService.MedidaAtualizada += ProcessarNovaMedida;
                _kinectService.StatusAtualizado += AtualizarStatus;
                _kinectService.CameraAtualizada += AtualizarCamera;
                _kinectService.PointCloudAtualizada += AtualizarPointCloud;
                _kinectService.SnapshotCriado += AtualizarSnapshot;
                _kinectService.CalibracaoConcluida += AtualizarVolumeMaximo;

                Status = "Sistema aguardando inicialização...";
                VolumeTexto = "Volume: 0 cm³";
                StatusKinect = "Kinect: Desconectado";
                StatusSQLite = "SQLite: Aguardando";
                StatusSignalR = "SignalR: Desconectado";
                StatusMvcFirebase = "MVC/Firebase: Aguardando";
                MensagemEnvioAplicacao = "Envio externo: aguardando comunicação com o MVC.";
                QuantidadePontos3D = "Pontos 3D: 0";
                PercentualOcupacaoTexto = "Ocupação: 0%";
                EspacoLivreTexto = "Espaço livre: 0 cm³";
                UltimoSnapshot = "Snapshot: nenhum";

                LigarKinectCommand = new RelayCommand(LigarKinect);
                DesligarKinectCommand = new RelayCommand(DesligarKinect);
                CalibrarCommand = new RelayCommand(CalibrarChao);
                CalibrarEspacoCommand = new RelayCommand(CalibrarEspaco);

                CarregarHistoricoMedicoes();
                ConectarSignalR();

                Task.Run(async () =>
                {
                    await Task.Delay(5000);
                    LoggerService.Info($"Estado SignalR após 5s: {_signalRService.EstadoConexao}");
                });
            }
            catch (Exception ex)
            {
                LoggerService.Erro("Erro ao inicializar MainViewModel.", ex);
                Status = "Erro ao inicializar sistema: " + ex.Message;
                MensagemEnvioAplicacao = "Não foi possível inicializar a comunicação externa.";
            }
        }

        private void AtualizarVolumeMaximo(double volumeMax)
        {
            _volumeMaximoValor = volumeMax;

            ExecutarNaUI(() =>
            {
                VolumeMaximo = volumeMax.ToString("F0");
                Status = $"Espaço calibrado. Volume máximo: {volumeMax:F0} cm³";
            });
        }

        private void CalibrarEspaco()
        {
            try
            {
                if (_encerrando) return;
                _kinectService.CalibrarEspaco();
            }
            catch (Exception ex)
            {
                LoggerService.Erro("Erro ao calibrar espaço.", ex);
                Status = "Erro ao calibrar espaço: " + ex.Message;
            }
        }

        private void ExecutarNaUI(Action acao)
        {
            if (Application.Current == null)
            {
                acao();
                return;
            }

            if (Application.Current.Dispatcher.CheckAccess())
                acao();
            else
                Application.Current.Dispatcher.Invoke(acao);
        }

        private async void ConectarSignalR()
        {
            try
            {
                if (_encerrando) return;

                LoggerService.Info($"Tentando conectar ao SignalR em: {KinectConfig.UrlSignalR}");

                ExecutarNaUI(() =>
                {
                    StatusSignalR = "SignalR: Conectando";
                    MensagemEnvioAplicacao = "Tentando conectar ao MVC para envio externo.";
                });

                await _signalRService.ConectarAsync();
                await Task.Delay(1000);

                if (_signalRService.EstaConectado)
                {
                    _signalRConectado = true;

                    ExecutarNaUI(() =>
                    {
                        StatusSignalR = "SignalR: Conectado";
                        Status = "Conectado ao MVC via SignalR.";
                        MensagemEnvioAplicacao = "Comunicação com MVC ativa. Aguardando envio de medições.";
                    });

                    LoggerService.Info("Conexão SignalR estabelecida com sucesso.");
                    await _signalRService.EnviarStatusAsync("Aplicação Kinect conectada.");
                }
                else
                {
                    _signalRConectado = false;

                    ExecutarNaUI(() =>
                    {
                        StatusSignalR = "SignalR: Sem conexão";
                        Status = "Falha ao conectar SignalR.";
                        MensagemEnvioAplicacao = "Não foi possível conectar ao MVC. As medições serão mantidas apenas no SQLite local.";
                    });

                    LoggerService.Info($"Falha ao conectar. Estado atual: {_signalRService.EstadoConexao}");
                }
            }
            catch (Exception ex)
            {
                _signalRConectado = false;

                ExecutarNaUI(() =>
                {
                    StatusSignalR = "SignalR: Sem conexão";
                    Status = $"Erro ao conectar SignalR: {ex.Message}";
                    MensagemEnvioAplicacao = "Erro na comunicação externa. Medições não serão enviadas ao MVC.";
                });

                LoggerService.Erro("Erro ao conectar SignalR.", ex);
            }
        }

        private void AtualizarStatusSignalR(string msg)
        {
            if (_encerrando) return;

            ExecutarNaUI(() =>
            {
                StatusSignalR = msg;
                Status = msg;

                if (msg.Contains("Conectado"))
                    MensagemEnvioAplicacao = "Comunicação externa ativa com o MVC.";
                else if (msg.Contains("Sem conexão") || msg.Contains("Falha") || msg.Contains("Erro"))
                    MensagemEnvioAplicacao = "Sem comunicação externa. As medições permanecem no SQLite local.";
            });

            LoggerService.Info("Status SignalR atualizado: " + msg);
        }

        private void LigarKinect()
        {
            try
            {
                if (_encerrando) return;

                LoggerService.Info("Inicializando Kinect.");
                Status = "Inicializando Kinect...";
                StatusKinect = "Kinect: Inicializando";

                CriarEspacoMapeadoSePossivel();

                bool sucesso = _kinectService.InicializarKinect();

                if (sucesso)
                {
                    Status = "Kinect iniciado.";
                    StatusKinect = "Kinect: Conectado";
                    LoggerService.Info("Kinect iniciado com sucesso.");

                    if (_signalRConectado)
                    {
                        Task.Run(async () =>
                        {
                            try { await _signalRService.EnviarStatusAsync("Kinect iniciado."); }
                            catch (Exception ex) { LoggerService.Erro("Erro ao enviar status via SignalR.", ex); }
                        });
                    }
                }
                else
                {
                    Status = "Falha ao iniciar Kinect.";
                    StatusKinect = "Kinect: Erro ao iniciar";
                    LoggerService.Info("Falha ao iniciar Kinect.");
                }
            }
            catch (Exception ex)
            {
                LoggerService.Erro("Erro ao iniciar Kinect.", ex);
                StatusKinect = "Kinect: Erro";
                Status = "Erro ao iniciar Kinect: " + ex.Message;
            }
        }

        private void CriarEspacoMapeadoSePossivel()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(NomeEspaco))
                {
                    LoggerService.Info("Espaço não definido. Kinect será iniciado sem cadastro de espaço.");
                    return;
                }

                double volumeMaximoCm3 = 0;
                double percentualAlerta = 0;

                double.TryParse(VolumeMaximo, out volumeMaximoCm3);
                double.TryParse(PercentualAlerta, out percentualAlerta);

                var espaco = new EspacoMapeado
                {
                    NomeEspaco = NomeEspaco,
                    VolumeMaximoPermitidoCm3 = volumeMaximoCm3,
                    VolumeAtualCm3 = 0,
                    PercentualOcupacao = 0,
                    EspacoLivreCm3 = volumeMaximoCm3,
                    Ativo = true,
                    MapeamentoConcluido = false,
                    Status = "Mapeamento iniciado",
                    DataCriacao = DateTime.Now
                };

                _kinectService.DefinirEspaco(espaco);
                LoggerService.Info("Espaço enviado ao KinectService: " + NomeEspaco);
            }
            catch (Exception ex)
            {
                LoggerService.Erro("Erro ao criar espaço mapeado.", ex);
            }
        }

        private async void DesligarKinect()
        {
            await DesligarKinectAsync();
        }

        private async Task DesligarKinectAsync()
        {
            try
            {
                LoggerService.Info("Desligando Kinect.");
                _kinectService.DesligarKinect();

                ExecutarNaUI(() =>
                {
                    Status = "Kinect desligado.";
                    StatusKinect = "Kinect: Desconectado";
                    VolumeTexto = "Volume: 0 cm³";
                    CameraImage = null;
                    QuantidadePontos3D = "Pontos 3D: 0";
                    PercentualOcupacaoTexto = "Ocupação: 0%";
                    EspacoLivreTexto = "Espaço livre: 0 cm³";
                    UltimoSnapshot = "Snapshot: nenhum";
                    StatusMvcFirebase = "MVC/Firebase: Aguardando";
                    MensagemEnvioAplicacao = "Kinect desligado. Nenhuma medição será enviada para outra aplicação.";
                });

                if (_signalRConectado)
                {
                    await _signalRService.EnviarStatusAsync("Kinect desligado.");
                    await _signalRService.DesconectarAsync();
                    _signalRConectado = false;

                    ExecutarNaUI(() =>
                    {
                        StatusSignalR = "SignalR: Desconectado";
                        Status = "Kinect desligado e conexão com MVC encerrada.";
                        MensagemEnvioAplicacao = "Conexão externa encerrada. Sistema sem envio para o MVC.";
                    });
                }
            }
            catch (Exception ex)
            {
                LoggerService.Erro("Erro ao desligar Kinect.", ex);
                Status = "Erro ao desligar Kinect: " + ex.Message;
            }
        }

        private void CalibrarChao()
        {
            try
            {
                if (_encerrando) return;

                LoggerService.Info("Iniciando calibração do chão.");
                _kinectService.CalibrarChao();
                Status = "Chão calibrado.";
                LoggerService.Info("Chão calibrado com sucesso.");
            }
            catch (Exception ex)
            {
                LoggerService.Erro("Erro ao calibrar chão.", ex);
                Status = "Erro ao calibrar chão: " + ex.Message;
            }
        }

        private void AtualizarStatus(string msg)
        {
            if (_encerrando) return;
            ExecutarNaUI(() => Status = msg);
            LoggerService.Info("Status atualizado: " + msg);
        }

        private void AtualizarCamera(ImageSource imagem)
        {
            if (_encerrando) return;
            ExecutarNaUI(() => CameraImage = imagem);
        }

        private void AtualizarPointCloud(List<Point3DData> pontos)
        {
            try
            {
                if (_encerrando) return;
                ExecutarNaUI(() => QuantidadePontos3D = $"Pontos 3D: {pontos.Count}");
            }
            catch (Exception ex)
            {
                LoggerService.Erro("Erro ao atualizar Point Cloud na interface.", ex);
            }
        }

        private void AtualizarSnapshot(SnapshotEspacial snapshot)
        {
            try
            {
                if (_encerrando) return;

                ExecutarNaUI(() =>
                {
                    UltimoSnapshot = $"Snapshot: {snapshot.NomeSnapshot}";
                    PercentualOcupacaoTexto = $"Ocupação: {snapshot.PercentualOcupacao:F2}%";
                    EspacoLivreTexto = $"Espaço livre: {snapshot.EspacoLivreCm3:F0} cm³";
                });
            }
            catch (Exception ex)
            {
                LoggerService.Erro("Erro ao atualizar snapshot na interface.", ex);
            }
        }

        private void CarregarHistoricoMedicoes()
        {
            try
            {
                HistoricoMedicoes.Clear();
                var medicoes = _repository.ObterUltimasMedicoes(50);
                foreach (var medicao in medicoes)
                    HistoricoMedicoes.Add(medicao);

                LoggerService.Info("Histórico de medições carregado.");
            }
            catch (Exception ex)
            {
                LoggerService.Erro("Erro ao carregar histórico de medições.", ex);
            }
        }

        private void ProcessarNovaMedida(double volumeCm3)
        {
            if (_encerrando) return;

            ExecutarNaUI(() => VolumeTexto = $"Volume: {volumeCm3:F0} cm³");

            if (_volumeMaximoValor > 0)
            {
                double percentual = (volumeCm3 / _volumeMaximoValor) * 100;

                ExecutarNaUI(() =>
                {
                    PercentualOcupacaoTexto = $"Ocupação: {percentual:F1}%";

                    if (percentual >= _limitePercentual)
                    {
                        Status = $"ALERTA! Ocupação acima do limite ({_limitePercentual}%)";
                    }
                });
            }

            if (DateTime.Now >= _proximaGravacao)
            {
                _proximaGravacao = DateTime.Now.AddSeconds(KinectConfig.IntervaloSalvarSQLiteSegundos);

                var medicao = new MedicaoVolume
                {
                    DataHora = DateTime.Now,
                    VolumeCm3 = volumeCm3,
                    KinectLigado = true,
                    Calibrado = true,
                    Status = "AutoSave"
                };

                Task.Run(() =>
                {
                    try
                    {
                        if (_encerrando) return;

                        ExecutarNaUI(() => StatusSQLite = "SQLite: Salvando");
                        _repository.SalvarMedicao(medicao);

                        ExecutarNaUI(() =>
                        {
                            HistoricoMedicoes.Insert(0, medicao);
                            if (HistoricoMedicoes.Count > 50)
                                HistoricoMedicoes.RemoveAt(HistoricoMedicoes.Count - 1);

                            StatusSQLite = "SQLite: Salvo com sucesso";
                        });
                    }
                    catch (Exception ex)
                    {
                        LoggerService.Erro("Erro ao salvar medição no SQLite.", ex);
                        ExecutarNaUI(() => StatusSQLite = "SQLite: Erro ao salvar");
                    }
                });
            }

            if (_signalRService.EstaConectado && DateTime.Now >= _proximoEnvioSignalR)
            {
                _proximoEnvioSignalR = DateTime.Now.AddSeconds(KinectConfig.IntervaloEnvioSignalRSegundos);

                Task.Run(async () =>
                {
                    try
                    {
                        if (_encerrando) return;

                        ExecutarNaUI(() =>
                        {
                            StatusSignalR = "SignalR: Enviando";
                            StatusMvcFirebase = "MVC/Firebase: Enviando";
                            MensagemEnvioAplicacao = "Enviando medição para outra aplicação via SignalR.";
                        });

                        bool enviado = await _signalRService.EnviarVolumeAsync(volumeCm3);

                        if (enviado)
                        {
                            await _signalRService.EnviarStatusAsync("Volume enviado pelo Kinect.");
                            LoggerService.Info($"Volume enviado ao MVC via SignalR: {volumeCm3:F0} cm³");

                            ExecutarNaUI(() =>
                            {
                                StatusSignalR = "SignalR: Enviado";
                                StatusMvcFirebase = "MVC/Firebase: Enviado";
                                Status = $"Informações enviadas ao MVC via SignalR às {DateTime.Now:HH:mm:ss}.";
                                MensagemEnvioAplicacao = $"Última medição enviada ao MVC: {volumeCm3:F0} cm³ às {DateTime.Now:HH:mm:ss}.";
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        LoggerService.Erro("Erro ao enviar dados via SignalR.", ex);
                    }
                });
            }
        }

        public async Task EncerrarAplicacaoAsync()
        {
            try
            {
                _encerrando = true;
                LoggerService.Info("Encerrando aplicação.");

                ExecutarNaUI(() =>
                {
                    Status = "Encerrando aplicação...";
                    MensagemEnvioAplicacao = "Encerrando comunicação externa e finalizando aplicação.";
                });

                _kinectService.DesligarKinect();

                if (_signalRConectado)
                {
                    await _signalRService.DesconectarAsync();
                    _signalRConectado = false;
                }

                ExecutarNaUI(() =>
                {
                    StatusKinect = "Kinect: Desconectado";
                    StatusSignalR = "SignalR: Desconectado";
                    StatusMvcFirebase = "MVC/Firebase: Aguardando";
                    Status = "Aplicação encerrada.";
                    MensagemEnvioAplicacao = "Aplicação encerrada. Nenhuma comunicação externa ativa.";
                });

                LoggerService.Info("Aplicação encerrada com segurança.");
            }
            catch (Exception ex)
            {
                LoggerService.Erro("Erro ao encerrar aplicação.", ex);
            }
        }
    }
}
