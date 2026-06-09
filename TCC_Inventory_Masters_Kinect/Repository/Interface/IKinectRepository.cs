using System.Collections.Generic;
using TCC_Inventory_Masters_Kinect.Model;

namespace TCC_Inventory_Masters_Kinect.Repository.Interface
{
    public interface IKinectRepository
    {
        // ==================== MEDIÇÕES ====================
        void SalvarMedicao(MedicaoVolume medicao);
        List<MedicaoVolume> ObterUltimasMedicoes(int quantidade);

        // ==================== ESPAÇOS ====================
        void SalvarEspaco(Space space);
        void AtualizarEspaco(Space space);
        List<Space> ObterTodosEspacos();
        Space ObterEspaco(int id);
        Space ObterEspacoPorNome(string nomeEspaco);

        // ==================== HISTÓRICO DE OCUPAÇÃO ====================
        void SalvarHistorico(HistoricoOcupacao historico);
        List<HistoricoOcupacao> ObterHistoricoPorEspaco(int espacoId);
        List<HistoricoOcupacao> ObterUltimosHistoricos(int quantidade);
    }
}
