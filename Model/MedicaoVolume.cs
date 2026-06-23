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

        public string KinectTexto
        {
            get { return KinectLigado ? "Ligado" : "Desligado"; }
        }

        public bool Calibrado { get; set; }

        public string CalibradoTexto
        {
            get { return Calibrado ? "Sim" : "Nao"; }
        }

        public string Status { get; set; }

        public string Usuario { get; set; }

        public string Empresa { get; set; }

        public string NomeEspaco { get; set; }

        public double LimiteOcupacaoPercentual { get; set; }
    }
}