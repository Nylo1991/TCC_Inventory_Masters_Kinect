using System;
using System.Linq;
using TCC_Inventory_Masters_Kinect.Model;

namespace TCC_Inventory_Masters_Kinect.Data
{
    internal interface IKinectDataContext : IDisposable
    {
        IQueryable<MedicaoVolume> MedicaoVolumes { get; }
        IQueryable<HistoricoOcupacao> HistoricosOcupacao { get; }
        void AdicionarMedicao(MedicaoVolume medicao);
        void AdicionarHistorico(HistoricoOcupacao historico);
        int SaveChanges();
    }

    // Produção mantém o mesmo contexto EF e o mesmo banco por empresa.
    internal sealed class KinectDataContext : IKinectDataContext
    {
        private readonly AppDbContext _context;
        internal KinectDataContext(string empresa) { _context = new AppDbContext(empresa); }
        public IQueryable<MedicaoVolume> MedicaoVolumes => _context.MedicaoVolumes;
        public IQueryable<HistoricoOcupacao> HistoricosOcupacao => _context.HistoricosOcupacao;
        public void AdicionarMedicao(MedicaoVolume medicao) => _context.MedicaoVolumes.Add(medicao);
        public void AdicionarHistorico(HistoricoOcupacao historico) => _context.HistoricosOcupacao.Add(historico);
        public int SaveChanges() => _context.SaveChanges();
        public void Dispose() => _context.Dispose();
    }
}
