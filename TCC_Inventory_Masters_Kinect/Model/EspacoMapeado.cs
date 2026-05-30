using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCC_Inventory_Masters_Kinect.Model
{
    public class EspacoMapeado
    {
        public int Id { get; set; }

        public string NomeEspaco { get; set; }

        public double LarguraMetros { get; set; }

        public double AlturaMetros { get; set; }

        public double ProfundidadeMetros { get; set; }

        public double VolumeTotalCm3 { get; set; }

        public double VolumeMaximoPermitidoCm3 { get; set; }

        public double VolumeAtualCm3 { get; set; }

        public double PercentualOcupacao { get; set; }

        public double EspacoLivreCm3 { get; set; }

        public bool Ativo { get; set; }

        public bool MapeamentoConcluido { get; set; }

        public string Status { get; set; }

        public DateTime DataCriacao { get; set; }

        public DateTime? DataUltimaAtualizacao { get; set; }
    }
}

