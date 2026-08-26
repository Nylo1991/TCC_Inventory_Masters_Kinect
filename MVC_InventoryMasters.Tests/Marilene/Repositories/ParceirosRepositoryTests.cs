using Microsoft.Extensions.Logging.Abstractions;
using MVC_InventoryMasters.Models;
using MVC_InventoryMasters.Repositories;
using MVC_InventoryMasters.Services;
using MVC_InventoryMasters.Tests.Infrastructure;

namespace MVC_InventoryMasters.Tests.Marilene.Repositories;

// Cenários de Marilene adaptados para dados sintéticos e transporte em memória.
[Trait("Integrante", "Marilene")]

public class ParceirosRepositoryTests
{
    private readonly FirestoreMemory db = new();
    private ParceirosRepository Repo => new(db.Firebase, NullLogger<ParceirosRepository>.Instance, db.Context);
    private static readonly DateTime Dia = new(2026, 1, 15, 12, 0, 0, DateTimeKind.Utc);
    private Task Seed() => db.Seed("Parceiros", "p1", new Parceiro { Nome = "Coleta Azul", Email = "coleta@example.com", EmpresaId = "empresa-a", Ativo = true, Data_Cadastro = Dia });
    [Fact] public async Task Adicionar_PersisteDadosEContexto()
    { await Repo.Adicionar(new Parceiro { Nome = "Coleta Azul", Ativo = true }); var p = Assert.Single(await Repo.ListarTodos()); Assert.Equal("Coleta Azul", p.Nome); Assert.Equal("empresa-a", p.EmpresaId); }
    [Fact] public async Task BuscarPorId_Existente_RetornaDados()
    { await Seed(); Assert.Equal("Coleta Azul", (await Repo.BuscarPorId("p1"))!.Nome); }
    [Fact] public async Task BuscarPorId_Inexistente_RetornaNull() => Assert.Null(await Repo.BuscarPorId("ausente"));
    [Fact] public async Task ListarTodos_RetornaIds()
    { await Seed(); Assert.Equal("p1", Assert.Single(await Repo.ListarTodos()).Id); }
    [Fact] public async Task ListarPorEmpresa_IsolaDados()
    { await Seed(); await db.Seed("Parceiros", "p2", new Parceiro { EmpresaId = "empresa-b" }); Assert.Equal("p1", Assert.Single(await Repo.ListarPorEmpresa()).Id); }
    [Theory] [InlineData("AZUL", 1)] [InlineData("ausente", 0)] [InlineData("", 1)]
    public async Task Pesquisar_FiltraSemDiferenciarMaiusculas(string termo, int total)
    { await Seed(); Assert.Equal(total, (await Repo.Pesquisar(termo)).Count); }
    [Fact] public async Task FiltrarAvancado_AplicaTextoDatasEStatus()
    {
        await Seed();
        await db.Seed("Parceiros", "inativo", new Parceiro { Nome = "Coleta Azul", EmpresaId = "empresa-a", Ativo = false, Data_Cadastro = Dia });
        await db.Seed("Parceiros", "antigo", new Parceiro { Nome = "Coleta Azul", EmpresaId = "empresa-a", Ativo = true, Data_Cadastro = Dia.AddDays(-2) });
        Assert.Equal("p1", Assert.Single(await Repo.FiltrarAvancado("AZUL", Dia, Dia, true)).Id);
        Assert.Empty(await Repo.FiltrarAvancado("inexistente", Dia, Dia, true));
    }
    [Fact] public async Task Atualizar_PersisteMudancas()
    { await Seed(); await Repo.Atualizar(new Parceiro { Id = "p1", Nome = "Novo", EmpresaId = "empresa-a" }); Assert.Equal("Novo", (await Repo.BuscarPorId("p1"))!.Nome); }
    [Fact] public async Task Excluir_Remove()
    { await Seed(); await Repo.Excluir("p1"); Assert.Null(await Repo.BuscarPorId("p1")); }
    [Fact] public async Task Excluir_Erro_LancaMensagem()
    { db.Failure = new InvalidOperationException(); Assert.Equal("Não foi possível excluir o parceiro.", (await Assert.ThrowsAsync<Exception>(() => Repo.Excluir("p1"))).Message); }
    [Fact] public async Task AtualizarStatus_AlteraSoStatus()
    { await Seed(); await Repo.AtualizarStatus("p1", false); var p = await Repo.BuscarPorId("p1"); Assert.False(p!.Ativo); Assert.Equal("Coleta Azul", p.Nome); }
    [Fact] public async Task AtualizarStatus_Erro_LancaMensagem()
    { db.Failure = new InvalidOperationException(); Assert.Equal("Não foi possível atualizar o status do parceiro.", (await Assert.ThrowsAsync<Exception>(() => Repo.AtualizarStatus("p1", false))).Message); }
}

