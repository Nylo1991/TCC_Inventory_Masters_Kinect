using System.Collections.Generic;
using TCC_Inventory_Masters_Kinect.Model;

namespace TCC_Inventory_Masters_Kinect.Repository.Interface
{
    public interface IKinectRepository
    {
        void SalvarMedicao(MedicaoVolume medicao);
        List<MedicaoVolume> ObterUltimasMedicoes(int quantidade);

        void SalvarEspaco(EspacoMapeado espaco);
        EspacoMapeado ObterEspaco(int id);
        EspacoMapeado ObterEspacoPorNome(string nomeEspaco);

        void SalvarHistorico(HistoricoOcupacao historico);
        List<HistoricoOcupacao> ObterHistoricoPorEspaco(int espacoId);
        List<HistoricoOcupacao> ObterUltimosHistoricos(int quantidade);

        void SalvarSnapshot(SnapshotEspacial snapshot);
        List<SnapshotEspacial> ObterSnapshotsPorEspaco(int espacoId);
        SnapshotEspacial ObterUltimoSnapshot(int espacoId);
    }
}
