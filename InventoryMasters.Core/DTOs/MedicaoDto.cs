namespace InventoryMasters.Core.DTOs;

/// <summary>
/// Dados transferidos entre aplicações.
/// </summary>
public class MedicaoDto
{
    public decimal AreaM2 { get; set; }

    public DateTime DataLeitura { get; set; }
}