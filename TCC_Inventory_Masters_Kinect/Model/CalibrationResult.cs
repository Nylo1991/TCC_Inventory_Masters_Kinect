using System;

namespace TCC_Inventory_Masters_Kinect.Model
{
    public class CalibrationResult
    {
        public double MaxVolume { get; set; }
        public int TotalPointsFound { get; set; }
        public DateTime CalibratedAt { get; set; }
    }
}

