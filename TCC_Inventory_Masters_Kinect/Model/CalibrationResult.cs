using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCC_Inventory_Masters_Kinect.Model
{
    public class CalibrationResult
    {
        public double MaxVolume { get; set; }
        public int TotalPointsFound { get; set; }
        public DateTime CalibratedAt { get; set; }
    }

}



