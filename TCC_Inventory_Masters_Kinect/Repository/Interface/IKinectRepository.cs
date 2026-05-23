using TCC_Inventory_Masters_Kinect.Model;

namespace TCC_Inventory_Masters_Kinect.Repository.Interface
{
    public interface IKinectRepository
    {
        void SalvarMedicao(MedicaoVolume medicao);
    }
}