using System;
using System.Windows;
using TCC_Inventory_Masters_Kinect.Service;
using TCC_Inventory_Masters_Kinect.View;

namespace TCC_Inventory_Masters_Kinect.View
{
    public partial class MainWindow : Window
    {
        private KinectService _kinectService;

        public MainWindow()
        {
            InitializeComponent();
            _kinectService = new KinectService();
            _kinectService.InicializarKinect();
        }

        private void AbrirKinect_Click(object sender, RoutedEventArgs e)
        {
            // Criamos a página passando o serviço para ela
            var paginaKinect = new KinectVisualizer(_kinectService);
            MainFrame.Navigate(paginaKinect);
        }

        private void AbrirCadastro_Click(object sender, RoutedEventArgs e) { }
        private void AbrirHistorico_Click(object sender, RoutedEventArgs e) { }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _kinectService?.DesligarKinect();
        }
    }
}