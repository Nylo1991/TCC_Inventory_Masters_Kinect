using System;
using System.ComponentModel;
using System.Windows;
using TCC_Inventory_Masters_Kinect.Model;
using TCC_Inventory_Masters_Kinect.ViewModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;
using TCC_Inventory_Masters_Kinect.Data;
using TCC_Inventory_Masters_Kinect.Logs;

namespace TCC_Inventory_Masters_Kinect.View
{
    public partial class KinectMonitorWindow : Window
    {
        /// <summary>
        /// Janela principal do monitoramento do Kinect, responsável por exibir a interface de calibração,
        /// exibir o vídeo de calibração e gerenciar a interação do usuário com o sistema.
        /// </summary>
        private MainViewModel _viewModel;

        private readonly DispatcherTimer _inatividadeTimer;
        private readonly TimeSpan _tempoLimiteInatividade = TimeSpan.FromMinutes(20);
        private bool _sessaoBloqueada;

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

            ///Metado de bloqueio automatico da tela apos 15 minutos de inatividade, 
            ///para evitar o uso indevido do sistema em caso de esquecimento ou abandono da estação de trabalho

            _inatividadeTimer = new DispatcherTimer
            {
                Interval = _tempoLimiteInatividade
            };

            _inatividadeTimer.Tick += InatividadeTimer_Tick;
            _inatividadeTimer.Start();

            PreviewMouseMove += RegistrarAtividadeUsuario;
            PreviewMouseDown += RegistrarAtividadeUsuario;
            PreviewKeyDown += RegistrarAtividadeUsuario;

        }

        /// <summary>
        /// Evento que é acionado sempre que o usuário interage com a interface, seja movendo o mouse, clicando ou pressionando uma tecla,
        /// para registrar a atividade do usuário e reiniciar o timer de inatividade, 
        /// garantindo que a sessão permaneça ativa enquanto o usuário estiver presente.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RegistrarAtividadeUsuario(object sender, EventArgs e)
        {
            if (_sessaoBloqueada)
            {
                return;
            }

            ReiniciarTimerInatividade();
        }

        private void ReiniciarTimerInatividade()
        {
            _inatividadeTimer.Stop();
            _inatividadeTimer.Start();
        }

        private void InatividadeTimer_Tick(object sender, EventArgs e)
        {
            _inatividadeTimer.Stop();
            BloquearSessaoPorInatividade();
        }

        private void BloquearSessaoPorInatividade()
        {
            if (_sessaoBloqueada)
            {
                return;
            }

            _sessaoBloqueada = true;

            TelaBloqueioInatividade.Visibility = Visibility.Visible;
            SenhaDesbloqueioPasswordBox.Password = string.Empty;
            MensagemBloqueioTextBlock.Text = string.Empty;
            SenhaDesbloqueioPasswordBox.Focus();

            LoggerService.LogWarning("Sessao bloqueada por inatividade. Monitoramento Kinect continua ativo.");
        }

        private void DesbloquearSessao_Click(object sender, RoutedEventArgs e)
        {
            DesbloquearSessao();
        }

        private void SenhaDesbloqueioPasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                DesbloquearSessao();
            }
        }
        /// <summary>
        /// metado que realiza o processo de desbloqueio da sessão quando o usuário informa a senha correta,
        /// e volta a tela apos o bloqueio por inatividade , garantindo que apenas usuários autorizados possam 
        /// acessar a interface após um período de inatividade,
        /// </summary>
        private void DesbloquearSessao()
        {
            try
            {
                string senha = SenhaDesbloqueioPasswordBox.Password?.Trim();

                if (string.IsNullOrWhiteSpace(senha))
                {
                    MensagemBloqueioTextBlock.Text = "Informe a senha para desbloquear.";
                    return;
                }

                using (var db = new AppDbContext())
                {
                    var usuario = db.UsuariosAcesso.FirstOrDefault(x =>
                        x.Usuario == _viewModel.UsuarioLogado &&
                        x.Senha == senha &&
                        x.Ativo);

                    if (usuario == null)
                    {
                        MensagemBloqueioTextBlock.Text = "Senha invalida.";
                        LoggerService.LogWarning("Tentativa invalida de desbloqueio por inatividade.");
                        return;
                    }
                }

                _sessaoBloqueada = false;

                TelaBloqueioInatividade.Visibility = Visibility.Collapsed;
                SenhaDesbloqueioPasswordBox.Password = string.Empty;
                MensagemBloqueioTextBlock.Text = string.Empty;

                ReiniciarTimerInatividade();

                LoggerService.Info("Sessao desbloqueada apos inatividade.");
            }
            catch
            {
                MensagemBloqueioTextBlock.Text = "Erro ao desbloquear sessao.";
                LoggerService.Erro("Erro ao desbloquear sessao por inatividade.");
            }
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
        /// garantindo que todos os recursos sejam liberados corretamente  e retornando a tela de login .
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SairButton_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel != null)
            {
                _viewModel.DesligarMonitoramento();
            }

            var login = new KinectLogin();
            login.Show();

            Close();
        }

        private void AbrirMonitoramento_Click(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>
        /// Evento de fechamento da janela, que garante que os recursos do Kinect sejam liberados corretamente e 
        /// que os eventos sejam desvinculados para evitar vazamentos de memória,e aciona a tela novamente apos o usuario fechar a janela de monitoramento, 
        /// para permitir que o usuário possa realizar novas calibrações ou acessar o histórico de medições sem precisar reiniciar a aplicação.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KinectWindow_Closing(object sender, CancelEventArgs e)
        {
            if (_viewModel != null)
            {
                _viewModel.CalibracaoFinalizada -= FinalizarVideoCalibracao;
            }

            if (_inatividadeTimer != null)
            {
                _inatividadeTimer.Stop();
                _inatividadeTimer.Tick -= InatividadeTimer_Tick;
            }

            PreviewMouseMove -= RegistrarAtividadeUsuario;
            PreviewMouseDown -= RegistrarAtividadeUsuario;
            PreviewKeyDown -= RegistrarAtividadeUsuario;
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