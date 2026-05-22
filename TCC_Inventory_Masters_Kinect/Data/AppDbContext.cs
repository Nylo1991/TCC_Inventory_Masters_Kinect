using InventoryMastersKinect.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using TCC_Inventory_Masters_Kinect.Model;

namespace InventoryMastersKinect.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<MedicaoVolume> MedicoesVolume { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // O arquivo do banco será gerado automaticamente na raiz do projeto
            optionsBuilder.UseSqlite("Data Source=inventory_masters.db");
        }
    }
}