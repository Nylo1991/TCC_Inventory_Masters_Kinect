using System;

namespace MVC_InventoryMasters.Models
{
    public class MedicaoVolume
    {
        public int Id { get; set; }

        public double VolumeCm3 { get; set; }

        public DateTime DataHora { get; set; }
        
        public bool KinectLigado { get; set; }
       
        public bool Calibrado { get; set; }
       
        public string? Status { get; set; }
    }
}