using System.Collections.Generic;
using System.Linq;
using TCC_Inventory_Masters_Kinect.Data;
using TCC_Inventory_Masters_Kinect.Logs;
using TCC_Inventory_Masters_Kinect.Model;
using TCC_Inventory_Masters_Kinect.Repository.Interface;

namespace TCC_Inventory_Masters_Kinect.Repository
{
    public class KinectRepository : IKinectRepository
    {
        public void SalvarMedicao(MedicaoVolume medicao)
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    db.MedicaoVolumes.Add(medicao);
                    db.SaveChanges();
                }

                LoggerService.Info($"Medicao salva no SQLite. Volume: {medicao.VolumeCm3:F0} cm³");
            }
            catch
            {
                LoggerService.Erro("Erro ao salvar medicao no SQLite.");
            }
        }

        public List<MedicaoVolume> ObterUltimasMedicoes(int quantidade)
        {
            try
            {
                using (var db = new AppDbContext())
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

        public void SalvarHistorico(HistoricoOcupacao historico)
        {
            try
            {
                using (var db = new AppDbContext())
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

        public List<HistoricoOcupacao> ObterHistoricoPorEspaco(int espacoId)
        {
            try
            {
                using (var db = new AppDbContext())
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

        public List<HistoricoOcupacao> ObterUltimosHistoricos(int quantidade)
        {
            try
            {
                using (var db = new AppDbContext())
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

        public List<MedicaoVolume> ObterMedicoesEmOrdemCrescente(int quantidade)
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    return db.MedicaoVolumes
                        .OrderBy(x => x.Id)
                        .Take(quantidade)
                        .ToList();
                }
            }
            catch
            {
                LoggerService.Erro("Erro ao buscar medicoes em ordem crescente.");
                return new List<MedicaoVolume>();
            }
        }
    }
}