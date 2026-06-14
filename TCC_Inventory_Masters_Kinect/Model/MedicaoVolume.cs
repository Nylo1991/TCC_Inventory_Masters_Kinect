using System;

namespace TCC_Inventory_Masters_Kinect.Model
{
    public class MedicaoVolume
    {
        public int Id { get; set; }

        public double VolumeCm3 { get; set; }

        public double VolumeM3
        {
            get { return VolumeCm3 / 1000000.0; }
        }

        public DateTime DataHora { get; set; }

        public bool KinectLigado { get; set; }

        public bool Calibrado { get; set; }

        public string Status { get; set; }
    }
}