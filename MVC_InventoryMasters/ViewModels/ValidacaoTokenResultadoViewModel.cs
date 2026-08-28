namespace MVC_InventoryMasters.ViewModels
{
    public class ValidacaoTokenResultadoViewModel
    {
        public bool TokenValido { get; set; }
        public bool EmailValidado { get; set; }
        public string? Usuario { get; set; }
        public string? Empresa { get; set; }
        public string? EmpresaId { get; set; }
        public string? Email { get; set; }
        public string? Mensagem { get; set; }
    }
}
