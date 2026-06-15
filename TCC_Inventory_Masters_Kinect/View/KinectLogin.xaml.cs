using System;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Windows;
using TCC_Inventory_Masters_Kinect.Data;
using TCC_Inventory_Masters_Kinect.Logs;
using TCC_Inventory_Masters_Kinect.Model;


namespace TCC_Inventory_Masters_Kinect.View
{
    /// <summary>
    /// Janela de logim e cadastro de usuarios para acesso ao monitoramento volumétrico do Kinect.
    /// todas as responsabilidades de validação, autenticação e persistência de usuarios estão centralizadas nesta classe.
    /// </summary>
    public partial class KinectLogin : Window
    {

        /// <summary>
        /// Construtor da janela de login. Verifica se existem usuarios cadastrados no banco SQLite.
        /// Se existirem, exibe a tela de login. Caso contrário, exibe a tela de cadastro para criar o primeiro usuario. 
        /// Qualquer erro na verificação do banco resultará na exibição da tela de cadastro, 
        /// assumindo que o banco pode estar vazio ou inacessível.
        /// </summary>
        public KinectLogin()
        {
            InitializeComponent();
            MensagemTextBlock.Text = string.Empty;
            DefinirTelaInicial();
        }

        /// <summary>
        /// Responsavel por definir qual tela exibir inicialmente, login ou cadastro, 
        /// com base na existencia de usuarios cadastrados no banco de dados.
        /// </summary>
        private void DefinirTelaInicial()
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    if (db.UsuariosAcesso.Any())
                    {
                        MostrarLogin();
                    }
                    else
                    {
                        MostrarCadastro();
                    }
                }
            }
            catch
            {
                MensagemTextBlock.Text = string.Empty;
                MostrarCadastro();
                LoggerService.Erro("Erro ao verificar usuarios cadastrados.");
            }
        }

        private void AbaEntrar_Click(object sender, RoutedEventArgs e)
        {
            MostrarLogin();
        }

        private void AbaCadastro_Click(object sender, RoutedEventArgs e)
        {
            MostrarCadastro();
        }
        /// <summary>
        /// responsavel por configurar a interface para exibir a tela de login, ocultando o painel de cadastro e 
        /// ajustando os textos e estilos dos botões para refletir a seleção atual.
        /// </summary>
        private void MostrarLogin()
        {
            MensagemTextBlock.Text = string.Empty;
            LoginPanel.Visibility = Visibility.Visible;
            CadastroPanel.Visibility = Visibility.Collapsed;
            TituloTextBlock.Text = "Acesso ao Kinect";
            SubtituloTextBlock.Text = "Entre para iniciar o monitoramento volumetrico";
            AbaEntrarButton.Background = System.Windows.Media.Brushes.ForestGreen;
            AbaEntrarButton.Foreground = System.Windows.Media.Brushes.White;
            AbaCadastroButton.Background = System.Windows.Media.Brushes.LightGray;
            AbaCadastroButton.Foreground = System.Windows.Media.Brushes.Black;
        }
        /// <summary>
        /// responsavel por configurar a interface para exibir a tela de cadastro, ocultando o painel de login e
        /// ajustando os textos e estilos dos botões para refletir a seleção atual.
        /// </summary>
        private void MostrarCadastro()
        {
            MensagemTextBlock.Text = string.Empty;
            LoginPanel.Visibility = Visibility.Collapsed;
            CadastroPanel.Visibility = Visibility.Visible;
            TituloTextBlock.Text = "Cadastro de Acesso";
            SubtituloTextBlock.Text = "Crie um usuario para acessar o sistema";
            AbaEntrarButton.Background = System.Windows.Media.Brushes.LightGray;
            AbaEntrarButton.Foreground = System.Windows.Media.Brushes.Black;
            AbaCadastroButton.Background = System.Windows.Media.Brushes.ForestGreen;
            AbaCadastroButton.Foreground = System.Windows.Media.Brushes.White;
        }
        /// <summary>
        /// Responsavel por autenticar o usuario com base no identificador (usuario ou email) e senha fornecidos.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Entrar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string identificador = LoginUsuarioTextBox.Text?.Trim().ToLower();
                string senha = LoginSenhaPasswordBox.Password?.Trim();

                if (string.IsNullOrWhiteSpace(identificador) || string.IsNullOrWhiteSpace(senha))
                {
                    MensagemTextBlock.Text = "Informe usuario ou email e senha.";
                    return;
                }

                using (var db = new AppDbContext())
                {
                    var usuarioAcesso = db.UsuariosAcesso
                        .FirstOrDefault(x =>
                            (x.Usuario.ToLower() == identificador || x.Email.ToLower() == identificador) &&
                            x.Senha == senha &&
                            x.Ativo);

                    if (usuarioAcesso == null)
                    {
                        MensagemTextBlock.Text = "Usuario, email ou senha invalidos.";
                        LoggerService.LogWarning("Tentativa de login invalida.");
                        return;
                    }

                    LoggerService.Info("Login realizado com sucesso.");
                    AbrirMonitor(usuarioAcesso.Usuario);
                }
            }
            catch
            {
                MensagemTextBlock.Text = "Erro ao realizar login.";
                LoggerService.Erro("Erro ao realizar login.");
            }
        }

        /// <summary>
        /// Responsavel por validar os dados de cadastro e criar um novo usuario no banco de dados .
        /// isso garante que apenas usuarios com dados validos sejam criados,
        /// e que o processo de cadastro seja robusto contra erros comuns, como campos vazios, senhas fracas ou emails invalidos.
        /// Mais so tem um porem ainda não manda email para o usuario cadastrado, isso pode ser implementado em uma versão futura para 
        /// melhorar a experiencia do usuario e a segurança do sistema.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cadastrar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MensagemTextBlock.Text = string.Empty;

                string usuario = CadastroUsuarioTextBox.Text?.Trim();
                string email = CadastroEmailTextBox.Text?.Trim().ToLower();
                string senha = CadastroSenhaPasswordBox.Password?.Trim();
                string confirmarSenha = CadastroConfirmarSenhaPasswordBox.Password?.Trim();

                if (string.IsNullOrWhiteSpace(usuario))
                {
                    MensagemTextBlock.Text = "Informe o usuario.";
                    return;
                }

                if (string.IsNullOrWhiteSpace(email))
                {
                    MensagemTextBlock.Text = "Informe o email.";
                    return;
                }

                if (!EmailValido(email))
                {
                    MensagemTextBlock.Text = "Informe um email valido.";
                    return;
                }

                if (string.IsNullOrWhiteSpace(senha))
                {
                    MensagemTextBlock.Text = "Informe a senha.";
                    return;
                }

                if (senha.Contains(" "))
                {
                    MensagemTextBlock.Text = "A senha não pode conter espaços.";
                    return;
                }

                if (senha.Length < 6)
                {
                    MensagemTextBlock.Text = "A senha deve ter no minimo 6 caracteres.";
                    return;
                }

                if (string.IsNullOrWhiteSpace(confirmarSenha))
                {
                    MensagemTextBlock.Text = "Confirme a senha.";
                    return;
                }

                if (confirmarSenha.Contains(" "))
                {
                    MensagemTextBlock.Text = "A confirmação da senha não pode conter espaços.";
                    return;
                }

                if (senha != confirmarSenha)
                {
                    MensagemTextBlock.Text = "As senhas nao conferem.";
                    return;
                }

                using (var db = new AppDbContext())
                {
                    bool usuarioJaExiste = db.UsuariosAcesso
                        .Any(x => x.Usuario.ToLower() == usuario.ToLower());

                    if (usuarioJaExiste)
                    {
                        MensagemTextBlock.Text = "Este usuario ja esta cadastrado.";
                        return;
                    }

                    bool emailJaExiste = db.UsuariosAcesso
                        .Any(x => x.Email.ToLower() == email);

                    if (emailJaExiste)
                    {
                        MensagemTextBlock.Text = "Este email ja esta cadastrado.";
                        return;
                    }

                    var novoUsuario = new UsuarioAcesso
                    {
                        Usuario = usuario,
                        Email = email,
                        Senha = senha,
                        Perfil = "Usuario",
                        CriadoEm = DateTime.Now,
                        Ativo = true
                    };

                    db.UsuariosAcesso.Add(novoUsuario);
                    db.SaveChanges();
                }

                LoggerService.Info("Usuario cadastrado com sucesso.");
                AbrirMonitor(usuario);
            }
            catch
            {
                MensagemTextBlock.Text = "Erro ao salvar usuario. Verifique se a tabela UsuariosAcesso existe no banco.";
                LoggerService.Erro("Erro ao cadastrar usuario.");
            }
        }
        /// <summary>
        /// Responsavel por validar se o email fornecido no cadastro é valido, utilizando uma combinação de
        /// verificações manuais e expressões regulares para garantir que o formato do email seja correto e 
        /// que ele possa ser utilizado para comunicação futura, 
        /// caso seja necessário implementar funcionalidades como recuperação de senha ou notificações por email.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        private bool EmailValido(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            email = email.Trim();

            if (email.Contains(" "))
            {
                return false;
            }

            if (email.Count(x => x == '@') != 1)
            {
                return false;
            }

            string padrao = @"^[^@\s]+@[^@\s]+\.[A-Za-z]{2,}$";

            if (!Regex.IsMatch(email, padrao))
            {
                return false;
            }

            try
            {
                var endereco = new MailAddress(email);
                return endereco.Address.Equals(email, StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// Responsavel por abrir a janela de monitoramento volumétrico do Kinect, 
        /// passando o nome do usuario autenticado para personalizar a experiencia e registrar o acesso nos logs.
        /// </summary>
        /// <param name="usuario">Nome do usuario autenticado</param>
        private void AbrirMonitor(string usuario)
        {
            var janela = new KinectMonitorWindow(usuario);
            janela.Show();
            Close();
        }
    }
}