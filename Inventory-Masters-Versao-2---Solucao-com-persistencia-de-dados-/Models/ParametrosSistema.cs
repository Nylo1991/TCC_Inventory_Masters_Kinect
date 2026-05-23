namespace InventoryMaster.Models;

public class ParametrosSistema
{
    public int Id { get; set; }
    
    public decimal Volume_Maximo{ get; set; }
    
    public decimal Volume_Minimo{ get; set; }

    public DateTime? Data_Atualizacao { get; set; }

    public bool Email_Notificacao_Ativo { get; set; }
}