using Microsoft.Extensions.Logging.Abstractions;
using MVC_InventoryMasters.Models;
using MVC_InventoryMasters.Repositories;
using MVC_InventoryMasters.Services;
using MVC_InventoryMasters.Tests.Infrastructure;

namespace MVC_InventoryMasters.Tests.Marilene.Repositories;

// Cenários de Marilene adaptados para dados sintéticos e transporte em memória.
[Trait("Integrante", "Marilene")]

public class ParametrosSistemaRepositoryTests
{
    private readonly FirestoreMemory db = new();
    private ParametrosSistemaRepository Repo => new(db.Firebase, NullLogger<ParametrosSistemaRepository>.Instance, db.Context);
    [Fact] public async Task BuscarPorEmpresa_Existente_RetornaValores()
    {
        await db.Seed("parametrosSistema", "configuracao_empresa-a", new ParametrosSistema { EmpresaId = "empresa-a", CapacidadeMaxima = 150, PercentualAlerta = 85, DataAtualizacao = DateTime.UtcNow });
        var p = Repo.BuscarPorEmpresa("empresa-a"); Assert.Equal(150d, p.CapacidadeMaxima); Assert.Equal(85, p.PercentualAlerta);
    }
    [Fact] public async Task BuscarPorEmpresa_SemConfig_UsaGlobalSemTrocarEmpresa()
    {
        await db.Seed("parametrosSistema", "configuracao", new ParametrosSistema { EmpresaId = "global", CapacidadeMaxima = 200, DataAtualizacao = DateTime.UtcNow });
        var p = Repo.BuscarPorEmpresa("empresa-a"); Assert.Equal(200d, p.CapacidadeMaxima); Assert.Equal("empresa-a", p.EmpresaId);
    }
    [Fact] public void BuscarPorEmpresa_Erro_RetornaPadrao()
    { db.Failure = new InvalidOperationException(); Assert.Equal(new ParametrosSistema().PercentualAlerta, Repo.BuscarPorEmpresa("empresa-a").PercentualAlerta); Assert.True(db.Calls > 0); }
    [Fact] public void ObterPadroes_ValoresPredefinidos()
    { var p = Repo.ObterPadroes(); Assert.Equal(300d, p.CapacidadeMaxima); Assert.Equal(10, p.PercentualAlerta); Assert.True(p.NotificacaoAutomatica); Assert.Equal(10, p.DiasSemColetaAlerta); }
    [Fact] public void Salvar_UsaContextoEPersiste()
    {
        var p = Repo.ObterPadroes(); p.CapacidadeMaxima = 450;
        Repo.Salvar(p);
        var salvo = Repo.Buscar(); Assert.Equal(450d, salvo.CapacidadeMaxima); Assert.Equal("empresa-a", salvo.EmpresaId);
    }
    [Fact] public void Salvar_Erro_LancaMensagem()
    { db.Failure = new InvalidOperationException(); Assert.Equal("Não foi possível salvar os parâmetros do sistema.", Assert.Throws<Exception>(() => Repo.Salvar(Repo.ObterPadroes())).Message); }
    [Theory] [InlineData(50, 200, 25)] [InlineData(200, 100, 200)] [InlineData(0, 100, 0)] [InlineData(10, 0, 0)] [InlineData(10, -5, 0)]
    public void CalcularPercentual_ValoresELimites(double volume, double capacidade, double esperado)
    { Assert.Equal(esperado, Repo.CalcularPercentualOcupacao(volume, capacidade)); Assert.Equal(0, db.Calls); }
    [Fact] public void Salvar_Raio_PreservaValor()
    { var p = Repo.ObterPadroes(); p.RaioDeteccaoKinect = 3.5; Repo.Salvar(p); Assert.Equal(3.5, Repo.Buscar().RaioDeteccaoKinect); }
    // Valida o intervalo declarado no modelo, não um suposto limite físico do sensor.
    [Theory] [InlineData(-0.1, false)] [InlineData(0, true)] [InlineData(3.5, true)] [InlineData(100, true)] [InlineData(100.1, false)]
    public void ValidarRaio_RespeitaDataAnnotation(double raio, bool esperado)
    {
        var p = Repo.ObterPadroes(); p.RaioDeteccaoKinect = raio;
        var context = new System.ComponentModel.DataAnnotations.ValidationContext(p) { MemberName = nameof(p.RaioDeteccaoKinect) };
        Assert.Equal(esperado, System.ComponentModel.DataAnnotations.Validator.TryValidateProperty(raio, context, new List<System.ComponentModel.DataAnnotations.ValidationResult>()));
    }
    [Fact] public async Task Buscar_UsaEmpresaDoContexto()
    {
        await db.Seed("parametrosSistema", "configuracao_empresa-a", new ParametrosSistema { CapacidadeMaxima = 150, DataAtualizacao = DateTime.UtcNow });
        await db.Seed("parametrosSistema", "configuracao_empresa-b", new ParametrosSistema { CapacidadeMaxima = 999, DataAtualizacao = DateTime.UtcNow });
        Assert.Equal(150d, Repo.Buscar().CapacidadeMaxima);
    }
}

