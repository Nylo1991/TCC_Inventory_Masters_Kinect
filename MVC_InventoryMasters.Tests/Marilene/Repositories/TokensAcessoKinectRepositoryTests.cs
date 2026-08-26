using Microsoft.Extensions.Logging.Abstractions;
using MVC_InventoryMasters.Models;
using MVC_InventoryMasters.Repositories;
using MVC_InventoryMasters.Services;
using MVC_InventoryMasters.Tests.Infrastructure;

namespace MVC_InventoryMasters.Tests.Marilene.Repositories;

// Cenários de Marilene adaptados para dados sintéticos e transporte em memória.
[Trait("Integrante", "Marilene")]

public class TokensAcessoKinectRepositoryTests
{
    private readonly FirestoreMemory db = new();
    private TokensAcessoKinectRepository Repo => new(db.Firebase, NullLogger<TokensAcessoKinectRepository>.Instance);
    private static TokenAcessoKinect Token(string hash = "hash-teste") =>
        new() { TokenHash = hash, UsuarioId = "u1", ExpiraEm = DateTime.UtcNow.AddMinutes(10) };
    [Fact] public async Task Adicionar_PersisteCampos()
    { await Repo.Adicionar(Token()); var t = await Repo.BuscarAtivoPorHash("hash-teste"); Assert.NotNull(t); Assert.Equal("u1", t.UsuarioId); Assert.False(string.IsNullOrEmpty(t.Id)); }
    [Fact] public async Task BuscarAtivoPorHash_RetornaSomenteHashSolicitado()
    { await db.Seed("TokensAcessoKinect", "t1", Token()); await db.Seed("TokensAcessoKinect", "t2", Token("outro")); Assert.Equal("t1", (await Repo.BuscarAtivoPorHash("hash-teste"))!.Id); }
    [Theory] [InlineData(true, false)] [InlineData(false, true)]
    public async Task BuscarAtivoPorHash_RejeitaUsadoOuRevogado(bool usado, bool revogado)
    { var t = Token(); t.Utilizado = usado; t.Revogado = revogado; await db.Seed("TokensAcessoKinect", "t1", t); Assert.Null(await Repo.BuscarAtivoPorHash("hash-teste")); }
    [Fact] public async Task BuscarAtivoPorHash_Inexistente_RetornaNull()
    { await db.Seed("TokensAcessoKinect", "t1", Token()); Assert.Null(await Repo.BuscarAtivoPorHash("ausente")); }
    [Fact] public async Task BuscarAtivoPorHash_Erro_RetornaNull()
    { db.Failure = new InvalidOperationException(); Assert.Null(await Repo.BuscarAtivoPorHash("hash-teste")); }
    [Fact] public async Task MarcarComoUtilizado_PersisteDataEInvalidaBusca()
    {
        await db.Seed("TokensAcessoKinect", "t1", Token());
        var antes = DateTime.UtcNow.AddSeconds(-1);
        await Repo.MarcarComoUtilizado(new TokenAcessoKinect { Id = "t1" });
        var t = await db.Read<TokenAcessoKinect>("TokensAcessoKinect", "t1");
        Assert.True(t.Utilizado); Assert.InRange(t.ValidadoEm!.Value, antes, DateTime.UtcNow);
        Assert.Null(await Repo.BuscarAtivoPorHash("hash-teste"));
    }
    [Theory] [InlineData(null)] [InlineData("")] [InlineData(" ")]
    public async Task MarcarComoUtilizado_SemId_NaoAcessaBanco(string? id)
    { await Repo.MarcarComoUtilizado(new TokenAcessoKinect { Id = id }); Assert.Equal(0, db.Calls); }
}

