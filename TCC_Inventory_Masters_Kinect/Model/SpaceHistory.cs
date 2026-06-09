using System;

namespace TCC_Inventory_Masters_Kinect.Model
{
    public class SpaceHistory
    {
        public int Id { get; set; }
        public int SpaceId { get; set; }
        public double CurrentVolume { get; set; }
        public double Percentage { get; set; }
        public DateTime RecordedAt { get; set; }

        public Space Space { get; set; }
    }
}
