using InventoryMasters.Core.DTOs;

namespace InventoryMasters.Core.Interfaces;

/// <summary>
/// Contrato para manipulação de medições.
/// </summary>
public interface IMedicaoService
{
    Task RegistrarAsync(MedicaoDto dto);

    Task<List<MedicaoDto>> ListarAsync();
}