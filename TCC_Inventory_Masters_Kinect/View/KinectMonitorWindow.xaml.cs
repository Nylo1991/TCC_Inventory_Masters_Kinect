using System.ComponentModel;
using System.Windows;
using TCC_Inventory_Masters_Kinect.ViewModel;

namespace TCC_Inventory_Masters_Kinect.View
{
    public partial class KinectMonitorWindow : Window
    {
        public KinectMonitorWindow()
        {
            InitializeComponent();
            DataContext = new KinectMonitorWindowViewModel ();   // ← Testando com MainViewModel}
        }
        private void KinectWindow_Closing(object sender, CancelEventArgs e)
        {
            // Código ao fechar a janela
        }
    }
}
