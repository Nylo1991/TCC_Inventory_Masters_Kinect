using System;

namespace TCC_Inventory_Masters_Kinect.Model
{
    public class Log
    {
        public int Id { get; set; }
        public DateTime DataHora { get; set; } = DateTime.Now;
        public string Nivel { get; set; }
        public string Mensagem { get; set; }
    }
}