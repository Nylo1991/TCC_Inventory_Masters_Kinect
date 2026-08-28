using MVC_InventoryMasters.Models;

namespace MVC_InventoryMasters.Repositories;

public interface IMedicaoVolumeRepository
{
    Task Adicionar(MedicaoVolume medicao);
    Task<List<MedicaoVolume>> ListarTodos();
    Task<List<MedicaoVolume>> ListarPorEmpresa(string? empresaId = null);
    Task<List<MedicaoVolume>> FiltrarAvancado( string origem, string status, DateTime? dataInicio, DateTime? dataFim);
    Task<MedicaoSummary> ObterSummary();
}

public interface INotificacaoRepository
{
    Task Adicionar(Notificacao notif);
    Task<List<Notificacao>> ListarTodos();
    Task<List<Notificacao>> ListarPorEmpresa(string? empresaId = null);
    Task<bool> AtualizarStatus(string id, string novoStatus);
    Task<bool> ExisteNotificacaoPendente(string? empresaId = null);
}

public interface IParametrosSistemaRepository
{
    ParametrosSistema Buscar();
    ParametrosSistema BuscarPorEmpresa(string empresaId);
    ParametrosSistema ObterPadroes();
    void Salvar(ParametrosSistema parametros);
    double CalcularPercentualOcupacao( double volumeAtual, double capacidadeMaxima);
}

public interface IParceirosRepository
{
    Task<List<Parceiro>> ListarTodos();
    Task<List<Parceiro>> ListarPorEmpresa(string? empresaId = null);
    Task<Parceiro?> BuscarPorId(string id);
    Task<List<Parceiro>> Pesquisar(string termo);
    Task<List<Parceiro>> FiltrarAvancado(string termo, DateTime? dataInicio, DateTime? dataFim, bool? ativo);
    Task Adicionar(Parceiro parceiro);
    Task Atualizar(Parceiro parceiro);
    Task Excluir(string id);
    Task AtualizarStatus(string id, bool ativo);
}

public interface IPerfisRepository
{
    Task<List<Perfil>> ListarTodos();
    Task<List<Perfil>> ListarPorEmpresa(string? empresaId = null);
    Task<Perfil?> BuscarPorId(string id);
    Task Adicionar(Perfil perfil);
    Task Atualizar(Perfil perfil);
    Task Inativar(string id);
}

public interface IUsuariosRepository
{
    Task<List<Usuario>> ListarTodos();
    Task<List<Usuario>> ListarPorEmpresa(string? empresaId = null);
    Task<Usuario?> BuscarPorId(string id);
    Task<Usuario?> BuscarPorEmail(string email);
    Task Adicionar(Usuario usuario);
    Task Atualizar(Usuario usuario);
    Task Excluir(string id);
    Task AtualizarStatus(string id, bool ativo);
}

