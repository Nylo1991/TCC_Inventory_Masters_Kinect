using Microsoft.Extensions.Logging.Abstractions;
using MVC_InventoryMasters.Diulie.Tests.TestDoubles;
using MVC_InventoryMasters.Hubs;
using MVC_InventoryMasters.Models;

namespace MVC_InventoryMasters.Diulie.Tests.Hubs;

[Xunit.Trait("Integrante", "Diulie")]
public class MedicaoHubTests
{
    [Theory]
    [InlineData(0, 0)]
    [InlineData(1_000_000, 1)]
    [InlineData(2_500_000, 2.5)]
    public async Task EnviarVolume_DeveConverterSalvarETransmitir(
        double volumeCm3,
        double volumeM3Esperado)
    {
        var cenario = CriarCenario();

        await cenario.Hub.EnviarVolume(volumeCm3);

        var medicao = Assert.Single(cenario.Medicoes.Adicionadas);
        Assert.Equal(volumeM3Esperado, medicao.VolumeMedido);
        Assert.Equal("Kinect", medicao.OrigemLeitura);
        Assert.Equal("Normal", medicao.Status);
        Assert.NotNull(medicao.DataHora);

        var chamada = Assert.Single(cenario.Clientes.AllProxy.Calls);
        Assert.Equal("NovaMedicao", chamada.Method);
        var payload = Assert.Single(chamada.Arguments);
        var valorTransmitido = (double)payload!.GetType()
            .GetProperty("volumeMedido")!
            .GetValue(payload)!;
        Assert.Equal(volumeM3Esperado, valorTransmitido);
    }

    [Fact]
    public async Task EnviarVolume_AbaixoDoLimite_NaoDeveCriarNotificacao()
    {
        var cenario = CriarCenario(capacidadeMaxima: 10, percentualAlerta: 80);

        await cenario.Hub.EnviarVolume(7_900_000);

        Assert.Empty(cenario.Notificacoes.Adicionadas);
    }

    [Fact]
    public async Task EnviarVolume_NoLimite_DeveCriarNotificacaoPendente()
    {
        var cenario = CriarCenario(capacidadeMaxima: 10, percentualAlerta: 80);

        await cenario.Hub.EnviarVolume(8_000_000);

        var notificacao = Assert.Single(cenario.Notificacoes.Adicionadas);
        Assert.Equal(8, notificacao.VolumeMedido);
        Assert.Equal("Capacidade", notificacao.Tipo);
        Assert.True(notificacao.Automatica);
        Assert.Equal("Pendente", notificacao.StatusEnvio);
        Assert.Contains("80", notificacao.Mensagem);

        Assert.Contains(
            cenario.Clientes.AllProxy.Calls,
            chamada => chamada.Method == "NovaNotificacao");
    }

    [Fact]
    public async Task EnviarVolume_ComEmpresa_DeveUsarParametrosEGravarNoMesmoContexto()
    {
        var cenario = CriarCenario(capacidadeMaxima: 10, percentualAlerta: 80);

        await cenario.Hub.EnviarVolume(8_000_000, "empresa-teste");

        Assert.Equal("empresa-teste", cenario.Parametros.EmpresaConsultada);
        Assert.Equal("empresa-teste", Assert.Single(cenario.Medicoes.Adicionadas).EmpresaId);
        Assert.Equal("empresa-teste", Assert.Single(cenario.Notificacoes.Adicionadas).EmpresaId);
    }

    [Fact]
    public async Task EnviarVolume_DeveUsarCapacidadeELimiteInformadosPeloKinect()
    {
        var cenario = CriarCenario(capacidadeMaxima: 100, percentualAlerta: 95);

        await cenario.Hub.EnviarVolume(
            8_000_000,
            "empresa-teste",
            10_000_000,
            78);

        var notificacao = Assert.Single(cenario.Notificacoes.Adicionadas);
        Assert.Contains("80", notificacao.Mensagem);
    }

