using System;
using System.Windows;
using System.Windows.Threading;
using TCC_Inventory_Masters_Kinect.ViewModel;

namespace TCC_Inventory_Masters_Kinect.View
{
    public partial class HistoricoMedicoesWindow : Window
    {
        /// <summary>
        /// classe responsave por exixbir o historico de medição em tempo real .
        /// </summary>
        private readonly MainViewModel _mainViewModel;
        private readonly DispatcherTimer _timer;

        public HistoricoMedicoesWindow(MainViewModel mainViewModel)
        {
            InitializeComponent();

            _mainViewModel = mainViewModel;
            DataContext = _mainViewModel;

            _mainViewModel.CarregarHistoricoMedicoes();

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(2)
            };

            _timer.Tick += (s, e) =>
            {
                _mainViewModel.CarregarHistoricoMedicoes();
            };

            _timer.Start();
        }
        /// <summary>
        /// método de fechar page de historico de mediação.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Fechar_Click(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            Close();
        }

        /// <summary>
        /// método de onclosed foi implmentando  para que  para que o sistema não busque dados
        /// desnecessarios ,assegurando o ciclo de vida da tela, ao realizar o  fechamento a tela o sistema da stop 
        /// e para de buscar dados  não sobrecarregando a memoria ram .
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            _timer.Stop();
            base.OnClosed(e);
        }
    }
}