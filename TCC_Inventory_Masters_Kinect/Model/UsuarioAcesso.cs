using System;

namespace TCC_Inventory_Masters_Kinect.Model
{
    /// <summary>
    /// Classe resposnável por representar os dados de acesso do usuário.
    /// </summary>
    public class UsuarioAcesso
    {
        public int Id { get; set; }
        public string Usuario { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public string Perfil { get; set; }
        public DateTime CriadoEm { get; set; }
        public bool Ativo { get; set; }
        public string Empresa { get; set; }
    }
}