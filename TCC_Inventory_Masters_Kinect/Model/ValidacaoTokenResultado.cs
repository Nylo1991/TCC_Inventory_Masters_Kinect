namespace TCC_Inventory_Masters_Kinect.Model
{

    /// <summary>
    /// Classe para validar se de fato o token enviado pelo o MVC e valido .
    /// </summary>
    public class ValidacaoTokenResultado
    {
        public bool TokenValido { get; set; }
        public bool EmailValidado { get; set; }
        public string Usuario { get; set; }
        public string Empresa { get; set; }
        public string EmpresaId { get; set; }
        public string Email { get; set; }
        public string Mensagem { get; set; }
    }
}
