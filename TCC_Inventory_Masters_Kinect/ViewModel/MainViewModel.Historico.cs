using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using TCC_Inventory_Masters_Kinect.Logs;
using TCC_Inventory_Masters_Kinect.Model;
using TCC_Inventory_Masters_Kinect.View;

namespace TCC_Inventory_Masters_Kinect.ViewModel
{
    public partial class MainViewModel
    {
        private DispatcherTimer _historicoTimer;

        /// <summary>
        /// Inicia a atualização automática enquanto a janela de histórico estiver aberta.
        /// </summary>
        private void IniciarAtualizacaoHistorico()
        {
            PararAtualizacaoHistorico();
            CarregarHistoricoMedicoes();

            _historicoTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(2)
            };
            _historicoTimer.Tick += HistoricoTimerTick;
            _historicoTimer.Start();
        }

        /// <summary>
        /// Interrompe a atualização ao fechar a janela de histórico.
        /// </summary>
        private void PararAtualizacaoHistorico()
        {
            if (_historicoTimer == null)
            {
                return;
            }

            _historicoTimer.Stop();
            _historicoTimer.Tick -= HistoricoTimerTick;
            _historicoTimer = null;
        }

        private void HistoricoTimerTick(object sender, EventArgs e)
        {
            CarregarHistoricoMedicoes();
        }

        private void FecharHistorico()
        {
            PararAtualizacaoHistorico();

            var janela = Application.Current.Windows
                .OfType<HistoricoMedicoesWindow>()
                .FirstOrDefault(item => ReferenceEquals(item.DataContext, this));

            janela?.Close();
        }

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

                StatusSQLite = $"SQLite: {HistoricoMedicoes.Count} medicoes carregadas";

                LoggerService.Info(
                    $"Historico carregado. Usuario: {_sessao.Usuario}. Empresa: {_sessao.Empresa}. Total: {HistoricoMedicoes.Count}"
                );
            }
            catch
            {
                HistoricoMedicoes = new ObservableCollection<MedicaoVolume>();
                StatusSQLite = "SQLite: erro ao carregar historico";
                LoggerService.Erro("Erro ao carregar histórico de medições na MainViewModel.");
            }
        }
    }
}
