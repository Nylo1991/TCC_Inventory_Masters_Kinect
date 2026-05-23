using System.Data.Entity;
using TCC_Inventory_Masters_Kinect.Model;
using System.Data.SQLite; // Adicione este using

namespace TCC_Inventory_Masters_Kinect.Data
{
    // Adicionamos esta configuração para forçar o uso do SQLite
    [DbConfigurationType(typeof(System.Data.SQLite.EF6.SQLiteConfiguration))]
    public class AppDbContext : DbContext
    {
        public AppDbContext()
            : base("name=InventoryMastersDb") // O nome aqui deve bater com o App.config
        {
            // Isso garante que o banco seja criado se não existir
            Database.SetInitializer<AppDbContext>(null);
        }

        public DbSet<MedicaoVolume> MedicoesVolume { get; set; }
    }
}