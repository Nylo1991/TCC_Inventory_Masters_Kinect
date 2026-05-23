namespace InventoryMaster.Models;

public class MedicaoVolume
{
    public int Id { get; set; }

    public string Origem_Leitura { get; set; }
    
    public decimal Volume_Medido { get; set; }

    public DateTime? Data_Hora { get; set; }

    public int fk_Usuario_Id { get; set; }
}