using Microsoft.Extensions.Logging.Abstractions;
using MVC_InventoryMasters.Models;
using MVC_InventoryMasters.Repositories;
using MVC_InventoryMasters.Services;
using MVC_InventoryMasters.Tests.Infrastructure;

namespace MVC_InventoryMasters.Tests.Marilene.Repositories;

// Cenários de Marilene adaptados para dados sintéticos e transporte em memória.
[Trait("Integrante", "Marilene")]

public class UsuariosRepositoryTests
{
    private readonly FirestoreMemory db = new();
    private UsuariosRepository Repo => new(db.Firebase, NullLogger<UsuariosRepository>.Instance, db.Context);
    private Task Seed() => db.Seed("Usuarios", "u1", new Usuario { Nome = "Ana", Email = "ana@example.com", EmpresaId = "empresa-a", Ativo = true, Data_Cadastro = DateTime.UtcNow });
    [Fact] public async Task ListarTodos_Vazio_RetornaVazio() => Assert.Empty(await Repo.ListarTodos());
    [Fact] public async Task ListarTodos_ComUsuarios_RetornaCamposEIds()
    { await Seed(); var u = Assert.Single(await Repo.ListarTodos()); Assert.Equal("u1", u.Id); Assert.Equal("Ana", u.Nome); }
    [Theory] [InlineData("empresa-a")] [InlineData(null)] [InlineData("")]
    public async Task ListarPorEmpresa_UsaFiltroOuContexto(string? empresa)
    {
        await Seed(); await db.Seed("Usuarios", "u2", new Usuario { EmpresaId = "empresa-b", Data_Cadastro = DateTime.UtcNow });
        Assert.Equal("u1", Assert.Single(await Repo.ListarPorEmpresa(empresa)).Id);
    }
    [Fact] public async Task BuscarPorId_Existente_RetornaDados()
    { await Seed(); Assert.Equal("Ana", (await Repo.BuscarPorId("u1"))!.Nome); }
    [Fact] public async Task BuscarPorId_Inexistente_RetornaNull() => Assert.Null(await Repo.BuscarPorId("ausente"));
    [Theory] [InlineData("ana@example.com")] [InlineData(" ANA@EXAMPLE.COM ")]
    public async Task BuscarPorEmail_NormalizaEmail(string email)
    { await Seed(); Assert.Equal("u1", (await Repo.BuscarPorEmail(email))!.Id); }
    [Fact] public async Task BuscarPorEmail_LegadoMaiusculo_UsaFallback()
    { await db.Seed("Usuarios", "legado", new Usuario { Email = "ANA@EXAMPLE.COM", Data_Cadastro = DateTime.UtcNow }); Assert.Equal("legado", (await Repo.BuscarPorEmail("ana@example.com"))!.Id); }
    [Fact] public async Task BuscarPorEmail_Inexistente_RetornaNull()
    { await Seed(); Assert.Null(await Repo.BuscarPorEmail("ausente@example.com")); }
    [Fact] public async Task Adicionar_PersisteEAtribuiEmpresa()
    {
        await Repo.Adicionar(new Usuario { Nome = "Ana", Email = "ana@example.com", Ativo = true });
        var u = Assert.Single(await Repo.ListarTodos());
        Assert.Equal("Ana", u.Nome); Assert.Equal("empresa-a", u.EmpresaId); Assert.True(u.Ativo);
        Assert.False(string.IsNullOrEmpty(u.Id));
    }
    [Fact] public async Task Adicionar_Erro_LancaExcecao()
    { db.Failure = new InvalidOperationException(); var ex = await Assert.ThrowsAsync<Exception>(() => Repo.Adicionar(new Usuario())); Assert.Equal("Não foi possível cadastrar o usuário.", ex.Message); }
    [Fact] public async Task Atualizar_PersisteMudancas()
    { await Seed(); await Repo.Atualizar(new Usuario { Id = "u1", Nome = "Nome novo", EmpresaId = "empresa-a" }); Assert.Equal("Nome novo", (await Repo.BuscarPorId("u1"))!.Nome); }
    [Fact] public async Task Atualizar_Erro_LancaExcecao()
    { db.Failure = new InvalidOperationException(); var ex = await Assert.ThrowsAsync<Exception>(() => Repo.Atualizar(new Usuario { Id = "u1" })); Assert.Equal("Não foi possível atualizar o usuário.", ex.Message); }
    [Fact] public async Task AtualizarStatus_PreservaDemaisCampos()
    { await Seed(); await Repo.AtualizarStatus("u1", false); var u = await Repo.BuscarPorId("u1"); Assert.False(u!.Ativo); Assert.Equal("Ana", u.Nome); }
    [Fact] public async Task Excluir_RemoveDocumento()
    { await Seed(); await Repo.Excluir("u1"); Assert.Null(await Repo.BuscarPorId("u1")); }
    [Fact] public async Task Excluir_Inexistente_NaoLanca()
    { Assert.Null(await Record.ExceptionAsync(() => Repo.Excluir("ausente"))); Assert.Empty(await Repo.ListarTodos()); }
}
