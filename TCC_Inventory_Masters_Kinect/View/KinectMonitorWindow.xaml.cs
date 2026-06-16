using System;
using System.ComponentModel;
using System.Windows;
using TCC_Inventory_Masters_Kinect.ViewModel;

namespace TCC_Inventory_Masters_Kinect.View
{
    public partial class KinectMonitorWindow : Window
    {
        /// <summary>
        /// Janela principal do monitoramento do Kinect, responsável por exibir a interface de calibração,
        /// exibir o vídeo de calibração e gerenciar a interação do usuário com o sistema.
        /// </summary>
        private MainViewModel _viewModel;

        public KinectMonitorWindow()
            : this("Administrador")
        {
        }

        /// <summary>
        /// Construtor da janela principal do monitoramento do Kinect, 
        /// que recebe o nome do usuário logado para personalizar a experiência.
        /// </summary>
        /// <param name="usuarioLogado"></param>
        public KinectMonitorWindow(string usuarioLogado)
        {
            InitializeComponent();

            /// separação da lógica de calibração e monitoramento em um ViewModel 
            /// dedicado a busca de dados dentro da mainviewmodel 

            _viewModel = new MainViewModel(usuarioLogado);
            _viewModel.CalibracaoFinalizada += FinalizarVideoCalibracao;

            DataContext = _viewModel;
        }

        /// <summary>
        /// Evento de clique do botão "Calibrar", que inicia o processo de calibração do Kinect.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CalibrarButton_Click(object sender, RoutedEventArgs e)
        {
            CalibrationTitleTextBlock.Text = "Calibracao em andamento";
            CalibrationSubtitleTextBlock.Text = "Aguarde enquanto o Kinect calibra o espaco vazio";

            CalibrationVideoElement.Visibility = Visibility.Visible;
            CalibrationVideoElement.Position = TimeSpan.Zero;
            CalibrationVideoElement.Play();
        }

        /// <summary>
        /// Evento que é acionado quando o vídeo de calibração chega ao fim, 
        /// reiniciando a reprodução para criar um loop contínuo.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CalibrationVideoElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            CalibrationVideoElement.Position = TimeSpan.Zero;
            CalibrationVideoElement.Play();
        }

        /// <summary>
        /// Método que é chamado quando a calibração é finalizada,
        /// </summary>
        private void FinalizarVideoCalibracao()
        {
            Dispatcher.Invoke(() =>
            {
                CalibrationVideoElement.Stop();
                CalibrationVideoElement.Visibility = Visibility.Hidden;

                CalibrationTitleTextBlock.Text = "Calibracao concluida";
                CalibrationSubtitleTextBlock.Text = "Salve o espaco para liberar as medicoes automaticas";
            });
        }

        /// <summary>
        /// Evento de clique do botão "Sair", que fecha a aplicação de forma segura, 
        /// garantindo que todos os recursos sejam liberados corretamente.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SairButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void AbrirMonitoramento_Click(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>
        /// Evento de fechamento da janela, que garante que os recursos do Kinect sejam liberados corretamente
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KinectWindow_Closing(object sender, CancelEventArgs e)
        {
            if (_viewModel != null)
            {
                _viewModel.CalibracaoFinalizada -= FinalizarVideoCalibracao;
            }
        }

        /// <summary>
        /// Evento de clique do botão "Abrir Histórico", que verifica se os dados do espaço foram salvos
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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