using System.Windows;
using TCC_Inventory_Masters_Kinect.ViewModel;

namespace TCC_Inventory_Masters_Kinect.View
{
    public partial class KinectLogin : Window
    {
        public KinectLogin()
        {
            InitializeComponent();
            DataContext = new KinectLoginViewModel();
        }
    }
}
