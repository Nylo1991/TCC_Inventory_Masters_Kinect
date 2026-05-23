using System;
using System.Windows;
using TCC_Inventory_Masters_Kinect.Data;

namespace TCC_Inventory_Masters_Kinect
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                using (var db = new AppDbContext())
                {
                    db.Database.CreateIfNotExists();

                    db.Database.ExecuteSqlCommand(@"
                        CREATE TABLE IF NOT EXISTS MedicaoVolumes (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            VolumeCm3 REAL NOT NULL,
                            DataHora DATETIME NOT NULL,
                            KinectLigado INTEGER NOT NULL,
                            Calibrado INTEGER NOT NULL,
                            Status TEXT
                        );
                    ");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "ERRO SQLITE");
            }

            MainWindow janela = new MainWindow();
            janela.Show();
        }
    }
}