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
        /// <summary>
        /// Inicializa a janela de login do Kinect, configurando os elementos visuais e exibindo o painel de login por padrão.
        /// </summary>
        public KinectLogin()
        {
            InitializeComponent();
            MensagemTextBlock.Text = string.Empty;
            MostrarSolicitacaoToken();
        }
        /// <summary>
        /// Evento de clique do botão "Entrar". Exibe o painel de login, ocultando o painel de 
        /// cadastro e ajustando os textos e cores dos botões.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AbaEntrar_Click(object sender, RoutedEventArgs e)
        {
            MostrarLogin();
        }
        /// <summary>
        /// Evento de clique do botão "Cadastrar". 
        /// Exibe o painel de solicitação de token, ocultando o painel de login e ajustando os textos e cores dos botões.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AbaCadastro_Click(object sender, RoutedEventArgs e)
        {
            MostrarSolicitacaoToken();
        }

        /// <summary>
        ///  Exibe o painel de login, ocultando o painel de cadastro e ajustando os textos e cores dos botões.
        /// </summary>
        private void MostrarLogin()
        {
            MensagemTextBlock.Foreground = System.Windows.Media.Brushes.Red;
            MensagemTextBlock.Text = string.Empty;
            LoginPanel.Visibility = Visibility.Visible;
            CadastroPanel.Visibility = Visibility.Collapsed;
            TituloTextBlock.Text = "Acesso ao Kinect";
            SubtituloTextBlock.Text = "Informe o token enviado pelo sistema ";
            AbaEntrarButton.Background = System.Windows.Media.Brushes.ForestGreen;
            AbaEntrarButton.Foreground = System.Windows.Media.Brushes.White;
            AbaCadastroButton.Background = System.Windows.Media.Brushes.LightGray;
            AbaCadastroButton.Foreground = System.Windows.Media.Brushes.Black;
        }
        /// <summary>
        /// Exibe o painel de solicitação de token, ocultando o painel de login e ajustando os textos e cores dos botões.
        /// </summary>
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

        /// <summary>
        /// Evento de clique do botão "Entrar". Valida o token informado pelo usuário e,
        /// se válido, abre a janela do monitor Kinect.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Entrar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string token = LoginUsuarioTextBox.Text?.Trim();

                if (!System.Text.RegularExpressions.Regex.IsMatch(
                    token ?? string.Empty,
                    @"^\d{6}$"))
                {
                    MensagemTextBlock.Foreground = System.Windows.Media.Brushes.Red;
                    MensagemTextBlock.Text = "Informe os seis numeros do token de acesso.";
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

                LoggerService.Info("Acesso ao Kinect liberado com token validado pelo Sistema.");
                AbrirMonitor(sessao);
            }
            catch
            {
                MensagemTextBlock.Foreground = System.Windows.Media.Brushes.Red;
                MensagemTextBlock.Text = "Erro ao validar token no Sistema.";
                LoggerService.Erro("Erro ao validar token pelo o sistema.");
            }
        }
        /// <summary>
        /// Impede a digitacao de letras, sinais e espacos no campo de token.
        /// </summary>
        private void SomenteNumeros_PreviewTextInput(
            object sender,
            System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !System.Text.RegularExpressions.Regex.IsMatch(e.Text, @"^\d+$");
        }

        /// <summary>
        /// Permite colar somente um token numerico de ate seis digitos.
        /// </summary>
        private void TokenTextBox_Pasting(
            object sender,
            System.Windows.DataObjectPastingEventArgs e)
        {
            if (!e.SourceDataObject.GetDataPresent(System.Windows.DataFormats.Text, true))
            {
                e.CancelCommand();
                return;
            }

            string texto = e.SourceDataObject.GetData(System.Windows.DataFormats.Text) as string
                ?? string.Empty;
            var campo = sender as System.Windows.Controls.TextBox;
            int tamanhoFinal = (campo?.Text?.Length ?? 0)
                - (campo?.SelectionLength ?? 0)
                + texto.Length;

            if (!System.Text.RegularExpressions.Regex.IsMatch(texto, @"^\d+$") ||
                tamanhoFinal > 6)
            {
                e.CancelCommand();
            }
        }

        /// <summary>
        /// Exibe o marcador 000000 somente enquanto o campo estiver vazio.
        /// </summary>
        private void LoginUsuarioTextBox_TextChanged(
            object sender,
            System.Windows.Controls.TextChangedEventArgs e)
        {
            if (TokenPlaceholderTextBlock != null)
            {
                TokenPlaceholderTextBlock.Visibility =
                    string.IsNullOrEmpty(LoginUsuarioTextBox.Text)
                        ? Visibility.Visible
                        : Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Evento de clique do botão "Cadastrar". Solicita um token ao MVC para o e-mail informado pelo usuário.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// <summary>
        /// Abre a janela do monitor Kinect com a sessão do usuário validada e fecha a janela de login.
        /// </summary>
        /// <param name="sessao">Sessão do usuário validada.</param>
        private void AbrirMonitor(SessaoUsuario sessao)
        {
            var janela = new KinectMonitorWindow(sessao);
            janela.Show();
            Close();
        }
    }
}
