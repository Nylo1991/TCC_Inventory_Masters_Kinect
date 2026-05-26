using System;
using System.Data.Entity;
using System.Data.SQLite;
using System.Data.SQLite.EF6;
using System.IO;
using TCC_Inventory_Masters_Kinect.Model;

namespace TCC_Inventory_Masters_Kinect.Data
{
    [DbConfigurationType(typeof(SQLiteConfigurationInternal))]
    public class AppDbContext : DbContext
    {
        static AppDbContext()
        {
            // Evita o Entity Framework tentar criar migrations/tabelas automaticamente.
            Database.SetInitializer<AppDbContext>(null);
        }

        public AppDbContext() : base(CreateSQLiteConnection(), true)
        {
        }

        private static SQLiteConnection CreateSQLiteConnection()
        {
            string dbPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "inventorymasters.db"
            );

            return new SQLiteConnection(
                $"Data Source={dbPath};Version=3;BusyTimeout=5000;Journal Mode=WAL;"
            );
        }

        public DbSet<MedicaoVolume> MedicaoVolumes { get; set; }
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