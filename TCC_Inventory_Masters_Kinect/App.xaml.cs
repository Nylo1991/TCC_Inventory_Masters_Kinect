using System.Windows;
using TCC_Inventory_Masters_Kinect.Data; 

namespace TCC_Inventory_Masters_Kinect
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Esta lógica garante que o banco seja criado assim que a aplicação iniciar
            using (var db = new AppDbContext())
            {
                // Verifica se o arquivo .db existe, caso contrário, cria-o na pasta do executável
                db.Database.CreateIfNotExists();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
        }
    }
}