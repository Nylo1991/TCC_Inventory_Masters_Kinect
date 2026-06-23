using System;

namespace TCC_Inventory_Masters_Kinect.Model
{
    /// <summary>
    /// class Calibration Result é usada para armazenar os resultados da calibração do Kinect,
    /// incluindo o volume máximo detectado,o número total de pontos encontrados e a data/hora da calibração.
    /// </summary>
    public class CalibrationResult
    {
        public double MaxVolume { get; set; }
        public int TotalPointsFound { get; set; }
        public DateTime CalibratedAt { get; set; }
    }
}

