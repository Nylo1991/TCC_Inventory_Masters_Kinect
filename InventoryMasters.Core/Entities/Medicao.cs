namespace InventoryMasters.Core.Entities;

/// <summary>
/// Representa uma medição recebida pelo sistema.
/// </summary>
public class Medicao
{
    // Identificador único
    public string Id { get; set; } = Guid.NewGuid().ToString();

    // Área medida em metros quadrados
    public decimal AreaM2 { get; set; }

    // Data da leitura
    public DateTime DataLeitura { get; set; }

    // Nome do parceiro responsável
    public string ParceiroId { get; set; } = string.Empty;
}