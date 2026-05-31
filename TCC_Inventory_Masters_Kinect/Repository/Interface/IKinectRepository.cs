using System.Collections.Generic;
using TCC_Inventory_Masters_Kinect.Model;

namespace TCC_Inventory_Masters_Kinect.Repository.Interface
{
    public interface IKinectRepository
    {
        // ==========================================
        // MEDIÇÃO DE VOLUME
        // ==========================================

        void SalvarMedicao(
            MedicaoVolume medicao);

        List<MedicaoVolume> ObterUltimasMedicoes(
            int quantidade);

        // ==========================================
        // ESPAÇO MAPEADO
        // ==========================================

        void SalvarEspaco(
            EspacoMapeado espaco);

        EspacoMapeado ObterEspaco(
            int id);

        EspacoMapeado ObterEspacoPorNome(
            string nomeEspaco);

        // ==========================================
        // HISTÓRICO DE OCUPAÇÃO
        // ==========================================

        void SalvarHistorico(
            HistoricoOcupacao historico);

        // ==========================================
        // SNAPSHOT ESPACIAL
        // ==========================================

        void SalvarSnapshot(
            SnapshotEspacial snapshot);
    }
}