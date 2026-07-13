using System;

namespace TCC_Inventory_Masters_Kinect.Model
{
    /// <summary>
    /// Representa o histórico de ocupação de um espaço monitorado.
    /// </summary>
    public class HistoricoOcupacao
    {
        public int Id { get; set; }
        public int MedicaoVolumeId { get; set; }
        public double VolumeAtualCm3 { get; set; }
        public double VolumeMaximoCm3 { get; set; }
        public double EspacoLivreCm3 { get; set; }
        public double PercentualOcupacao { get; set; }
        public bool LimiteUltrapassado { get; set; }
        public string NivelOcupacao { get; set; }
        public string Status { get; set; }
        public DateTime DataHora { get; set; }
        public string Empresa { get; set; }
    }
}