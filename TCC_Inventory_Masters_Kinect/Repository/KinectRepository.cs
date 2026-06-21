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
        /// Salva uma medição volumétrica associada à empresa autenticada,
        /// garantindo o isolamento dos dados entre diferentes empresas.
        /// </summary>
        public void SalvarMedicao(MedicaoVolume medicao)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(_empresa))
                {
                    medicao.Empresa = _empresa;
                }

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
        /// Obtém as últimas medições registradas da empresa autenticada,
        /// ordenadas da mais recente para a mais antiga, impedindo o acesso
        /// a dados pertencentes a outras empresas.
        /// </summary>
        public List<MedicaoVolume> ObterUltimasMedicoes(int quantidade)
        {
            try
            {
                using (var db = new AppDbContext(_empresa))
                {
                    var consulta = db.MedicaoVolumes.AsQueryable();

                    if (!string.IsNullOrWhiteSpace(_empresa))
                    {
                        consulta = consulta.Where(x => x.Empresa == _empresa);
                    }

                    return consulta
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
        /// Obtém as medições em ordem crescente de identificação,
        /// aplicando filtro por usuário e empresa para garantir que
        /// apenas registros autorizados sejam retornados.
        /// </summary>
        public List<MedicaoVolume> ObterMedicoesEmOrdemCrescente(int quantidade, string usuario, string empresa)
        {
            try
            {
                using (var db = new AppDbContext(_empresa))
                {
                    var consulta = db.MedicaoVolumes.AsQueryable();

                    if (!string.IsNullOrWhiteSpace(usuario))
                    {
                        consulta = consulta.Where(x => x.Usuario == usuario);
                    }

                    if (!string.IsNullOrWhiteSpace(empresa))
                    {
                        consulta = consulta.Where(x => x.Empresa == empresa);
                    }

                    return consulta
                        .OrderByDescending(x => x.Id)
                        .Take(quantidade)
                        .OrderBy(x => x.Id)
                        .ToList();
                }
            }
            catch
            {
                LoggerService.Erro("Erro ao buscar medicoes em ordem crescente por usuario e empresa.");
                return new List<MedicaoVolume>();
            }
        }

        /// <summary>
        /// Salva um registro de histórico de ocupação vinculado à empresa autenticada,
        /// garantindo o isolamento dos dados entre diferentes empresas.
        /// </summary>
        public void SalvarHistorico(HistoricoOcupacao historico)
        {
            try
            {
               
                if (!string.IsNullOrWhiteSpace(_empresa))
                {
                    historico.Empresa = _empresa;
                }

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
        /// Obtém o histórico de ocupação de um espaço específico,
        /// aplicando filtro de segurança por empresa para impedir
        /// o acesso a registros pertencentes a outras Empresas.
        /// </summary>
        public List<HistoricoOcupacao> ObterHistoricoPorEspaco(int espacoId)
        {
            try
            {
                using (var db = new AppDbContext(_empresa))
                {
                    var consulta = db.HistoricosOcupacao
                        .Where(x => x.EspacoMapeadoId == espacoId);

                   
                    if (!string.IsNullOrWhiteSpace(_empresa))
                    {
                        consulta = consulta.Where(x => x.Empresa == _empresa);
                    }

                    return consulta
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
                    var consulta = db.HistoricosOcupacao.AsQueryable();

                    // Aplica filtro de segurança por empresa na consulta,
                    // impedindo o retorno de históricos pertencentes a outras empresas.
                    if (!string.IsNullOrWhiteSpace(_empresa))
                    {
                        consulta = consulta.Where(x => x.Empresa == _empresa);
                    }

                    return consulta
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