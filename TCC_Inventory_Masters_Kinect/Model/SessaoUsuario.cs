namespace TCC_Inventory_Masters_Kinect.Model
{
   
    public class SessaoUsuario
    {
        /// <summary>
        /// Classe de conexão para a empresa que foi registrada dentro do MVC tenha acesso ao sistema.
        /// </summary>
        public string Usuario { get; set; }
        public string Empresa { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
    }
}