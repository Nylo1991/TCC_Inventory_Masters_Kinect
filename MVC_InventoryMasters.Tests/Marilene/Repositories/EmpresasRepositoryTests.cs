using Microsoft.Extensions.Logging.Abstractions;
using MVC_InventoryMasters.Models;
using MVC_InventoryMasters.Repositories;
using MVC_InventoryMasters.Services;
using MVC_InventoryMasters.Tests.Infrastructure;

namespace MVC_InventoryMasters.Tests.Marilene.Repositories;

// Cenários de Marilene adaptados para dados sintéticos e transporte em memória.
[Trait("Integrante", "Marilene")]

public class EmpresasRepositoryTests
{
    private readonly FirestoreMemory db = new();
    private EmpresasRepository Repo => new(db.Firebase, NullLogger<EmpresasRepository>.Instance);
    [Fact] public async Task ListarTodas_ComEmpresas_RetornaDadosEIds()
    {
        await db.Seed("Empresas", "e1", new Empresa { Nome = "Empresa teste" });
        var item = Assert.Single(await Repo.ListarTodas());
        Assert.Equal("e1", item.Id); Assert.Equal("Empresa teste", item.Nome);
    }
    [Fact] public async Task ListarTodas_Vazio_RetornaVazio() => Assert.Empty(await Repo.ListarTodas());
    [Fact] public async Task ListarTodas_Erro_RetornaVazio()
    { db.Failure = new InvalidOperationException("falha simulada"); Assert.Empty(await Repo.ListarTodas()); }
    [Fact] public async Task BuscarPorId_Existente_RetornaEmpresa()
    { await db.Seed("Empresas", "e1", new Empresa { Nome = "Teste" }); Assert.Equal("e1", (await Repo.BuscarPorId("e1"))!.Id); }
    [Fact] public async Task BuscarPorId_Inexistente_RetornaNull() => Assert.Null(await Repo.BuscarPorId("ausente"));
    [Fact] public async Task BuscarPorId_Erro_RetornaNull()
    { db.Failure = new InvalidOperationException("falha simulada"); Assert.Null(await Repo.BuscarPorId("e1")); }
}

