using System;
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
        // ==================== MEDIÇÕES ====================
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
            catch (Exception ex)
            {
                LoggerService.Erro("Erro ao salvar medicao no SQLite.", ex);
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
            catch (Exception ex)
            {
                LoggerService.Erro("Erro ao buscar historico de medicoes no SQLite.", ex);
                return new List<MedicaoVolume>();
            }
        }

        // ==================== ESPAÇOS ====================
        public void SalvarEspaco(Space space)
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    db.Spaces.Add(space);
                    db.SaveChanges();
                }

                LoggerService.Info("Espaço salvo no SQLite: " + space.Name);
            }
            catch (Exception ex)
            {
                LoggerService.Erro("Erro ao salvar espaço no SQLite.", ex);
            }
        }

        public void AtualizarEspaco(Space space)
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    db.Entry(space).State = System.Data.Entity.EntityState.Modified;  // .NET Framework
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                LoggerService.Erro("Erro ao atualizar espaço.", ex);
            }
        }


        public List<Space> ObterTodosEspacos()
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    return db.Spaces.ToList();
                }
            }
            catch (Exception ex)
            {
                LoggerService.Erro("Erro ao buscar todos os espaços.", ex);
                return new List<Space>();
            }
        }

        public Space ObterEspaco(int id)
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    return db.Spaces.FirstOrDefault(x => x.Id == id);
                }
            }
            catch (Exception ex)
            {
                LoggerService.Erro("Erro ao buscar espaço por ID.", ex);
                return null;
            }
        }

        public Space ObterEspacoPorNome(string nomeEspaco)
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    return db.Spaces.FirstOrDefault(x => x.Name == nomeEspaco);
                }
            }
            catch (Exception ex)
            {
                LoggerService.Erro("Erro ao buscar espaço por nome.", ex);
                return null;
            }
        }

        // ==================== HISTÓRICO DE OCUPAÇÃO ====================
        public void SalvarHistorico(HistoricoOcupacao historico)
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    db.HistoricosOcupacao.Add(historico);
                    db.SaveChanges();
                }

                LoggerService.Info("Histórico de ocupação salvo no SQLite.");
            }
            catch (Exception ex)
            {
                LoggerService.Erro("Erro ao salvar histórico de ocupação no SQLite.", ex);
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
            catch (Exception ex)
            {
                LoggerService.Erro("Erro ao buscar histórico por espaço.", ex);
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
            catch (Exception ex)
            {
                LoggerService.Erro("Erro ao buscar últimos históricos.", ex);
                return new List<HistoricoOcupacao>();
            }
        }
    }
}
