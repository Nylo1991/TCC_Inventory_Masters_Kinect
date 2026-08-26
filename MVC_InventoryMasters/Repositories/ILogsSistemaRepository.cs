namespace MVC_InventoryMasters.Repositories;

public interface ILogsSistemaRepository
{
    Task Registrar(string acao, string mensagem, string nivel = "Informacao",
        string? email = null, string? usuarioId = null, string? empresaId = null);
}
