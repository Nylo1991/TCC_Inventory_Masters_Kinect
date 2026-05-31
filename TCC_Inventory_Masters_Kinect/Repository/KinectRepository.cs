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
        // ==========================================
        // MEDIÇÃO DE VOLUME
        // ==========================================

        public void SalvarMedicao(
            MedicaoVolume medicao)
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    db.MedicaoVolumes.Add(
                        medicao);

                    db.SaveChanges();
                }

                LoggerService.Info(
                    $"Medição salva no SQLite. Volume: {medicao.VolumeCm3:F0} cm³");
            }
            catch (Exception ex)
            {
                LoggerService.Erro(
                    "Erro ao salvar medição no SQLite.",
                    ex);
            }
        }

        // ==========================================
        // BUSCAR ÚLTIMAS MEDIÇÕES
        // ==========================================

        public List<MedicaoVolume> ObterUltimasMedicoes(
            int quantidade)
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    return db.MedicaoVolumes
                        .OrderByDescending(
                            x => x.Id)
                        .Take(
                            quantidade)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                LoggerService.Erro(
                    "Erro ao buscar histórico de medições no SQLite.",
                    ex);

                return new List<MedicaoVolume>();
            }
        }

        // ==========================================
        // ESPAÇO MAPEADO
        // ==========================================

        public void SalvarEspaco(
            EspacoMapeado espaco)
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    db.EspacosMapeados.Add(
                        espaco);

                    db.SaveChanges();
                }

                LoggerService.Info(
                    "Espaço mapeado salvo no SQLite: " + espaco.NomeEspaco);
            }
            catch (Exception ex)
            {
                LoggerService.Erro(
                    "Erro ao salvar espaço mapeado no SQLite.",
                    ex);
            }
        }

        public EspacoMapeado ObterEspaco(
            int id)
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    return db.EspacosMapeados
                        .FirstOrDefault(
                            x => x.Id == id);
                }
            }
            catch (Exception ex)
            {
                LoggerService.Erro(
                    "Erro ao buscar espaço por ID.",
                    ex);

                return null;
            }
        }

        public EspacoMapeado ObterEspacoPorNome(
            string nomeEspaco)
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    return db.EspacosMapeados
                        .FirstOrDefault(
                            x => x.NomeEspaco ==
                                 nomeEspaco);
                }
            }
            catch (Exception ex)
            {
                LoggerService.Erro(
                    "Erro ao buscar espaço por nome.",
                    ex);

                return null;
            }
        }

        // ==========================================
        // HISTÓRICO DE OCUPAÇÃO
        // ==========================================

        public void SalvarHistorico(
            HistoricoOcupacao historico)
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    db.HistoricosOcupacao.Add(
                        historico);

                    db.SaveChanges();
                }

                LoggerService.Info(
                    "Histórico de ocupação salvo no SQLite.");
            }
            catch (Exception ex)
            {
                LoggerService.Erro(
                    "Erro ao salvar histórico de ocupação no SQLite.",
                    ex);
            }
        }

        // ==========================================
        // SNAPSHOT ESPACIAL
        // ==========================================

        public void SalvarSnapshot(
            SnapshotEspacial snapshot)
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    db.SnapshotsEspaciais.Add(
                        snapshot);

                    db.SaveChanges();
                }

                LoggerService.Info(
                    "Snapshot espacial salvo no SQLite: " + snapshot.NomeSnapshot);
            }
            catch (Exception ex)
            {
                LoggerService.Erro(
                    "Erro ao salvar snapshot espacial no SQLite.",
                    ex);
            }
        }
    }
}