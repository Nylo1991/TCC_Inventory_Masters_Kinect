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
            try
            {
                using (var db = new AppDbContext())
                {
                    db.MedicaoVolumes.Add(medicao);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao salvar: " + ex.Message);
            }
        }
    }
}