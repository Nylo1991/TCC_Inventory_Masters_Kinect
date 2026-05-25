namespace InventoryMaster.Models;

public class Parceiro
{
    public int Id { get; set; }

    public string Nome { get; set; }

    public string Email { get; set; }

    public string Empresa { get; set; }

    public string Telefone { get; set; }
    
    public string Endereco { get; set; }

    public DateTime? Data_Cadastro { get; set; }

<<<<<<< HEAD

=======
>>>>>>> parent of cc72f7d (Alteração do MVC conectado com o Firebase)
    public bool Ativo { get; set; }
}