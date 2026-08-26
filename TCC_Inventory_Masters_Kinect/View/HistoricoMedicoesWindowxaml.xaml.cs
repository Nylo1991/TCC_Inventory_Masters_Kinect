using System.Windows;
using TCC_Inventory_Masters_Kinect.ViewModel;

namespace TCC_Inventory_Masters_Kinect.View
{
    public partial class HistoricoMedicoesWindow : Window
    {
        public HistoricoMedicoesWindow(MainViewModel mainViewModel)
        {
            InitializeComponent();
            DataContext = mainViewModel;
        }
    }
}
