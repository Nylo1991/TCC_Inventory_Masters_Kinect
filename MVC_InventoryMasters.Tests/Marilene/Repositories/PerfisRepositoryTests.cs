using Microsoft.Extensions.Logging.Abstractions;
using MVC_InventoryMasters.Models;
using MVC_InventoryMasters.Repositories;
using MVC_InventoryMasters.Services;
using MVC_InventoryMasters.Tests.Infrastructure;

namespace MVC_InventoryMasters.Tests.Marilene.Repositories;

// Cenários de Marilene adaptados para dados sintéticos e transporte em memória.
[Trait("Integrante", "Marilene")]

public class PerfisRepositoryTests
{
    private readonly FirestoreMemory db = new();
    private PerfisRepository Repo => new(db.Firebase, NullLogger<PerfisRepository>.Instance, db.Context, new PermissaoService());
    private Task Seed() => db.Seed("PerfilUsuario", "p1", new Perfil { Nome = "Operador", EmpresaId = "empresa-a" });
    [Fact] public async Task ListarTodos_NormalizaPermissoes()
    { await Seed(); var p = Assert.Single(await Repo.ListarTodos()); Assert.Equal("p1", p.Id); Assert.Contains(PermissoesSistema.KinectAcessar, p.Permissoes); }
    [Fact] public async Task ListarTodos_Erro_RetornaVazio()
    { db.Failure = new InvalidOperationException(); Assert.Empty(await Repo.ListarTodos()); }
    [Theory] [InlineData("empresa-a")] [InlineData(null)] [InlineData("")]
    public async Task ListarPorEmpresa_UsaFiltroOuContexto(string? empresa)
    { await Seed(); await db.Seed("PerfilUsuario", "p2", new Perfil { EmpresaId = "empresa-b" }); Assert.Equal("p1", Assert.Single(await Repo.ListarPorEmpresa(empresa)).Id); }
    [Fact] public async Task BuscarPorId_Existente_NormalizaPermissoes()
    { await Seed(); var p = await Repo.BuscarPorId("p1"); Assert.Equal("p1", p!.Id); Assert.Contains(PermissoesSistema.KinectAcessar, p.Permissoes); }
    [Fact] public async Task BuscarPorId_Inexistente_RetornaNull() => Assert.Null(await Repo.BuscarPorId("ausente"));
    [Fact] public async Task BuscarPorId_Erro_RetornaNull()
    { db.Failure = new InvalidOperationException(); Assert.Null(await Repo.BuscarPorId("p1")); }
    [Fact] public async Task Adicionar_UsaContextoEPermissoesPadrao()
    { await Repo.Adicionar(new Perfil { Nome = "Operador" }); var p = Assert.Single(await Repo.ListarTodos()); Assert.Equal("empresa-a", p.EmpresaId); Assert.Contains(PermissoesSistema.KinectAcessar, p.Permissoes); }
    [Fact] public async Task Atualizar_PreservaPermissoesInformadas()
    { await Seed(); await Repo.Atualizar(new Perfil { Id = "p1", Nome = "Personalizado", Permissoes = new() { "medicoes.visualizar" } }); var p = await Repo.BuscarPorId("p1"); Assert.Equal("Personalizado", p!.Nome); Assert.Equal(new[] { "medicoes.visualizar" }, p.Permissoes); }
    [Fact] public async Task Inativar_PreservaNome()
    { await Seed(); await Repo.Inativar("p1"); var p = await Repo.BuscarPorId("p1"); Assert.False(p!.Ativo); Assert.Equal("Operador", p.Nome); }
}

