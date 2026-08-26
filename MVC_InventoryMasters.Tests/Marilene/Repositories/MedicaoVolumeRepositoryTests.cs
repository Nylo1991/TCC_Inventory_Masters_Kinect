using Microsoft.Extensions.Logging.Abstractions;
using MVC_InventoryMasters.Models;
using MVC_InventoryMasters.Repositories;
using MVC_InventoryMasters.Services;
using MVC_InventoryMasters.Tests.Infrastructure;

namespace MVC_InventoryMasters.Tests.Marilene.Repositories;

// Cenários de Marilene adaptados para dados sintéticos e transporte em memória.
[Trait("Integrante", "Marilene")]

public class MedicaoVolumeRepositoryTests
{
    private readonly FirestoreMemory db = new();
    private MedicaoVolumeRepository Repo => new(db.Firebase, db.Context);
    private static readonly DateTime Dia = new(2026, 1, 15, 12, 0, 0, DateTimeKind.Utc);
    private Task Seed(string id = "m1", string? empresa = "empresa-a", double volume = 10) =>
        db.Seed("Medicoes", id, new MedicaoVolume { EmpresaId = empresa, VolumeMedido = volume, OrigemLeitura = "Kinect", Status = "Normal", DataHora = Dia });
    [Theory] [InlineData(null, "empresa-a")] [InlineData("empresa-b", "empresa-b")]
    public async Task Adicionar_AtribuiOuPreservaEmpresa(string? empresa, string esperado)
    {
        var antes = DateTime.UtcNow.AddSeconds(-1);
        await Repo.Adicionar(new MedicaoVolume { EmpresaId = empresa, VolumeMedido = 25 });
        var m = Assert.Single(await Repo.ListarTodos());
        Assert.Equal(esperado, m.EmpresaId); Assert.Equal(25d, m.VolumeMedido);
        Assert.InRange(m.DataHora!.Value, antes, DateTime.UtcNow);
    }
    [Fact] public async Task Adicionar_Erro_PropagaExcecao()
    { db.Failure = new InvalidOperationException("falha simulada"); Assert.Same(db.Failure, await Assert.ThrowsAsync<InvalidOperationException>(() => Repo.Adicionar(new MedicaoVolume()))); }
    [Fact] public async Task ListarTodos_ComDados_RetornaIds()
    { await Seed(); Assert.Equal("m1", Assert.Single(await Repo.ListarTodos()).Id); }
    // A implementação propaga falhas; o teste original esperava lista vazia sem base no contrato.
    [Fact] public async Task ListarTodos_Erro_PropagaExcecao()
    { db.Failure = new InvalidOperationException(); Assert.Same(db.Failure, await Assert.ThrowsAsync<InvalidOperationException>(() => Repo.ListarTodos())); }
    [Fact] public async Task ListarPorEmpresa_IsolaRegistros()
    { await Seed(); await Seed("m2", "empresa-b"); Assert.Equal("m1", Assert.Single(await Repo.ListarPorEmpresa()).Id); }
    [Fact] public async Task ListarPorEmpresa_Global_IncluiLegados()
    { await Seed(); await Seed("global", "global"); await Seed("legado", null); Assert.Equal(new[] { "global", "legado" }, (await Repo.ListarPorEmpresa("global")).Select(m => m.Id).OrderBy(id => id)); }
    [Fact] public async Task FiltrarAvancado_OrigemEStatus_SelecionaSomenteCorrespondentes()
    {
        await Seed();
        await db.Seed("Medicoes", "manual", new MedicaoVolume { EmpresaId = "empresa-a", OrigemLeitura = "Manual", Status = "Normal" });
        await db.Seed("Medicoes", "alerta", new MedicaoVolume { EmpresaId = "empresa-a", OrigemLeitura = "Kinect", Status = "Alerta" });
        Assert.Equal("m1", Assert.Single(await Repo.FiltrarAvancado("kINECT", "NORMAL", null, null)).Id);
    }
    [Fact] public async Task FiltrarAvancado_Periodo_IncluiDiaLimiteEExcluiFora()
    {
        await Seed();
        await db.Seed("Medicoes", "antiga", new MedicaoVolume { EmpresaId = "empresa-a", DataHora = Dia.AddDays(-1) });
        await db.Seed("Medicoes", "sem-data", new MedicaoVolume { EmpresaId = "empresa-a", DataHora = null });
        Assert.Equal("m1", Assert.Single(await Repo.FiltrarAvancado("", "", Dia, Dia)).Id);
    }
    [Fact] public async Task ObterSummary_CalculaApenasEmpresaAtual()
    {
        await Seed(volume: 10); await Seed("m2", volume: 30); await Seed("outra", "empresa-b", 999);
        var s = await Repo.ObterSummary();
        Assert.Equal(2, s.TotalMedicoes); Assert.Equal(20d, s.MediaVolume); Assert.Equal(10d, s.MinVolume); Assert.Equal(30d, s.MaxVolume);
    }
    [Fact] public async Task ObterSummary_Vazio_RetornaZeros()
    { var s = await Repo.ObterSummary(); Assert.Equal(0, s.TotalMedicoes); Assert.Equal(0d, s.MediaVolume); Assert.Equal(0d, s.MinVolume); Assert.Equal(0d, s.MaxVolume); }
}

