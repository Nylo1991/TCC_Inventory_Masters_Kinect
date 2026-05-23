using System.Data.Entity;
using TCC_Inventory_Masters_Kinect.Model;
using System.Data.SQLite.EF6;

namespace TCC_Inventory_Masters_Kinect.Data
{
    // Esta linha diz ao Entity Framework como traduzir os comandos para o SQLite
    [DbConfigurationType(typeof(SQLiteConfigurationInternal))]
    public class AppDbContext : DbContext
    {
        public AppDbContext() : base("name=InventoryMastersDb")
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<AppDbContext>());
        }

        public DbSet<MedicaoVolume> MedicoesVolume { get; set; }
    }

    // Classe de configuração interna (não precisa de arquivo novo)
    public class SQLiteConfigurationInternal : DbConfiguration
    {
        public SQLiteConfigurationInternal()
        {
            SetProviderFactory("System.Data.SQLite", System.Data.SQLite.SQLiteFactory.Instance);
            SetProviderFactory("System.Data.SQLite.EF6", SQLiteProviderFactory.Instance);
            SetProviderServices("System.Data.SQLite", (System.Data.Entity.Core.Common.DbProviderServices)SQLiteProviderFactory.Instance.GetService(typeof(System.Data.Entity.Core.Common.DbProviderServices)));
        }
    }
}