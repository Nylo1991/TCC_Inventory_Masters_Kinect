using System;
using System.Linq;
using System.Net.Mail;
using System.Windows;
using TCC_Inventory_Masters_Kinect.Data;
using TCC_Inventory_Masters_Kinect.Logs;
using TCC_Inventory_Masters_Kinect.Model;

namespace TCC_Inventory_Masters_Kinect.View
{
    public partial class KinectLogin : Window
    {
        public KinectLogin()
        {
            InitializeComponent();
            MensagemTextBlock.Text = string.Empty;
            DefinirTelaInicial();
        }

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

        private void Entrar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string usuario = LoginUsuarioTextBox.Text?.Trim();
                string senha = LoginSenhaPasswordBox.Password?.Trim();

                if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(senha))
                {
                    MensagemTextBlock.Text = "Informe usuario e senha.";
                    return;
                }

                using (var db = new AppDbContext())
                {
                    var usuarioAcesso = db.UsuariosAcesso
                        .FirstOrDefault(x => x.Usuario == usuario && x.Senha == senha && x.Ativo);

                    if (usuarioAcesso == null)
                    {
                        MensagemTextBlock.Text = "Usuario ou senha invalidos.";
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

        private void Cadastrar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MensagemTextBlock.Text = string.Empty;

                string usuario = CadastroUsuarioTextBox.Text?.Trim();
                string email = CadastroEmailTextBox.Text?.Trim();
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

                if (senha != confirmarSenha)
                {
                    MensagemTextBlock.Text = "As senhas nao conferem.";
                    return;
                }

                using (var db = new AppDbContext())
                {
                    bool usuarioJaExiste = db.UsuariosAcesso.Any(x => x.Usuario == usuario);

                    if (usuarioJaExiste)
                    {
                        MensagemTextBlock.Text = "Este usuario ja esta cadastrado.";
                        return;
                    }

                    bool emailJaExiste = db.UsuariosAcesso.Any(x => x.Email == email);

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
                        Perfil = "",
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

        private bool EmailValido(string email)
        {
            try
            {
                var endereco = new MailAddress(email);
                return endereco.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private void AbrirMonitor(string usuario)
        {
            var janela = new KinectMonitorWindow(usuario);
            janela.Show();
            Close();
        }
    }
}