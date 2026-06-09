using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCC_Inventory_Masters_Kinect.Model
{
    public class Space
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double MaxVolume { get; set; }
        public DateTime CalibratedAt { get; set; }

        public ICollection<SpaceHistory> History { get; set; } = new List<SpaceHistory>();
    }
}


   
}
