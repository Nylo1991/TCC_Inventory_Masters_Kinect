using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace TCC_Inventory_Masters_Kinect.Model
{
    public class UsuarioAcesso
    {
        public int Id { get; set; }
        public string Usuario { get; set; }
        public string Senha { get; set; }
        public string Perfil { get; set; }
        public DateTime CriadoEm { get; set; }
        public bool Ativo { get; set; }
    }
}
