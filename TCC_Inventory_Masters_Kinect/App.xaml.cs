using System.Windows;
using TCC_Inventory_Masters_Kinect.Service;

namespace TCC_Inventory_Masters_Kinect
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            // Logger removido pois a classe não existe mais
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Logger removido pois a classe não existe mais
            base.OnExit(e);
        }
    }
}