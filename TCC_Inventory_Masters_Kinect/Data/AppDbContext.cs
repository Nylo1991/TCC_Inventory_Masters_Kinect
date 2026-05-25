using System.Data.Entity;
using TCC_Inventory_Masters_Kinect.Model;
using System.Data.SQLite;
using System.Data.SQLite.EF6;
using System;
using System.IO;

namespace TCC_Inventory_Masters_Kinect.Data
{
    [DbConfigurationType(typeof(SQLiteConfigurationInternal))]
    public class AppDbContext : DbContext
    {
        public AppDbContext() : base(CreateSQLiteConnection(), true)
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<AppDbContext>());

            CriarTabelaManual();
        }

        private static SQLiteConnection CreateSQLiteConnection()
        {
            string dbPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "inventorymasters.db"
            );

            return new SQLiteConnection(
                $"Data Source={dbPath};Version=3;"
            );
        }

        public DbSet<MedicaoVolume> MedicaoVolumes { get; set; }

        // ======================================================
        // CRIAÇÃO MANUAL DA TABELA
        // ======================================================

        private void CriarTabelaManual()
        {
            using (var conn = CreateSQLiteConnection())
            {
                conn.Open();

                string sql = @"
                CREATE TABLE IF NOT EXISTS MedicaoVolumes (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    VolumeCm3 REAL NOT NULL,
                    DataHora TEXT NOT NULL,
                    KinectLigado INTEGER NOT NULL,
                    Calibrado INTEGER NOT NULL,
                    Status TEXT
                );
                ";

                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }

    public class SQLiteConfigurationInternal : DbConfiguration
    {
        public SQLiteConfigurationInternal()
        {
            SetProviderFactory(
                "System.Data.SQLite",
                SQLiteFactory.Instance
            );

            SetProviderFactory(
                "System.Data.SQLite.EF6",
                SQLiteProviderFactory.Instance
            );

            SetProviderServices(
                "System.Data.SQLite",
                (System.Data.Entity.Core.Common.DbProviderServices)
                SQLiteProviderFactory.Instance.GetService(
                    typeof(System.Data.Entity.Core.Common.DbProviderServices)
                )
            );
        }
    }
}