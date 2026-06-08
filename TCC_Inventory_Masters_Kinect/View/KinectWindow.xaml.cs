using System.Windows;
using TCC_Inventory_Masters_Kinect.ViewModel;

namespace TCC_Inventory_Masters_Kinect.View
{
    public partial class KinectWindow : Window
    {
        private readonly MainViewModel _viewModel;

        public KinectWindow()
        {
            InitializeComponent();

            _viewModel = new MainViewModel();
            DataContext = _viewModel;

            CarregarVisualizador3D();
        }

        private void CarregarVisualizador3D()
        {
            // Agora passamos o KinectService diretamente
            var visualizador = new KinectVisualizer(_viewModel.KinectService);
            VisualizadorFrame.Navigate(visualizador);
        }

        private async void KinectWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            await _viewModel.EncerrarAplicacaoAsync();
        }
    }
}
