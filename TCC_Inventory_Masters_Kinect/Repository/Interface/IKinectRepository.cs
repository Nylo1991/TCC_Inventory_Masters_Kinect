using TCC_Inventory_Masters_Kinect.Model;

namespace TCC_Inventory_Masters_Kinect.Repository.Interface
{
    public interface IKinectRepository
    {
        void SalvarMedicao(
            MedicaoVolume medicao);

        void SalvarEspaco(
            EspacoMapeado espaco);

        void SalvarHistorico(
            HistoricoOcupacao historico);

        void SalvarSnapshot(
            SnapshotEspacial snapshot);

        EspacoMapeado ObterEspaco(
            int id);

        EspacoMapeado ObterEspacoPorNome(
            string nomeEspaco);
    }
}