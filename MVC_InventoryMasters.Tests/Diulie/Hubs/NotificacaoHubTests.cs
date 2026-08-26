using MVC_InventoryMasters.Hubs;
using MVC_InventoryMasters.Diulie.Tests.TestDoubles;

namespace MVC_InventoryMasters.Diulie.Tests.Hubs;

[Xunit.Trait("Integrante", "Diulie")]
public class NotificacaoHubTests
{
    [Fact]
    public async Task EnviarNotificacao_DeveEnviarMensagemParaTodos()
    {
        var clientes = new HubCallerClientsFake();
        var hub = new NotificacaoHub
        {
            Clients = clientes,
            Context = new HubCallerContextFake()
        };

        await hub.EnviarNotificacao("Estoque em nível crítico.");

        var chamada = Assert.Single(clientes.AllProxy.Calls);
        Assert.Equal("ReceberNotificacao", chamada.Method);
        Assert.Equal("Estoque em nível crítico.", Assert.Single(chamada.Arguments));
    }

    [Fact]
    public async Task CicloDeConexao_DeveConcluirSemErro()
    {
        var hub = new NotificacaoHub
        {
            Clients = new HubCallerClientsFake(),
            Context = new HubCallerContextFake()
        };

        await hub.OnConnectedAsync();
        await hub.OnDisconnectedAsync(null);
    }
}
