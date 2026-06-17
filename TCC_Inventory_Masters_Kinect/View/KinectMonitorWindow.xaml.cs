using System;
using System.ComponentModel;
using System.Windows;
using TCC_Inventory_Masters_Kinect.ViewModel;

namespace TCC_Inventory_Masters_Kinect.View
{
    public partial class KinectMonitorWindow : Window
    {
        private MainViewModel _viewModel;

        public KinectMonitorWindow()
            : this("Administrador")
        {
        }

        public KinectMonitorWindow(string usuarioLogado)
        {
            InitializeComponent();

            _viewModel = new MainViewModel(usuarioLogado);
            _viewModel.CalibracaoFinalizada += FinalizarVideoCalibracao;

            DataContext = _viewModel;
        }

        private void CalibrarButton_Click(object sender, RoutedEventArgs e)
        {
            CalibrationTitleTextBlock.Text = "Calibracao em andamento";
            CalibrationSubtitleTextBlock.Text = "Aguarde enquanto o Kinect calibra o espaco vazio";

            _viewModel.StatusMessage = "CALIBRANDO AMBIENTE...";
            _viewModel.StatusCalibracao = "Capturando referencia do espaco vazio";

            PainelCalibracao.Visibility = Visibility.Visible;
            CalibrationVideoElement.Visibility = Visibility.Visible;
            CalibrationVideoElement.Position = TimeSpan.Zero;
            CalibrationVideoElement.Play();
        }

        private void CalibrationVideoElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            CalibrationVideoElement.Position = TimeSpan.Zero;
            CalibrationVideoElement.Play();
        }

        private void FinalizarVideoCalibracao()
        {
            Dispatcher.Invoke(() =>
            {
                CalibrationVideoElement.Stop();
                CalibrationVideoElement.Visibility = Visibility.Hidden;
                PainelCalibracao.Visibility = Visibility.Collapsed;

                CalibrationTitleTextBlock.Text = "Calibracao concluida";
                CalibrationSubtitleTextBlock.Text = "Salve o espaco para liberar as medicoes automaticas";

                _viewModel.StatusMessage = "CALIBRACAO CONCLUIDA";
                _viewModel.StatusCalibracao = "Espaco pronto para monitoramento";
            });
        }

        private void SairButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void KinectWindow_Closing(object sender, CancelEventArgs e)
        {
            if (_viewModel != null)
            {
                _viewModel.CalibracaoFinalizada -= FinalizarVideoCalibracao;
            }
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