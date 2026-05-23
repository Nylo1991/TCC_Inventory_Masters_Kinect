using System;
using TCC_Inventory_Masters_Kinect.Data;
using TCC_Inventory_Masters_Kinect.Model;
using TCC_Inventory_Masters_Kinect.Repository.Interface;

namespace TCC_Inventory_Masters_Kinect.Repository
{
    public class KinectRepository : IKinectRepository
    {
        public void SalvarMedicao(MedicaoVolume medicao)
        {
            // O "using" garante que a conexão com o SQLite seja fechada após salvar
            using (var context = new AppDbContext())
            {
                context.MedicoesVolume.Add(medicao);
                context.SaveChanges();
            }
        }
    }
}