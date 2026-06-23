using System.Windows;
using TCC_Inventory_Masters_Kinect.Logs;
using TCC_Inventory_Masters_Kinect.Model;
using TCC_Inventory_Masters_Kinect.Service;

namespace TCC_Inventory_Masters_Kinect.View
{
    /// <summary>
    /// Janela de acesso ao Kinect. O MVC gera/envia o token e o aplicativo Kinect valida
    /// esse token antes de liberar o monitor.
    /// </summary>
    public partial class KinectLogin : Window
    {
        public KinectLogin()
        {
            InitializeComponent();
            MensagemTextBlock.Text = string.Empty;
            MostrarLogin();
        }
        private void AbaEntrar_Click(object sender, RoutedEventArgs e)
        {
            MostrarLogin();
        }
        private void AbaCadastro_Click(object sender, RoutedEventArgs e)
        {
            MostrarSolicitacaoToken();
        }
        private void MostrarLogin()
        {
            MensagemTextBlock.Foreground = System.Windows.Media.Brushes.Red;
            MensagemTextBlock.Text = string.Empty;
            LoginPanel.Visibility = Visibility.Visible;
            CadastroPanel.Visibility = Visibility.Collapsed;
            TituloTextBlock.Text = "Acesso ao Kinect";
            SubtituloTextBlock.Text = "Informe o token enviado pelo sistema MVC";
            AbaEntrarButton.Background = System.Windows.Media.Brushes.ForestGreen;
            AbaEntrarButton.Foreground = System.Windows.Media.Brushes.White;
            AbaCadastroButton.Background = System.Windows.Media.Brushes.LightGray;
            AbaCadastroButton.Foreground = System.Windows.Media.Brushes.Black;
        }
        private void MostrarSolicitacaoToken()
        {
            MensagemTextBlock.Foreground = System.Windows.Media.Brushes.Red;
            MensagemTextBlock.Text = string.Empty;
            LoginPanel.Visibility = Visibility.Collapsed;
            CadastroPanel.Visibility = Visibility.Visible;
            TituloTextBlock.Text = "Solicitar Token";
            SubtituloTextBlock.Text = "O MVC envia o token para o e-mail cadastrado";
            AbaEntrarButton.Background = System.Windows.Media.Brushes.LightGray;
            AbaEntrarButton.Foreground = System.Windows.Media.Brushes.Black;
            AbaCadastroButton.Background = System.Windows.Media.Brushes.ForestGreen;
            AbaCadastroButton.Foreground = System.Windows.Media.Brushes.White;
        }
        private async void Entrar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string token = LoginUsuarioTextBox.Text?.Trim();

                if (string.IsNullOrWhiteSpace(token))
                {
                    MensagemTextBlock.Foreground = System.Windows.Media.Brushes.Red;
                    MensagemTextBlock.Text = "Informe o token de acesso.";
                    return;
                }

                var autenticacaoService = new AutenticacaoMvcService();
                var resultado = await autenticacaoService.ValidarTokenAsync(token);

                if (resultado == null || !resultado.TokenValido)
                {
                    MensagemTextBlock.Foreground = System.Windows.Media.Brushes.Red;
                    MensagemTextBlock.Text = resultado?.Mensagem ?? "Token invalido ou expirado.";
                    LoggerService.LogWarning("Tentativa invalida de acesso ao Kinect.");
                    return;
                }

                var sessao = new SessaoUsuario
                {
                    Usuario = resultado.Usuario,
                    Empresa = resultado.Empresa,
                    Email = resultado.Email,
                    Token = token
                };

                LoggerService.Info("Acesso ao Kinect liberado com token validado pelo MVC.");
                AbrirMonitor(sessao);
            }
            catch
            {
                MensagemTextBlock.Foreground = System.Windows.Media.Brushes.Red;
                MensagemTextBlock.Text = "Erro ao validar token no MVC.";
                LoggerService.Erro("Erro ao validar token no MVC.");
            }
        }
        private async void Cadastrar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string email = CadastroEmailTextBox.Text?.Trim();

                if (string.IsNullOrWhiteSpace(email))
                {
                    MensagemTextBlock.Foreground = System.Windows.Media.Brushes.Red;
                    MensagemTextBlock.Text = "Informe o e-mail cadastrado.";
                    return;
                }

                var autenticacaoService = new AutenticacaoMvcService();
                var resultado = await autenticacaoService.SolicitarTokenAsync(email);

                if (resultado == null || !resultado.Sucesso)
                {
                    MensagemTextBlock.Foreground = System.Windows.Media.Brushes.Red;
                    MensagemTextBlock.Text = resultado?.Mensagem ?? "Nao foi possivel solicitar o token.";
                    return;
                }

                MostrarLogin();
                MensagemTextBlock.Foreground = System.Windows.Media.Brushes.ForestGreen;
                MensagemTextBlock.Text = "Token enviado. Informe o codigo recebido para acessar o Kinect.";
                LoggerService.Info("Token solicitado ao MVC pelo aplicativo Kinect.");
            }
            catch
            {
                MensagemTextBlock.Foreground = System.Windows.Media.Brushes.Red;
                MensagemTextBlock.Text = "Erro ao solicitar token no MVC.";
                LoggerService.Erro("Erro ao solicitar token no MVC pelo aplicativo Kinect.");
            }
        }
        private void AbrirMonitor(SessaoUsuario sessao)
        {
            var janela = new KinectMonitorWindow(sessao);
            janela.Show();
            Close();
        }
    }
}
