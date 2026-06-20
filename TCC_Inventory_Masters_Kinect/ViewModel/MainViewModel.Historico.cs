using System.Collections.ObjectModel;
using TCC_Inventory_Masters_Kinect.Logs;
using TCC_Inventory_Masters_Kinect.Model;

namespace TCC_Inventory_Masters_Kinect.ViewModel
{
    public partial class MainViewModel
    {
        /// <summary>
        /// Carrega o histórico das últimas medições salvas no banco SQLite.
        /// </summary>
        public void CarregarHistoricoMedicoes()
        {
            try
            {
                var medicoes = _repository.ObterMedicoesEmOrdemCrescente(
                    100,
                    _sessao.Usuario,
                    _sessao.Empresa
                );

                HistoricoMedicoes = new ObservableCollection<MedicaoVolume>(medicoes);
            }
            catch
            {
                HistoricoMedicoes = new ObservableCollection<MedicaoVolume>();
                LoggerService.Erro("Erro ao carregar histórico de medições na MainViewModel.");
            }
        }
    }
}