using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCC_Inventory_Masters_Kinect.Model
{
    public class Point3DData
    {
        public int Id { get; set; }

        public int EspacoMapeadoId { get; set; }

        public double X { get; set; }

        public double Y { get; set; }

        public double Z { get; set; }

        public double Distancia { get; set; }

        public int PixelX { get; set; }

        public int PixelY { get; set; }

        public byte R { get; set; }

        public byte G { get; set; }

        public byte B { get; set; }

        public string TipoObjeto { get; set; }

        public DateTime DataCaptura { get; set; }
    }
}
