using System;

namespace TCC_Inventory_Masters_Kinect.Model
{
    public class UsuarioAcesso
    {
        public int Id { get; set; }
        public string Usuario { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public string Perfil { get; set; }
        public DateTime CriadoEm { get; set; }
        public bool Ativo { get; set; }
    }
}