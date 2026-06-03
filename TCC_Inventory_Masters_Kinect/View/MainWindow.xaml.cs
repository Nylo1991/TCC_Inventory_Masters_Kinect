using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;
using TCC_Inventory_Masters_Kinect.ViewModel;

namespace TCC_Inventory_Masters_Kinect.View
{
    /// <summary>
    /// Interação lógica para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _podeFechar =
            false;

        private bool _encerramentoEmAndamento =
            false;

        private MainViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();

            _viewModel =
                new MainViewModel();

            DataContext =
                _viewModel;

            Closing +=
                MainWindow_Closing;
        }

        private void AbrirKinect_Click(
            object sender,
            RoutedEventArgs e)
        {
            KinectWindow janela =
                new KinectWindow();

            janela.DataContext =
                _viewModel;

            janela.Owner =
                this;

            janela.Show();
        }

        private void AbrirCadastro_Click(
            object sender,
            RoutedEventArgs e)
        {
            CadastroEspacoWindow janela =
                new CadastroEspacoWindow();

            janela.DataContext =
                _viewModel;

            janela.Owner =
                this;

            janela.Show();
        }

        private void AbrirHistorico_Click(
            object sender,
            RoutedEventArgs e)
        {
            HistoricoMedicoesWindow janela =
                new HistoricoMedicoesWindow();

            janela.DataContext =
                _viewModel;

            janela.Owner =
                this;

            janela.Show();
        }

        private async void MainWindow_Closing(
            object sender,
            CancelEventArgs e)
        {
            if (_podeFechar)
            {
                return;
            }

            if (_encerramentoEmAndamento)
            {
                e.Cancel =
                    true;

                return;
            }

            e.Cancel =
                true;

            _encerramentoEmAndamento =
                true;

            try
            {
                IsEnabled =
                    false;

                if (DataContext is MainViewModel viewModel)
                {
                    await viewModel
                        .EncerrarAplicacaoAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Erro ao fechar aplicação");
            }
            finally
            {
                _podeFechar =
                    true;

                _encerramentoEmAndamento =
                    false;

                Closing -=
                    MainWindow_Closing;

                Dispatcher.BeginInvoke(
                    new Action(() =>
                    {
                        try
                        {
                            Close();
                        }
                        catch
                        {
                            Application.Current.Shutdown();
                        }
                    }),
                    DispatcherPriority.ContextIdle);
            }
        }
    }
}