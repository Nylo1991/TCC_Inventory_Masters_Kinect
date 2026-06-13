using System;
using System.Windows;
using System.Windows.Threading;
using TCC_Inventory_Masters_Kinect.ViewModel;

namespace TCC_Inventory_Masters_Kinect.View
{
    public partial class HistoricoMedicoesWindow : Window
    {
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

        private void Fechar_Click(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            _timer.Stop();
            base.OnClosed(e);
        }
    }
}