using MVC_InventoryMasters.Models;
using MVC_InventoryMasters.ViewModels;

namespace MVC_InventoryMasters.Diulie.Tests.ViewModels;

[Xunit.Trait("Integrante", "Diulie")]
public class DashboardViewModelTests
{
    [Fact]
    public void Construtor_DeveInicializarColecoesEParametros()
    {
        var model = new DashboardViewModel();

        Assert.NotNull(model.Medicoes);
        Assert.NotNull(model.Alertas);
        Assert.NotNull(model.Parceiros);
        Assert.NotNull(model.Usuarios);
        Assert.NotNull(model.Parametros);
        Assert.Empty(model.MensagemErro);
    }

    [Fact]
    public void Totais_DeveRefletirQuantidadeDasColecoes()
    {
        var model = new DashboardViewModel
        {
            Medicoes = new() { new(), new() },
            Alertas = new() { new(), new(), new() },
            Parceiros = new() { new() },
            Usuarios = new() { new(), new(), new(), new() }
        };

        Assert.Equal(2, model.TotalMedicoes);
        Assert.Equal(3, model.TotalAlertas);
        Assert.Equal(1, model.TotalParceiros);
        Assert.Equal(4, model.TotalUsuarios);
    }

    [Fact]
    public void Totais_ColecoesNulas_DeveRetornarZero()
    {
        var model = new DashboardViewModel
        {
            Medicoes = null!,
            Alertas = null!,
            Parceiros = null!,
            Usuarios = null!
        };

        Assert.Equal(0, model.TotalMedicoes);
        Assert.Equal(0, model.TotalAlertas);
        Assert.Equal(0, model.TotalParceiros);
        Assert.Equal(0, model.TotalUsuarios);
        Assert.Empty(model.UltimasNotificacoes);
    }

    [Fact]
    public void UltimasNotificacoes_DeveRetornarCincoMaisRecentes()
    {
        var inicio = new DateTime(2026, 8, 1, 12, 0, 0, DateTimeKind.Utc);
        var alertas = Enumerable.Range(0, 7)
            .Select(i => new Notificacao
            {
                Id = $"N{i}",
                DataHora = inicio.AddMinutes(i)
            })
            .Reverse()
            .ToList();
        var model = new DashboardViewModel { Alertas = alertas };

        var resultado = model.UltimasNotificacoes;

        Assert.Equal(5, resultado.Count);
        Assert.Equal(new[] { "N6", "N5", "N4", "N3", "N2" },
            resultado.Select(n => n.Id));
    }

    [Fact]
    public void BaseViewModel_DeveArmazenarIdentificacaoDaPagina()
    {
        var model = new DashboardViewModel
        {
            NomeUsuario = "Maria",
            TituloPagina = "Dashboard"
        };

        Assert.Equal("Maria", model.NomeUsuario);
        Assert.Equal("Dashboard", model.TituloPagina);
    }

    [Fact]
    public void Indicadores_DeveArmazenarPercentualEParametros()
    {
        var parametros = new ParametrosSistema { CapacidadeMaxima = 25 };
        var model = new DashboardViewModel
        {
            PercentualOcupacao = 72.5m,
            Parametros = parametros,
            MensagemErro = "Aviso controlado"
        };

        Assert.Equal(72.5m, model.PercentualOcupacao);
        Assert.Same(parametros, model.Parametros);
        Assert.Equal("Aviso controlado", model.MensagemErro);
    }
}
