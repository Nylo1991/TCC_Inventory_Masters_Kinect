using System.Security.Claims;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.SignalR;
using MVC_InventoryMasters.Models;
using MVC_InventoryMasters.Repositories;

namespace MVC_InventoryMasters.Diulie.Tests.TestDoubles;

internal sealed record HubCall(string Method, object?[] Arguments);

// Suporte compartilhado pelos cenários de Diulie.
internal sealed class ClientProxyFake : IClientProxy
{
    public List<HubCall> Calls { get; } = new();

    public Task SendCoreAsync(
        string method,
        object?[] args,
        CancellationToken cancellationToken = default)
    {
        Calls.Add(new HubCall(method, args));
        return Task.CompletedTask;
    }
}

internal sealed class HubCallerClientsFake : IHubCallerClients
{
    public ClientProxyFake AllProxy { get; } = new();
    public ClientProxyFake CallerProxy { get; } = new();
    public ClientProxyFake OthersProxy { get; } = new();

    public IClientProxy All => AllProxy;
    public IClientProxy Caller => CallerProxy;
    public IClientProxy Others => OthersProxy;
    public IClientProxy AllExcept(IReadOnlyList<string> excludedConnectionIds) => AllProxy;
    public IClientProxy Client(string connectionId) => AllProxy;
    public IClientProxy Clients(IReadOnlyList<string> connectionIds) => AllProxy;
    public IClientProxy Group(string groupName) => AllProxy;
    public IClientProxy GroupExcept(string groupName, IReadOnlyList<string> excludedConnectionIds) => AllProxy;
    public IClientProxy Groups(IReadOnlyList<string> groupNames) => AllProxy;
    public IClientProxy OthersInGroup(string groupName) => OthersProxy;
    public IClientProxy User(string userId) => AllProxy;
    public IClientProxy Users(IReadOnlyList<string> userIds) => AllProxy;
}

internal sealed class HubCallerContextFake : HubCallerContext
{
    private readonly CancellationTokenSource _connectionAborted = new();
    private readonly IDictionary<object, object?> _items =
        new Dictionary<object, object?>();

    public override string ConnectionId { get; } = "conexao-teste";
    public override string? UserIdentifier { get; } = "usuario-teste";
    public override ClaimsPrincipal? User { get; } = new(new ClaimsIdentity());
    public override IDictionary<object, object?> Items => _items;
    public override IFeatureCollection Features { get; } = new FeatureCollection();
    public override CancellationToken ConnectionAborted => _connectionAborted.Token;

    public override void Abort() => _connectionAborted.Cancel();
}

internal sealed class MedicaoVolumeRepositoryFake : IMedicaoVolumeRepository
{
    public Task<List<MedicaoVolume>> ListarTodos() => throw new NotSupportedException();
    public Task<List<MedicaoVolume>> ListarPorEmpresa(string? empresaId = null) => throw new NotSupportedException();
    public Task<List<MedicaoVolume>> FiltrarAvancado(string origem, string status, DateTime? inicio, DateTime? fim) => throw new NotSupportedException();
    public Task<MedicaoSummary> ObterSummary() => throw new NotSupportedException();
    public List<MedicaoVolume> Adicionadas { get; } = new();
    public Exception? Excecao { get; set; }

    public Task Adicionar(MedicaoVolume medicao)
    {
        if (Excecao != null)
            throw Excecao;

        Adicionadas.Add(medicao);
        return Task.CompletedTask;
    }
}

internal sealed class ParametrosSistemaRepositoryFake : IParametrosSistemaRepository
{
    public string? EmpresaConsultada { get; private set; }
    public ParametrosSistema BuscarPorEmpresa(string empresaId)
    {
        EmpresaConsultada = empresaId;
        return Resultado!;
    }
    public ParametrosSistema ObterPadroes() => throw new NotSupportedException();
    public void Salvar(ParametrosSistema parametros) => throw new NotSupportedException();
    public double CalcularPercentualOcupacao(double volumeAtual, double capacidadeMaxima) => throw new NotSupportedException();
    public ParametrosSistema? Resultado { get; set; } = new()
    {
        CapacidadeMaxima = 10,
        PercentualAlerta = 80
    };

    public ParametrosSistema Buscar() => Resultado!;
}

internal sealed class NotificacaoRepositoryFake : INotificacaoRepository
{
    public Task<List<Notificacao>> ListarTodos() => throw new NotSupportedException();
    public Task<List<Notificacao>> ListarPorEmpresa(string? empresaId = null) => throw new NotSupportedException();
    public Task<bool> AtualizarStatus(string id, string status) => throw new NotSupportedException();
    public bool ExistePendente { get; set; }
    public Exception? ExcecaoAoAdicionar { get; set; }
    public List<Notificacao> Adicionadas { get; } = new();

    public Task Adicionar(Notificacao notificacao)
    {
        if (ExcecaoAoAdicionar != null)
            throw ExcecaoAoAdicionar;

        Adicionadas.Add(notificacao);
        return Task.CompletedTask;
    }

    public Task<bool> ExisteNotificacaoPendente(string? empresaId = null) =>
        Task.FromResult(ExistePendente);
}
