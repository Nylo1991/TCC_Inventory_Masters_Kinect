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
        // Força a utilização do SQLite e define o caminho absoluto do arquivo
        public AppDbContext() : base(CreateSQLiteConnection(), true)
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<AppDbContext>());
        }

        private static SQLiteConnection CreateSQLiteConnection()
        {
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "inventorymasters.db");
            return new SQLiteConnection($"Data Source={dbPath};Version=3;");
        }

        public DbSet<MedicaoVolume> MedicaoVolumes { get; set; }
    }

    public class SQLiteConfigurationInternal : DbConfiguration
    {
        public SQLiteConfigurationInternal()
        {
            SetProviderFactory("System.Data.SQLite", SQLiteFactory.Instance);
            SetProviderFactory("System.Data.SQLite.EF6", SQLiteProviderFactory.Instance);
            SetProviderServices("System.Data.SQLite", (System.Data.Entity.Core.Common.DbProviderServices)SQLiteProviderFactory.Instance.GetService(typeof(System.Data.Entity.Core.Common.DbProviderServices)));
        }
    }
}