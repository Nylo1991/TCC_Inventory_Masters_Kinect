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
        public void SalvarMedicao(MedicaoVolume medicao)
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    db.MedicaoVolumes.Add(medicao);
                    db.SaveChanges();
                }

                LoggerService.Info(
                    $"Medicao salva no SQLite. Volume: {medicao.VolumeCm3:F0} cm3");
            }
            catch (Exception ex)
            {
                LoggerService.Erro(
                    "Erro ao salvar medicao no SQLite.", ex);
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
                LoggerService.Erro(
                    "Erro ao buscar historico de medicoes no SQLite.", ex);

                return new List<MedicaoVolume>();
            }
        }

        public void SalvarEspaco(EspacoMapeado espaco)
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    db.EspacosMapeados.Add(espaco);
                    db.SaveChanges();
                }

                LoggerService.Info(
                    "Espaco mapeado salvo no SQLite: " + espaco.NomeEspaco);
            }
            catch (Exception ex)
            {
                LoggerService.Erro(
                    "Erro ao salvar espaco mapeado no SQLite.", ex);
            }
        }

        public EspacoMapeado ObterEspaco(int id)
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    return db.EspacosMapeados
                        .FirstOrDefault(x => x.Id == id);
                }
            }
            catch (Exception ex)
            {
                LoggerService.Erro(
                    "Erro ao buscar espaco por ID.", ex);

                return null;
            }
        }

        public EspacoMapeado ObterEspacoPorNome(string nomeEspaco)
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    return db.EspacosMapeados
                        .FirstOrDefault(x => x.NomeEspaco == nomeEspaco);
                }
            }
            catch (Exception ex)
            {
                LoggerService.Erro(
                    "Erro ao buscar espaco por nome.", ex);

                return null;
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

                LoggerService.Info(
                    "Historico de ocupacao salvo no SQLite.");
            }
            catch (Exception ex)
            {
                LoggerService.Erro(
                    "Erro ao salvar historico de ocupacao no SQLite.", ex);
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
                LoggerService.Erro(
                    "Erro ao buscar historico por espaco.", ex);

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
                LoggerService.Erro(
                    "Erro ao buscar ultimos historicos.", ex);

                return new List<HistoricoOcupacao>();
            }
        }

        public void SalvarSnapshot(SnapshotEspacial snapshot)
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    db.SnapshotsEspaciais.Add(snapshot);
                    db.SaveChanges();
                }

                LoggerService.Info(
                    "Snapshot espacial salvo no SQLite: " + snapshot.NomeSnapshot);
            }
            catch (Exception ex)
            {
                LoggerService.Erro(
                    "Erro ao salvar snapshot espacial no SQLite.", ex);
            }
        }

        public List<SnapshotEspacial> ObterSnapshotsPorEspaco(int espacoId)
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    return db.SnapshotsEspaciais
                        .Where(x => x.EspacoMapeadoId == espacoId)
                        .OrderByDescending(x => x.Id)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                LoggerService.Erro(
                    "Erro ao buscar snapshots por espaco.", ex);

                return new List<SnapshotEspacial>();
            }
        }

        public SnapshotEspacial ObterUltimoSnapshot(int espacoId)
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    return db.SnapshotsEspaciais
                        .Where(x => x.EspacoMapeadoId == espacoId)
                        .OrderByDescending(x => x.Id)
                        .FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                LoggerService.Erro(
                    "Erro ao buscar ultimo snapshot.", ex);

                return null;
            }
        }
    }
}
