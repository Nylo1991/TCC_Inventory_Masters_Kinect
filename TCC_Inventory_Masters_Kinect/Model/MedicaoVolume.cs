using System;


namespace TCC_Inventory_Masters_Kinect.Model
{
    /// <summary>
    /// Modelo de dados para representar uma medição volumétrica realizada pelo Kinect.
    /// </summary>
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
        public string Usuario { get; set; }
        public string Empresa { get; set; }
    }
}