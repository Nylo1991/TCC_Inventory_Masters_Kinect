namespace InventoryMaster.Models;

public class NotificacaoParceiros
{
    public int Id { get; set; }
    
    public string Status_Entrega { get; set; }

    public int fk_Parceiro_Id { get; set; }
    
    public int fk_Notificacao_Id { get; set; }
}