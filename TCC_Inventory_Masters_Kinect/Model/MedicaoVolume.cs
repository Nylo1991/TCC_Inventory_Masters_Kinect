using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCC_Inventory_Masters_Kinect.Model
{
    public class MedicaoVolume
    {
        public int Id { get; set; }
        public double VolumeCm3 { get; set; }

        public DateTime DataHora { get; set; }

        public bool KinectLigado { get; set; }

        public bool Calibrado { get; set; }

        public string Status { get; set; }
    }
}
