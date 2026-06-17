using System.Collections.Generic;
using System.Linq;
using TCC_Inventory_Masters_Kinect.Data;
using TCC_Inventory_Masters_Kinect.Logs;
using TCC_Inventory_Masters_Kinect.Model;
using TCC_Inventory_Masters_Kinect.Repository.Interface;

namespace TCC_Inventory_Masters_Kinect.Repository
{
    /// <summary>
    /// Repositório responsável por persistir e consultar medições volumétricas,
    /// históricos de ocupação e dados relacionados ao monitoramento do Kinect.
    /// </summary>
    public class KinectRepository : IKinectRepository
    {
        private readonly string _empresa;

        public KinectRepository()
            : this(null)
        {
        }

        public KinectRepository(string empresa)
        {
            _empresa = empresa;
        }

        /// <summary>
        /// Salva uma medição volumétrica realizada pelo Kinect no banco SQLite.
        /// </summary>
        public void SalvarMedicao(MedicaoVolume medicao)
        {
            try
            {
                using (var db = new AppDbContext(_empresa))
                {
                    db.MedicaoVolumes.Add(medicao);
                    db.SaveChanges();
                }

                LoggerService.Info($"Medicao salva no SQLite. Volume: {medicao.VolumeCm3:F0} cm3");
            }
            catch
            {
                LoggerService.Erro("Erro ao salvar medicao no SQLite.");
            }
        }

        /// <summary>
        /// Obtém as últimas medições registradas no SQLite, ordenadas da mais recente para a mais antiga.
        /// </summary>
        public List<MedicaoVolume> ObterUltimasMedicoes(int quantidade)
        {
            try
            {
                using (var db = new AppDbContext(_empresa))
                {
                    return db.MedicaoVolumes
                        .OrderByDescending(x => x.Id)
                        .Take(quantidade)
                        .ToList();
                }
            }
            catch
            {
                LoggerService.Erro("Erro ao buscar historico de medicoes no SQLite.");
                return new List<MedicaoVolume>();
            }
        }

        /// <summary>
        /// Obtém as medições registradas no SQLite em ordem crescente de ID.
        /// </summary>
        public List<MedicaoVolume> ObterMedicoesEmOrdemCrescente(int quantidade, string usuario, string empresa)
        {
            try
            {
                using (var db = new AppDbContext(_empresa))
                {
                    return db.MedicaoVolumes
                        .Where(x => x.Usuario == usuario && x.Empresa == empresa)
                        .OrderByDescending(x => x.Id)
                        .Take(quantidade)
                        .OrderBy(x => x.Id)
                        .ToList();
                }
            }
            catch
            {
                LoggerService.Erro("Erro ao buscar medicoes em ordem crescente por usuario.");
                return new List<MedicaoVolume>();
            }
        }

        /// <summary>
        /// Salva um registro de histórico de ocupação do espaço monitorado.
        /// </summary>
        public void SalvarHistorico(HistoricoOcupacao historico)
        {
            try
            {
                using (var db = new AppDbContext(_empresa))
                {
                    db.HistoricosOcupacao.Add(historico);
                    db.SaveChanges();
                }

                LoggerService.Info("Historico de ocupacao salvo no SQLite.");
            }
            catch
            {
                LoggerService.Erro("Erro ao salvar historico de ocupacao no SQLite.");
            }
        }

        /// <summary>
        /// Obtém o histórico de ocupação de um espaço específico.
        /// </summary>
        public List<HistoricoOcupacao> ObterHistoricoPorEspaco(int espacoId)
        {
            try
            {
                using (var db = new AppDbContext(_empresa))
                {
                    return db.HistoricosOcupacao
                        .Where(x => x.EspacoMapeadoId == espacoId)
                        .OrderByDescending(x => x.Id)
                        .ToList();
                }
            }
            catch
            {
                LoggerService.Erro("Erro ao buscar historico por espaco.");
                return new List<HistoricoOcupacao>();
            }
        }

        /// <summary>
        /// Obtém os últimos registros de histórico de ocupação.
        /// </summary>
        public List<HistoricoOcupacao> ObterUltimosHistoricos(int quantidade)
        {
            try
            {
                using (var db = new AppDbContext(_empresa))
                {
                    return db.HistoricosOcupacao
                        .OrderByDescending(x => x.Id)
                        .Take(quantidade)
                        .ToList();
                }
            }
            catch
            {
                LoggerService.Erro("Erro ao buscar ultimos historicos.");
                return new List<HistoricoOcupacao>();
            }
        }
    }
}