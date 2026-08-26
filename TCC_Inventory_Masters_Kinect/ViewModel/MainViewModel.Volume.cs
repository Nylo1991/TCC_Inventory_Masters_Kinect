using System;
using System.Threading.Tasks;
using System.Windows.Threading;
using TCC_Inventory_Masters_Kinect.Logs;
using TCC_Inventory_Masters_Kinect.Model;

namespace TCC_Inventory_Masters_Kinect.ViewModel
{
    public partial class MainViewModel
    {
        /// <summary>
        /// Inicia o timer de medição automática.
        /// </summary>
        private void IniciarTimerVolume()
        {
            _volumeTimer?.Stop();

            _volumeTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(60)
            };

            _volumeTimer.Tick += async (s, e) =>
            {
                await MedirSalvarEEnviarAsync("Medição automática");
            };

            _volumeTimer.Start();

            LoggerService.Info("Timer de medição automática iniciado.");
        }

        /// <summary>
        /// Executa uma medição manual.
        /// </summary>
        private async Task ExecutarMedicaoAsync()
        {
            await MedirSalvarEEnviarAsync("Medição manual");
        }

        /// <summary>
        /// Mede o volume, salva no banco SQLite e envia ao MVC via SignalR.
        /// </summary>
        internal async Task MedirSalvarEEnviarAsync(string statusMedicao)
        {
            try
            {
                if (!_kinectService.IsConnected)
                {
                    StatusMessage = "Kinect não está conectado.";
                    LoggerService.LogWarning("Tentativa de medição com Kinect desconectado.");
                    return;
                }

                if (_volumeMaximoCm3 <= 0)
                {
                    StatusMessage = "Calibre o espaço antes de medir.";
                    LoggerService.LogWarning("Tentativa de medição sem calibração.");
                    return;
                }

                if (!EspacoSalvo)
                {
                    StatusMessage = "Salve o espaço antes de medir.";
                    LoggerService.LogWarning("Tentativa de medição antes de salvar o espaço.");
                    return;
                }

                double volumeAtualCm3 = _kinectService.CalcularVolumeAtualCm3();

                if (volumeAtualCm3 <= 0)
                {
                    StatusMessage = "Nenhum volume detectado.";
                    LoggerService.LogWarning("Nenhum volume detectado na medição.");
                    return;
                }

                _ultimoVolumeAtual = volumeAtualCm3;

                AtualizarIndicadoresVolume(volumeAtualCm3);

                double limiteOcupacao = 0;

                if (!string.IsNullOrWhiteSpace(PercentualAlerta))
                {
                    double.TryParse(PercentualAlerta, out limiteOcupacao);
                }

                var medicao = new MedicaoVolume
                {
                    VolumeCm3 = volumeAtualCm3,
                    DataHora = DateTime.Now,
                    KinectLigado = _kinectService.IsConnected,
                    Calibrado = true,
                    Status = statusMedicao,
                    Usuario = _sessao.Usuario,
                    Empresa = _sessao.Empresa,
                    NomeEspaco = NomeEspaco,
                    LimiteOcupacaoPercentual = limiteOcupacao
                };

                _repository.SalvarMedicao(medicao);

                CarregarHistoricoMedicoes();

                StatusSQLite = $"SQLite: medicao salva. Historico: {HistoricoMedicoes.Count}";
                StatusMessage = $"Medido: {FormatarVolumeM3(volumeAtualCm3)}";

                if (_signalRService.EstaConectado)
                {
                    await _signalRService.EnviarVolumeAsync(volumeAtualCm3);
                    MensagemEnvioAplicacao = $"Volume enviado: {FormatarVolumeM3(volumeAtualCm3)}";
                }
                else
                {
                    MensagemEnvioAplicacao = "SignalR não está conectado.";
                }

                LoggerService.Info($"{statusMedicao}. Usuário: {_sessao.Usuario}. Empresa: {_sessao.Empresa}. Volume: {volumeAtualCm3:F0} cm3");
            }
            catch
            {
                StatusMessage = "Erro na medição";
                MensagemEnvioAplicacao = "Erro na medição";
                LoggerService.Erro("Erro na medição pela MainViewModel.");
            }
        }

        /// <summary>
        /// Atualiza os indicadores de volume exibidos na interface.
        /// </summary>
        internal void AtualizarIndicadoresVolume(double volumeAtualCm3)
        {
            VolumeTexto = FormatarVolumeM3(volumeAtualCm3);

            if (_volumeMaximoCm3 <= 0)
            {
                PercentualOcupacaoTexto = "0%";
                EspacoLivreTexto = "0.000 m3";
                StatusAlertaTexto = "Normal";
                return;
            }

            double percentual = (volumeAtualCm3 / _volumeMaximoCm3) * 100.0;
            percentual = Math.Max(0, Math.Min(100, percentual));

            double espacoLivreCm3 = _volumeMaximoCm3 - volumeAtualCm3;
            espacoLivreCm3 = Math.Max(0, espacoLivreCm3);

            PercentualOcupacaoTexto = $"{percentual:F1}%";
            EspacoLivreTexto = FormatarVolumeM3(espacoLivreCm3);

            double limite = 0;

            if (!string.IsNullOrWhiteSpace(PercentualAlerta))
            {
                double.TryParse(PercentualAlerta, out limite);
            }

            StatusAlertaTexto = limite > 0 && percentual >= limite
                ? "Limite"
                : "Normal";
        }

        /// <summary>
        /// Converte volume de centímetros cúbicos para metros cúbicos.
        /// </summary>
        internal static string FormatarVolumeM3(double volumeCm3)
        {
            double volumeM3 = volumeCm3 / 1000000.0;
            return $"{volumeM3:F3} m3";
        }
    }
}
