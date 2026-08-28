using TCC_Inventory_Masters_Kinect.Logs;

namespace TCC_Inventory_Masters_Kinect.ViewModel
{
    public partial class MainViewModel
    {
        /// <summary>
        /// Valida e salva os dados do espaço monitorado.
        /// </summary>
        private void SalvarEspaco()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(NomeEspaco))
                {
                    MensagemEspaco = "Informe o nome do espaço.";
                    LoggerService.LogWarning("Tentativa de salvar espaço sem nome.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(PercentualAlerta))
                {
                    MensagemEspaco = "Informe o limite de ocupação.";
                    LoggerService.LogWarning("Tentativa de salvar espaço sem limite de ocupação.");
                    return;
                }

                double limiteOcupacao;

                if (!double.TryParse(PercentualAlerta, out limiteOcupacao))
                {
                    MensagemEspaco = "Informe um limite de ocupação válido.";
                    LoggerService.LogWarning("Tentativa de salvar espaço com limite inválido.");
                    return;
                }

                if (limiteOcupacao <= 0 || limiteOcupacao > 100)
                {
                    MensagemEspaco = "O limite de ocupação deve estar entre 1% e 100%.";
                    LoggerService.LogWarning("Tentativa de salvar espaço com limite fora do permitido.");
                    return;
                }

                if (_volumeMaximoCm3 <= 0)
                {
                    MensagemEspaco = "Calibre o espaço antes de salvar.";
                    LoggerService.LogWarning("Tentativa de salvar espaço sem calibração.");
                    return;
                }

                EspacoSalvo = true;
                MensagemEspaco = "Espaço salvo. Histórico e medição automática liberados.";
                StatusMessage = "Espaço salvo com sucesso.";

                IniciarTimerVolume();

                LoggerService.Info($"Espaço salvo: {NomeEspaco}");
            }
            catch
            {
                MensagemEspaco = "Erro ao salvar os dados do espaço.";
                StatusMessage = "Erro ao salvar espaço.";
                LoggerService.Erro("Erro ao salvar espaço na MainViewModel.");
            }
        }
    }
}
