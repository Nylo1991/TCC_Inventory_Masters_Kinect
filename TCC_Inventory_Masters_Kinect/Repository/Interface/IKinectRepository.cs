using System.Collections.Generic;
using TCC_Inventory_Masters_Kinect.Model;

namespace TCC_Inventory_Masters_Kinect.Repository.Interface
{
    /// <summary>
    /// responsavel por definir os métodos de acesso a dados relacionados às 
    /// medições volumétricas e históricos de ocupação do Kinect.
    /// </summary>
    public interface IKinectRepository
    {
        /// <summary>
        /// Salva uma nova medição volumétrica no banco de dados, incluindo informações como volume em cm³ e m³,
        /// data e hora da medição, status do Kinect e calibração.
        /// </summary>
        /// <param name="medicao"></param>
        void SalvarMedicao(MedicaoVolume medicao);
          List<MedicaoVolume> ObterUltimasMedicoes(int quantidade);
          List<MedicaoVolume> ObterMedicoesEmOrdemCrescente(int quantidade);

        /// <summary>
        /// salva um novo registro de histórico de ocupação no banco de dados, 
        /// contendo informações como o volume atual,
        /// </summary>
        /// <param name="historico"></param>
        void SalvarHistorico(HistoricoOcupacao historico);
          List<HistoricoOcupacao> ObterHistoricoPorEspaco(int espacoId);
          List<HistoricoOcupacao> ObterUltimosHistoricos(int quantidade);
    }
}