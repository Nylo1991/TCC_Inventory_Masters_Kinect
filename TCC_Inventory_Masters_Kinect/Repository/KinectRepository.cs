using System;
using System.Linq;
using TCC_Inventory_Masters_Kinect.Data;
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
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Erro ao salvar medição: "
                    + ex.Message);
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
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Erro ao salvar espaço: "
                    + ex.Message);
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
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Erro ao salvar histórico: "
                    + ex.Message);
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
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Erro ao salvar snapshot: "
                    + ex.Message);
            }
        }

        // ==========================================
        // BUSCAR ESPAÇO POR ID
        // ==========================================

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
            catch
            {
                return null;
            }
        }

        // ==========================================
        // BUSCAR ESPAÇO POR NOME
        // ==========================================

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
            catch
            {
                return null;
            }
        }
    }
}