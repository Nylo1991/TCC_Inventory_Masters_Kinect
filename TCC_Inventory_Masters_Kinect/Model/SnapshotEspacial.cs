using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCC_Inventory_Masters_Kinect.Model
{
    public class SnapshotEspacial
    {
        public int Id { get; set; }

        public int EspacoMapeadoId { get; set; }

        public string NomeSnapshot { get; set; }

        public string CaminhoArquivo { get; set; }

        public double VolumeAtualCm3 { get; set; }

        public double VolumeMaximoCm3 { get; set; }

        public double PercentualOcupacao { get; set; }

        public double EspacoLivreCm3 { get; set; }

        public string CaminhoImagemRGB { get; set; }

        public string CaminhoImagemDepth { get; set; }

        public string Observacao { get; set; }

        public string Status { get; set; }

        public DateTime DataCaptura { get; set; }
    }
}
