using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCC_Inventory_Masters_Kinect.Model;

namespace TCC_Inventory_Masters_Kinect.Repository.Interface
{
    public interface IKinectRepository
    {
        void SalvarMedicao(MedicaoVolume medicao);
    }
}