    [Fact]
    public async Task EnviarVolume_ComNotificacaoPendente_NaoDeveDuplicarAlerta()
    {
        var cenario = CriarCenario(capacidadeMaxima: 10, percentualAlerta: 80);
        cenario.Notificacoes.ExistePendente = true;

        await cenario.Hub.EnviarVolume(9_000_000);

        Assert.Empty(cenario.Notificacoes.Adicionadas);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(0.0)]
    [InlineData(-1.0)]
    public async Task EnviarVolume_ParametrosAusentesOuInvalidos_NaoDeveCriarAlerta(
        double? capacidadeMaxima)
    {
        var cenario = CriarCenario();
        cenario.Parametros.Resultado = capacidadeMaxima.HasValue
            ? new ParametrosSistema
            {
                CapacidadeMaxima = capacidadeMaxima.Value,
                PercentualAlerta = 80
            }
            : null;

        await cenario.Hub.EnviarVolume(9_000_000);

        Assert.Empty(cenario.Notificacoes.Adicionadas);
        Assert.Single(cenario.Clientes.AllProxy.Calls);
    }

    [Fact]
    public async Task EnviarVolume_QuandoPersistenciaFalha_DeveAvisarSomenteChamador()
    {
        var cenario = CriarCenario();
        cenario.Medicoes.Excecao = new InvalidOperationException("Falha simulada");

        await cenario.Hub.EnviarVolume(1_000_000);

        Assert.Contains(
            cenario.Clientes.AllProxy.Calls,
            chamada => chamada.Method == "NovaMedicao");
        var chamada = Assert.Single(cenario.Clientes.CallerProxy.Calls);
        Assert.Equal("ErroProcessamento", chamada.Method);
        Assert.Equal(
            "A medição foi exibida, mas não pôde ser salva no banco de dados.",
            Assert.Single(chamada.Arguments));
    }

    [Fact]
    public async Task EnviarVolume_QuandoNotificacaoFalha_DeveContinuarTransmitindoMedicao()
    {
        var cenario = CriarCenario(capacidadeMaxima: 10, percentualAlerta: 80);
        cenario.Notificacoes.ExcecaoAoAdicionar =
            new InvalidOperationException("Falha simulada");

        await cenario.Hub.EnviarVolume(9_000_000);

        Assert.Single(cenario.Medicoes.Adicionadas);
        Assert.Single(cenario.Clientes.AllProxy.Calls);
        Assert.Empty(cenario.Clientes.CallerProxy.Calls);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task CicloDeConexao_DeveConcluirComOuSemErro(bool desconectarComErro)
    {
        var cenario = CriarCenario();
        Exception? excecao = desconectarComErro
            ? new InvalidOperationException("Desconexão simulada")
            : null;

        await cenario.Hub.OnConnectedAsync();
        await cenario.Hub.OnDisconnectedAsync(excecao);
    }

    private static CenarioHub CriarCenario(
        double capacidadeMaxima = 10,
        int percentualAlerta = 80)
    {
        var medicoes = new MedicaoVolumeRepositoryFake();
        var parametros = new ParametrosSistemaRepositoryFake
        {
            Resultado = new ParametrosSistema
            {
                CapacidadeMaxima = capacidadeMaxima,
                PercentualAlerta = percentualAlerta
            }
        };
        var notificacoes = new NotificacaoRepositoryFake();
        var clientes = new HubCallerClientsFake();
        var hub = new MedicaoHub(
            medicoes,
            parametros,
            notificacoes,
            NullLogger<MedicaoHub>.Instance)
        {
            Clients = clientes,
            Context = new HubCallerContextFake()
        };

        return new CenarioHub(
            hub,
            medicoes,
            parametros,
            notificacoes,
            clientes);
    }

    private sealed record CenarioHub(
        MedicaoHub Hub,
        MedicaoVolumeRepositoryFake Medicoes,
        ParametrosSistemaRepositoryFake Parametros,
        NotificacaoRepositoryFake Notificacoes,
        HubCallerClientsFake Clientes);
}
