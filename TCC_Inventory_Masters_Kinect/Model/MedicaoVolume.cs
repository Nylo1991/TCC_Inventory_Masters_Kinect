using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TCC_Inventory_Masters_Kinect.Model
{
    /// <summary>
    /// Representa uma medição volumétrica realizada pelo Kinect.
    /// </summary>
    public class MedicaoVolume
    {
        public int Id { get; set; }

        public double VolumeCm3 { get; set; }

        [NotMapped]
        public double VolumeM3 => VolumeCm3 / 1000000.0;

        public DateTime DataHora { get; set; }

        public bool KinectLigado { get; set; }

        [NotMapped]
        public string KinectTexto => KinectLigado
            ? "Ligado"
            : "Desligado";

        public bool Calibrado { get; set; }

        [NotMapped]
        public string CalibradoTexto => Calibrado
            ? "Sim"
            : "Não";

        public string Status { get; set; }

        public string Usuario { get; set; }

        public string Empresa { get; set; }

        public string NomeEspaco { get; set; }

        public double LimiteOcupacaoPercentual { get; set; }
    }
}