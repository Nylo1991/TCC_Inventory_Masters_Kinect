using System;
using System.ComponentModel;
using System.Windows;
using TCC_Inventory_Masters_Kinect.ViewModel;

namespace TCC_Inventory_Masters_Kinect.View
{
    public partial class KinectMonitorWindow : Window
    {
        public KinectMonitorWindow()
            : this("Administrador")
        {
        }

        public KinectMonitorWindow(string usuarioLogado)
        {
            InitializeComponent();
            DataContext = new MainViewModel(usuarioLogado);
        }

        private void CalibrarButton_Click(object sender, RoutedEventArgs e)
        {
            CalibrationVideoElement.Visibility = Visibility.Visible;
            CalibrationVideoElement.Position = TimeSpan.Zero;
            CalibrationVideoElement.Play();
        }

        private void SairButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void KinectWindow_Closing(object sender, CancelEventArgs e)
        {
        }

        private void AbrirHistorico_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as MainViewModel;

            if (viewModel == null || !viewModel.EspacoSalvo)
            {
                MessageBox.Show(
                    "Salve os dados do espaco antes de abrir o historico.",
                    "Historico indisponivel",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                return;
            }

            var janela = new HistoricoMedicoesWindow(viewModel)
            {
                Owner = this
            };

            janela.ShowDialog();
        }
    }
}