using System;
using System.ComponentModel;
using System.Windows;
using TCC_Inventory_Masters_Kinect.Model;
using TCC_Inventory_Masters_Kinect.ViewModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;
using TCC_Inventory_Masters_Kinect.Logs;
using TCC_Inventory_Masters_Kinect.Service;

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
        private readonly SessaoUsuario _sessao;
        private bool _sessaoBloqueada;

        /// <summary>
        /// Construtor da janela principal do monitoramento do Kinect,
        /// que recebe exclusivamente a sessao do usuario validada pelo MVC.
        /// </summary>
        /// <param name="sessao"></param>
        public KinectMonitorWindow(SessaoUsuario sessao)
        {
            if (sessao == null ||
                string.IsNullOrWhiteSpace(sessao.Token) ||
                string.Equals(sessao.Token, "DEV", StringComparison.OrdinalIgnoreCase))
            {
                LoggerService.LogWarning("Abertura do monitor bloqueada por sessao invalida.");
                throw new InvalidOperationException(
                    "O monitor do Kinect exige autenticacao valida pelo MVC.");
            }

            InitializeComponent();

            _sessao = sessao;

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
        /// <summary>
        /// Método que reinicia o timer de inatividade, garantindo que a contagem de 
        /// tempo seja resetada sempre que o usuário interagir com a interface,
        /// mantendo a sessão ativa enquanto o usuário estiver presente.
        /// </summary>
        private void ReiniciarTimerInatividade()
        {
            _inatividadeTimer.Stop();
            _inatividadeTimer.Start();
        }
        /// <summary>
        /// Evento que é acionado quando o timer de inatividade atinge o tempo limite definido,
        /// bloqueando a sessão do usuário e exibindo a tela de bloqueio.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InatividadeTimer_Tick(object sender, EventArgs e)
        {
            _inatividadeTimer.Stop();
            BloquearSessaoPorInatividade();
        }
        /// <summary>
        /// Método que bloqueia a sessão do usuário após um período de inatividade,
        /// exibindo uma tela de bloqueio e solicitando a senha para desbloqueio,
        /// garantindo que apenas usuários autorizados possam acessar a interface após um período de inatividade.
        /// </summary>
        private void BloquearSessaoPorInatividade()
        {
            if (_sessaoBloqueada)
            {
                return;
            }

            _sessaoBloqueada = true;

            TelaBloqueioInatividade.Visibility = Visibility.Visible;
            EmailSessaoTextBlock.Text = _sessao.Email;
            TokenDesbloqueioPasswordBox.Password = string.Empty;
            MensagemBloqueioTextBlock.Text =
                "Solicite um novo token para desbloquear esta sessao.";
            TokenDesbloqueioPasswordBox.Focus();

            LoggerService.LogWarning("Sessao bloqueada por inatividade. Monitoramento Kinect continua ativo.");
        }
        /// <summary>
        /// Evento de clique do botão "Desbloquear Sessão", que verifica a senha informada pelo usuário e, 
        /// se correta, desbloqueia a sessão e retorna à tela principal do monitoramento.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void DesbloquearSessao_Click(object sender, RoutedEventArgs e)
        {
            await DesbloquearSessaoAsync();
        }

        /// <summary>
        /// Evento que é acionado quando o usuário pressiona a tecla Enter no campo de senha,
        /// realizando o processo de desbloqueio da sessão.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void TokenDesbloqueioPasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                await DesbloquearSessaoAsync();
            }
        }

        /// <summary>
        /// Solicita ao MVC um novo token para o e-mail da sessao bloqueada.
        /// </summary>
        private async void SolicitarNovoToken_Click(object sender, RoutedEventArgs e)
        {
            SolicitarNovoTokenButton.IsEnabled = false;
            MensagemBloqueioTextBlock.Text = "Solicitando novo token...";

            try
            {
                var autenticacaoService = new AutenticacaoMvcService();
                var resultado = await autenticacaoService.SolicitarTokenAsync(_sessao.Email);

                MensagemBloqueioTextBlock.Text = resultado != null && resultado.Sucesso
                    ? "Token enviado. Consulte seu e-mail e informe o codigo recebido."
                    : resultado?.Mensagem ?? "Nao foi possivel solicitar um novo token.";

                if (resultado != null && resultado.Sucesso)
                {
                    TokenDesbloqueioPasswordBox.Focus();
                    LoggerService.Info("Novo token solicitado para desbloqueio por inatividade.");
                }
            }
            catch (Exception ex)
            {
                MensagemBloqueioTextBlock.Text = "Erro ao solicitar o token de desbloqueio.";
                LoggerService.Erro("Erro ao solicitar token de desbloqueio: " + ex.Message);
            }
            finally
            {
                SolicitarNovoTokenButton.IsEnabled = true;
            }
        }
        /// <summary>
        /// metado que realiza o processo de desbloqueio da sessão quando o usuário informa a senha correta,
        /// e volta a tela apos o bloqueio por inatividade , garantindo que apenas usuários autorizados possam 
        /// acessar a interface após um período de inatividade,
        /// </summary>
        private async System.Threading.Tasks.Task DesbloquearSessaoAsync()
        {
            DesbloquearButton.IsEnabled = false;

            try
            {
                string token = TokenDesbloqueioPasswordBox.Password?.Trim();

                if (string.IsNullOrWhiteSpace(token))
                {
                    MensagemBloqueioTextBlock.Text = "Informe o token para desbloquear.";
                    return;
                }

                var autenticacaoService = new AutenticacaoMvcService();
                var resultado = await autenticacaoService.ValidarTokenAsync(token);

                bool mesmaSessao = resultado != null &&
                    resultado.TokenValido &&
                    string.Equals(resultado.Email, _sessao.Email, StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(resultado.Empresa, _sessao.Empresa, StringComparison.OrdinalIgnoreCase);

                if (!mesmaSessao)
                {
                    MensagemBloqueioTextBlock.Text = resultado?.Mensagem ??
                        "Token invalido, expirado ou pertencente a outro usuario.";
                    LoggerService.LogWarning("Tentativa invalida de desbloqueio por token.");
                    return;
                }

                _sessaoBloqueada = false;

                TelaBloqueioInatividade.Visibility = Visibility.Collapsed;
                TokenDesbloqueioPasswordBox.Password = string.Empty;
                MensagemBloqueioTextBlock.Text = string.Empty;

                ReiniciarTimerInatividade();

                LoggerService.Info("Sessao desbloqueada apos inatividade.");
            }
            catch (Exception ex)
            {
                MensagemBloqueioTextBlock.Text = "Erro ao desbloquear sessao.";
                LoggerService.Erro("Erro ao desbloquear sessao por inatividade: " + ex.Message);
            }
            finally
            {
                DesbloquearButton.IsEnabled = true;
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
        /// <summary>
        /// Evento de clique do botão "Abrir Monitoramento", que inicia o processo de monitoramento do Kinect.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
