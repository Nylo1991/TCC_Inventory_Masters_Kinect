using Microsoft.Extensions.Logging.Abstractions;
using MVC_InventoryMasters.Models;
using MVC_InventoryMasters.Repositories;
using MVC_InventoryMasters.Services;
using MVC_InventoryMasters.Tests.Infrastructure;

namespace MVC_InventoryMasters.Tests.Marilene.Repositories;

// Cenários de Marilene adaptados para dados sintéticos e transporte em memória.
[Trait("Integrante", "Marilene")]

public class NotificacaoRepositoryTests
{
    private readonly FirestoreMemory db = new();
    private NotificacaoRepository Repo => new(db.Firebase, NullLogger<NotificacaoRepository>.Instance, db.Context);
    private static readonly DateTime Dia = new(2026, 1, 15, 12, 0, 0, DateTimeKind.Utc);
    private Task Seed(string id = "n1", string? empresa = "empresa-a", string status = "Pendente", int dias = 0) =>
        db.Seed("Notificacoes", id, new Notificacao { EmpresaId = empresa, Mensagem = "Alerta", StatusEnvio = status, DataHora = Dia.AddDays(dias) });
    [Theory] [InlineData(null, "empresa-a")] [InlineData("empresa-b", "empresa-b")]
    public async Task Adicionar_AtribuiOuPreservaEmpresa(string? empresa, string esperado)
    {
        await Repo.Adicionar(new Notificacao { EmpresaId = empresa, Mensagem = "Alerta" });
        var n = Assert.Single(await Repo.ListarTodos());
        Assert.Equal(esperado, n.EmpresaId); Assert.Equal("Alerta", n.Mensagem); Assert.False(string.IsNullOrEmpty(n.Id));
    }
    [Fact] public async Task Adicionar_Erro_LancaMensagem()
    { db.Failure = new InvalidOperationException(); Assert.Equal("Ocorreu um erro ao registrar a notificação.", (await Assert.ThrowsAsync<Exception>(() => Repo.Adicionar(new Notificacao()))).Message); }
    [Fact] public async Task ListarTodos_OrdenaMaisRecentePrimeiro()
    { await Seed(); await Seed("n2", dias: 1); Assert.Equal(new[] { "n2", "n1" }, (await Repo.ListarTodos()).Select(n => n.Id)); }
    [Fact] public async Task ListarTodos_Erro_RetornaVazio()
    { db.Failure = new InvalidOperationException(); Assert.Empty(await Repo.ListarTodos()); }
    [Fact] public async Task ListarPorEmpresa_IsolaDados()
    { await Seed(); await Seed("outra", "empresa-b"); Assert.Equal("n1", Assert.Single(await Repo.ListarPorEmpresa()).Id); }
    [Fact] public async Task ListarPorEmpresa_Global_IncluiLegados()
    { await Seed(); await Seed("global", "global"); await Seed("legado", null); Assert.Equal(new[] { "global", "legado" }, (await Repo.ListarPorEmpresa("global")).Select(n => n.Id).OrderBy(id => id)); }
    [Fact] public async Task AtualizarStatus_PersisteERetornaTrue()
    { await Seed(); Assert.True(await Repo.AtualizarStatus("n1", "Enviado")); Assert.Equal("Enviado", (await db.Read<Notificacao>("Notificacoes", "n1")).StatusEnvio); }
    [Fact] public async Task AtualizarStatus_Erro_RetornaFalse()
    { db.Failure = new InvalidOperationException(); Assert.False(await Repo.AtualizarStatus("n1", "Enviado")); }
    [Fact] public async Task ExistePendente_FiltraEmpresaEStatus()
    {
        await Seed("outra", "empresa-b"); await Seed("enviada", status: "Enviado");
        Assert.False(await Repo.ExisteNotificacaoPendente());
        await db.Seed("Notificacoes", "recente", new Notificacao
        {
            EmpresaId = "empresa-a",
            Mensagem = "Alerta recente",
            StatusEnvio = "Pendente",
            DataHora = DateTime.UtcNow
        });
        Assert.True(await Repo.ExisteNotificacaoPendente());
    }
    [Fact] public async Task ExistePendente_IgnoraPendenciaAntiga()
    {
        await Seed("antiga", "empresa-a", "Pendente");
        Assert.False(await Repo.ExisteNotificacaoPendente());
    }
}
