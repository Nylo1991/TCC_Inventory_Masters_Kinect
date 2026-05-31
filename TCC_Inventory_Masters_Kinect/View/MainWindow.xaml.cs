using System;
using System.ComponentModel;
using System.Windows;
using TCC_Inventory_Masters_Kinect.ViewModel;

namespace TCC_Inventory_Masters_Kinect.View
{
    /// <summary>
    /// Interação lógica para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _fechando = false;

        public MainWindow()
        {
            InitializeComponent();

            DataContext =
                new MainViewModel();

            Closing +=
                MainWindow_Closing;
        }

        private async void MainWindow_Closing(
            object sender,
            CancelEventArgs e)
        {
            if (_fechando)
            {
                return;
            }

            e.Cancel =
                true;

            _fechando =
                true;

            try
            {
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
                Closing -=
                    MainWindow_Closing;

                Close();
            }
        }
    }
}