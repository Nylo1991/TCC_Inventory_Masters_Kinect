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

        public MainWindow()
        {
            InitializeComponent();

            // Evita criar dois MainViewModel.
            // Se o DataContext já estiver definido no XAML, ele reaproveita.
            if (DataContext == null)
            {
                DataContext =
                    new MainViewModel();
            }

            Closing +=
                MainWindow_Closing;
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