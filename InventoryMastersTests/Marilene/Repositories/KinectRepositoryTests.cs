using System;
using System.Collections.Generic;
using System.Linq;
using TCC_Inventory_Masters_Kinect.Data;
using TCC_Inventory_Masters_Kinect.Model;
using TCC_Inventory_Masters_Kinect.Repository;
using TCC_Inventory_Masters_Kinect.Tests.Infrastructure;
using Xunit;

namespace TCC_Inventory_Masters_Kinect.Tests.Marilene
{
    // Cenários originais de Marilene, agora sem criar ou alterar bancos SQLite reais.
    [Trait("Integrante", "Marilene")]
    public class KinectRepositoryTests
    {
        private readonly ContextoFake contexto = new ContextoFake();
        private KinectRepository Repo(string empresa = "empresa-a") => new KinectRepository(empresa, () => contexto);
        private KinectRepository RepoComErro() => new KinectRepository("empresa-a", () => throw new InvalidOperationException("falha simulada"));

        [Fact] public void SalvarMedicao_PersisteEConfirma()
        {
            var m = new MedicaoVolume { VolumeCm3 = 250, Empresa = "original" };
            Repo(null).SalvarMedicao(m);
            Assert.Same(m, Assert.Single(contexto.Medicoes));
            Assert.Equal("original", m.Empresa); Assert.Equal(1, contexto.Salvamentos); Assert.True(contexto.Descartado);
        }
        [Fact] public void SalvarMedicao_UsaEmpresaConfigurada()
        { var m = new MedicaoVolume { Empresa = "outra" }; Repo().SalvarMedicao(m); Assert.Equal("empresa-a", Assert.Single(contexto.Medicoes).Empresa); }
        [Fact] public void SalvarMedicao_Erro_CapturaERegistraLog()
        {
            int antes = TestEnvironment.Logs.Count;
            Assert.Null(Record.Exception(() => RepoComErro().SalvarMedicao(new MedicaoVolume())));
            Assert.Contains(TestEnvironment.Logs.Skip(antes), l => l.Nivel == "ERRO" && l.Mensagem.Contains("salvar medicao"));
        }
        private void Medicoes()
        { contexto.Medicoes.AddRange(new[] { new MedicaoVolume { Id = 1 }, new MedicaoVolume { Id = 3 }, new MedicaoVolume { Id = 2 } }); }
        [Fact] public void ObterUltimasMedicoes_LimitaEOrdena()
        { Medicoes(); Assert.Equal(new[] { 3, 2 }, Repo().ObterUltimasMedicoes(2).Select(m => m.Id)); Assert.True(contexto.Descartado); }
        [Fact] public void ObterUltimasMedicoes_Erro_RetornaVazio() => Assert.Empty(RepoComErro().ObterUltimasMedicoes(2));
        [Fact] public void ObterMedicoesEmOrdemCrescente_OrdenaAsUltimas()
        { Medicoes(); Assert.Equal(new[] { 2, 3 }, Repo().ObterMedicoesEmOrdemCrescente(2, "usuario", "empresa-a").Select(m => m.Id)); }
        [Fact] public void ObterMedicoesEmOrdemCrescente_Erro_RetornaVazio()
        { Assert.Empty(RepoComErro().ObterMedicoesEmOrdemCrescente(2, "usuario", "empresa-a")); }
        [Fact] public void SalvarHistorico_PersisteEConfirma()
        {
            var h = new HistoricoOcupacao { Empresa = "original", VolumeAtualCm3 = 100 };
            Repo(null).SalvarHistorico(h);
            Assert.Same(h, Assert.Single(contexto.Historicos)); Assert.Equal("original", h.Empresa);
            Assert.Equal(1, contexto.Salvamentos); Assert.True(contexto.Descartado);
        }
        [Fact] public void SalvarHistorico_UsaEmpresaConfigurada()
        { Repo().SalvarHistorico(new HistoricoOcupacao { Empresa = "outra" }); Assert.Equal("empresa-a", Assert.Single(contexto.Historicos).Empresa); }
        [Fact] public void SalvarHistorico_Erro_CapturaERegistraLog()
        {
            int antes = TestEnvironment.Logs.Count;
            Assert.Null(Record.Exception(() => RepoComErro().SalvarHistorico(new HistoricoOcupacao())));
            Assert.Contains(TestEnvironment.Logs.Skip(antes), l => l.Nivel == "ERRO" && l.Mensagem.Contains("salvar historico"));
        }
        private void Historicos()
        {
            contexto.Historicos.AddRange(new[] {
                new HistoricoOcupacao { Id = 1, MedicaoVolumeId = 10, Empresa = "empresa-a" },
                new HistoricoOcupacao { Id = 2, MedicaoVolumeId = 10, Empresa = "empresa-b" },
                new HistoricoOcupacao { Id = 3, MedicaoVolumeId = 10, Empresa = "empresa-a" },
                new HistoricoOcupacao { Id = 4, MedicaoVolumeId = 20, Empresa = "empresa-a" }
            });
        }
        [Fact] public void ObterHistoricoPorEspaco_FiltraEspacoEmpresaEOrdena()
        { Historicos(); Assert.Equal(new[] { 3, 1 }, Repo().ObterHistoricoPorEspaco(10).Select(h => h.Id)); }
        [Fact] public void ObterHistoricoPorEspaco_Erro_RetornaVazio() => Assert.Empty(RepoComErro().ObterHistoricoPorEspaco(10));
        [Fact] public void ObterUltimosHistoricos_FiltraEmpresaLimitaEOrdena()
        { Historicos(); Assert.Equal(new[] { 4, 3 }, Repo().ObterUltimosHistoricos(2).Select(h => h.Id)); }
        [Fact] public void ObterUltimosHistoricos_Erro_RetornaVazio() => Assert.Empty(RepoComErro().ObterUltimosHistoricos(2));

        private sealed class ContextoFake : IKinectDataContext
        {
            internal List<MedicaoVolume> Medicoes { get; } = new List<MedicaoVolume>();
            internal List<HistoricoOcupacao> Historicos { get; } = new List<HistoricoOcupacao>();
            internal int Salvamentos { get; private set; }
            internal bool Descartado { get; private set; }
            public IQueryable<MedicaoVolume> MedicaoVolumes => Medicoes.AsQueryable();
            public IQueryable<HistoricoOcupacao> HistoricosOcupacao => Historicos.AsQueryable();
            public void AdicionarMedicao(MedicaoVolume m) => Medicoes.Add(m);
            public void AdicionarHistorico(HistoricoOcupacao h) => Historicos.Add(h);
            public int SaveChanges() { Salvamentos++; return 1; }
            public void Dispose() { Descartado = true; }
        }
    }
}

