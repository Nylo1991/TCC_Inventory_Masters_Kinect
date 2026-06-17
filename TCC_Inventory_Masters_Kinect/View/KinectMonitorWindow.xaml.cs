using System;
using System.ComponentModel;
using System.Windows;
using TCC_Inventory_Masters_Kinect.Model;
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
            : this(new SessaoUsuario
            {
                Usuario = "Administrador",
                Empresa = "Empresa Teste",
                Email = "teste@inventorymasters.com",
                Token = "DEV"
            })
        {
        }

        /// <summary>
        /// Construtor da janela principal do monitoramento do Kinect,
        /// que recebe a sessão do usuário validada pelo MVC ou pelo modo de desenvolvimento.
        /// </summary>
        /// <param name="sessao"></param>
        public KinectMonitorWindow(SessaoUsuario sessao)
        {
            InitializeComponent();

            /// separação da lógica de calibração e monitoramento em um ViewModel
            /// dedicado a busca de dados dentro da mainviewmodel

            _viewModel = new MainViewModel(sessao);
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

            PainelCalibracao.Visibility = Visibility.Visible;

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
        /// Método que é chamado quando a calibração é finalizada.
        /// </summary>
        private void FinalizarVideoCalibracao()
        {
            Dispatcher.Invoke(() =>
            {
                CalibrationVideoElement.Stop();
                CalibrationVideoElement.Visibility = Visibility.Hidden;
                PainelCalibracao.Visibility = Visibility.Collapsed;

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
                ExibirAvisoHistoricoIndisponivel();
                return;
            }

            var janela = new HistoricoMedicoesWindow(viewModel)
            {
                Owner = this
            };

            janela.ShowDialog();
        }

        /// <summary>
        /// Metado de mostra que antes de abrir o hstorico e necessario registra o espaço antes 
        /// </summary>
        private void ExibirAvisoHistoricoIndisponivel()
        {
            var aviso = new Window
            {
                Title = "Historico indisponivel",
                Width = 460,
                Height = 250,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this,
                ResizeMode = ResizeMode.NoResize,
                Background = System.Windows.Media.Brushes.White
            };

            var painel = new System.Windows.Controls.Grid
            {
                Margin = new Thickness(24)
            };

            painel.RowDefinitions.Add(new System.Windows.Controls.RowDefinition
            {
                Height = GridLength.Auto
            });

            painel.RowDefinitions.Add(new System.Windows.Controls.RowDefinition
            {
                Height = new GridLength(1, GridUnitType.Star)
            });

            painel.RowDefinitions.Add(new System.Windows.Controls.RowDefinition
            {
                Height = GridLength.Auto
            });

            var titulo = new System.Windows.Controls.TextBlock
            {
                Text = "Historico indisponivel",
                Foreground = new System.Windows.Media.SolidColorBrush(
                    System.Windows.Media.Color.FromRgb(17, 17, 17)),
                FontSize = 24,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 14)
            };

            var mensagem = new System.Windows.Controls.TextBlock
            {
                Text = "Salve os dados do espaco antes de abrir o historico de medicoes.",
                Foreground = new System.Windows.Media.SolidColorBrush(
                    System.Windows.Media.Color.FromRgb(75, 85, 99)),
                FontSize = 16,
                TextWrapping = TextWrapping.Wrap,
                VerticalAlignment = VerticalAlignment.Center
            };

            var botao = new System.Windows.Controls.Button
            {
                Content = "Entendi",
                Width = 110,
                Height = 38,
                HorizontalAlignment = HorizontalAlignment.Right,
                Background = new System.Windows.Media.SolidColorBrush(
                    System.Windows.Media.Color.FromRgb(16, 138, 59)),
                Foreground = System.Windows.Media.Brushes.White,
                FontWeight = FontWeights.Bold,
                BorderThickness = new Thickness(0),
                Cursor = System.Windows.Input.Cursors.Hand
            };

            botao.Click += (s, e) => aviso.Close();

            System.Windows.Controls.Grid.SetRow(titulo, 0);
            System.Windows.Controls.Grid.SetRow(mensagem, 1);
            System.Windows.Controls.Grid.SetRow(botao, 2);

            painel.Children.Add(titulo);
            painel.Children.Add(mensagem);
            painel.Children.Add(botao);

            aviso.Content = painel;
            aviso.ShowDialog();
        }
    }
}