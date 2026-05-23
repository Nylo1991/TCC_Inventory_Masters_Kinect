using System.Data.Entity;
using TCC_Inventory_Masters_Kinect.Model;

namespace TCC_Inventory_Masters_Kinect.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() : base("InventoryMastersDb")
        {
        }

        public DbSet<MedicaoVolume> MedicaoVolumes { get; set; }
    }
}