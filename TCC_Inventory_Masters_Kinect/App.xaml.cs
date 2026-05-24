using System.Windows;
using TCC_Inventory_Masters_Kinect.Logs;
using TCC_Inventory_Masters_Kinect.Service;

namespace TCC_Inventory_Masters_Kinect
{
    public partial class App : Application
    {
        protected override void OnStartup(
            StartupEventArgs e)
        {
            base.OnStartup(e);

            LoggerServiceKinect.Log(
                "Sistema iniciado.");
        }

        protected override void OnExit(
            ExitEventArgs e)
        {
            LoggerServiceKinect.Log(
                "Sistema encerrado.");

            base.OnExit(e);
        }
    }
}