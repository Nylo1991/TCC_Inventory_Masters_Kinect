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

                var janela = new KinectMonitorWindow();
                janela.Show();

                Window.GetWindow(this)?.Close();
            }
            catch
            {
                MensagemTextBlock.Text = "Erro ao realizar login.";
                LoggerService.Erro("Erro ao realizar login.");
            }
        }

        private void CriarAdmin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    bool existeAdmin = db.UsuariosAcesso.Any(x => x.Usuario == "admin");

                    if (existeAdmin)
                    {
                        MensagemTextBlock.Text = "Administrador inicial ja existe.";
                        return;
                    }

                    var admin = new UsuarioAcesso
                    {
                        Usuario = "admin",
                        Senha = "123",
                        Perfil = "Administrador",
                        CriadoEm = DateTime.Now,
                        Ativo = true
                    };

                    db.UsuariosAcesso.Add(admin);
                    db.SaveChanges();
                }

                MensagemTextBlock.Text = "Administrador criado. Usuario: admin | Senha: 123";
                LoggerService.Info("Administrador inicial criado.");
            }
            catch
            {
                MensagemTextBlock.Text = "Erro ao criar administrador inicial.";
                LoggerService.Erro("Erro ao criar administrador inicial.");
            }
        }
    }
}