using Microsoft.Extensions.Logging.Abstractions;
using MVC_InventoryMasters.Models;
using MVC_InventoryMasters.Repositories;
using MVC_InventoryMasters.Services;
using MVC_InventoryMasters.Tests.Infrastructure;

namespace MVC_InventoryMasters.Tests.Marilene.Repositories;

// Cenários de Marilene adaptados para dados sintéticos e transporte em memória.
[Trait("Integrante", "Marilene")]

public class LogsSistemaRepositoryTests
{
    [Theory] [InlineData("empresa-b", "empresa-b")] [InlineData(null, "empresa-a")] [InlineData(" ", "empresa-a")]
    public async Task Registrar_EmpresaInformadaOuContexto_PersisteCampos(string? empresa, string esperado)
    {
        var db = new FirestoreMemory();
        var repo = new LogsSistemaRepository(db.Firebase, NullLogger<LogsSistemaRepository>.Instance, db.Context);
        var antes = DateTime.UtcNow.AddSeconds(-1);
        await repo.Registrar("Login", "Teste", "Aviso", "teste@example.com", "u1", empresa);
        var doc = Assert.Single((await db.Db.Collection("LogsSistema").GetSnapshotAsync()).Documents).ConvertTo<LogSistema>();
        Assert.Equal(esperado, doc.EmpresaId); Assert.Equal("Login", doc.Acao);
        Assert.Equal("Teste", doc.Mensagem); Assert.Equal("Aviso", doc.Nivel);
        Assert.Equal("teste@example.com", doc.Email); Assert.Equal("u1", doc.UsuarioId);
        Assert.InRange(doc.DataHora, antes, DateTime.UtcNow);
    }
    [Fact] public async Task Registrar_Erro_CapturaERegistraSemLancar()
    {
        var db = new FirestoreMemory { Failure = new InvalidOperationException("falha simulada") };
        var logger = new Moq.Mock<Microsoft.Extensions.Logging.ILogger<LogsSistemaRepository>>();
        var repo = new LogsSistemaRepository(db.Firebase, logger.Object, db.Context);
        Assert.Null(await Record.ExceptionAsync(() => repo.Registrar("Teste", "Mensagem")));
        Assert.True(db.Calls > 0);
        Assert.Contains(logger.Invocations, i => i.Method.Name == "Log" &&
            Equals(i.Arguments[0], Microsoft.Extensions.Logging.LogLevel.Error));
    }
}

