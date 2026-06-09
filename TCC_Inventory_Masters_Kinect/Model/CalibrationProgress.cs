using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCC_Inventory_Masters_Kinect.Model
{
    public class CalibrationProgress
    {
        public int CurrentAngle { get; set; }
        public string CurrentPosition { get; set; }
        public int Step { get; set; }
        public int TotalSteps { get; set; }
        public string Status { get; set; }
        public int Percentage { get; set; }
    }
}
