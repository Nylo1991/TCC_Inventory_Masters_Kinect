namespace InventoryMasters.Core.Events;

/// <summary>
/// Evento disparado ao receber uma nova medição.
/// </summary>
public class MedicaoRecebidaEvent
{
    public decimal AreaM2 { get; set; }

    public DateTime DataLeitura { get; set; }
}