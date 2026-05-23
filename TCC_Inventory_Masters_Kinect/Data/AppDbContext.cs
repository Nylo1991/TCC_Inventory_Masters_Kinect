using System.Data.Entity;
using TCC_Inventory_Masters_Kinect.Model;

namespace TCC_Inventory_Masters_Kinect.Data
{
    public class AppDbContext : DbContext
    {
        // O nome "InventoryMastersDb" liga este código ao App.config
        public AppDbContext() : base("name=InventoryMastersDb")
        {
            // Estratégia para SQLite: Se o banco não existir, ele cria automaticamente
            // com base nas classes DbSet (como MedicaoVolume)
            Database.SetInitializer(new CreateDatabaseIfNotExists<AppDbContext>());
        }

        public DbSet<MedicaoVolume> MedicoesVolume { get; set; }
    }
}