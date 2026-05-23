namespace InventoryMaster.Models;

public class Notificacao
{
    public int Id { get; set; }
    
    public int Quantidade_destinatario { get; set; }

    public string Mensagem { get; set; }
    
    public string Status_Envio { get; set; }
    
    public decimal Volume_Momento { get; set; }

    public DateTime? Data_Envio { get; set; }

    public int fk_Usuario_Id { get; set; }
}