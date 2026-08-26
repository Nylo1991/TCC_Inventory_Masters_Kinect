using System.Windows;
using TCC_Inventory_Masters_Kinect.Model;
using TCC_Inventory_Masters_Kinect.ViewModel;

namespace TCC_Inventory_Masters_Kinect.View
{
    public partial class KinectMonitorWindow : Window
    {
        public KinectMonitorWindow(SessaoUsuario sessao)
        {
            InitializeComponent();
            DataContext = new MainViewModel(sessao);
        }
    }
}
