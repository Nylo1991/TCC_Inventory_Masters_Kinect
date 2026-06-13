using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TCC_Inventory_Masters_Kinect.Data;
using TCC_Inventory_Masters_Kinect.Logs;
using TCC_Inventory_Masters_Kinect.Model;

namespace TCC_Inventory_Masters_Kinect.View
{
    public partial class Page1 : Page
    {
        public Page1()
        {
            InitializeComponent();
            AtualizarModoTela();
        }

        private void AtualizarModoTela()
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    bool existeUsuario = db.UsuariosAcesso.Any();

                    if (existeUsuario)
                    {
                        TituloTextBlock.Text = "Acesso ao Kinect";
                        SubtituloTextBlock.Text = "Entre para iniciar o monitoramento volumetrico";
                        EntrarButton.Content = "Entrar";
                    }
                    else
                    {
                        TituloTextBlock.Text = "Criar administrador";
                        SubtituloTextBlock.Text = "Cadastre o primeiro acesso do sistema";
                        EntrarButton.Content = "Criar e entrar";
                    }
                }
            }
            catch
            {
                MensagemTextBlock.Text = "Erro ao verificar usuarios.";
                LoggerService.Erro("Erro ao verificar usuarios no login.");
            }
        }

        private void Entrar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string usuario = UsuarioTextBox.Text?.Trim();
                string senha = SenhaPasswordBox.Password?.Trim();

                if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(senha))
                {
                    MensagemTextBlock.Text = "Informe usuario e senha.";
                    return;
                }

                using (var db = new AppDbContext())
                {
                    bool existeUsuario = db.UsuariosAcesso.Any();

                    if (!existeUsuario)
                    {
                        var primeiroUsuario = new UsuarioAcesso
                        {
                            Usuario = usuario,
                            Senha = senha,
                            Perfil = "Administrador",
                            CriadoEm = DateTime.Now,
                            Ativo = true
                        };

                        db.UsuariosAcesso.Add(primeiroUsuario);
                        db.SaveChanges();

                        LoggerService.Info("Primeiro usuario administrador criado.");
                        AbrirMonitor();
                        return;
                    }

                    var usuarioAcesso = db.UsuariosAcesso
                        .FirstOrDefault(x =>
                            x.Usuario == usuario &&
                            x.Senha == senha &&
                            x.Ativo);

                    if (usuarioAcesso == null)
                    {
                        MensagemTextBlock.Text = "Usuario ou senha invalidos.";
                        LoggerService.LogWarning("Tentativa de login invalida.");
                        return;
                    }
                }

                LoggerService.Info("Login realizado com sucesso.");
                AbrirMonitor();
            }
            catch
            {
                MensagemTextBlock.Text = "Erro ao realizar login.";
                LoggerService.Erro("Erro ao realizar login.");
            }
        }

        private void AbrirMonitor()
        {
            var janela = new KinectMonitorWindow();
            janela.Show();

            Window.GetWindow(this)?.Close();
        }
    }
}