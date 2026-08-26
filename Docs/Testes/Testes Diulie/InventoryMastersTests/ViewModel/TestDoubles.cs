using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TCC_Inventory_Masters_Kinect.Model;
using TCC_Inventory_Masters_Kinect.Repository.Interface;
using TCC_Inventory_Masters_Kinect.Service;
using TCC_Inventory_Masters_Kinect.ViewModel;

namespace TCC_Inventory_Masters_Kinect.Diulie.Tests.ViewModel
{
    internal sealed class AutenticacaoMvcServiceFake : IAutenticacaoMvcService
    {
        public Func<string, Task<TokenSolicitadoResultado>> AoSolicitarToken { get; set; }
        public Func<string, Task<ValidacaoTokenResultado>> AoValidarToken { get; set; }

        public Task<TokenSolicitadoResultado> SolicitarTokenAsync(string email)
        {
            return AoSolicitarToken != null
                ? AoSolicitarToken(email)
                : Task.FromResult<TokenSolicitadoResultado>(null);
        }

        public Task<ValidacaoTokenResultado> ValidarTokenAsync(string token)
        {
            return AoValidarToken != null
                ? AoValidarToken(token)
                : Task.FromResult<ValidacaoTokenResultado>(null);
        }
    }

    internal sealed class KinectRepositoryFake : IKinectRepository
    {
        public List<MedicaoVolume> Medicoes { get; set; } = new List<MedicaoVolume>();
        public Exception ErroAoConsultar { get; set; }
        public MedicaoVolume MedicaoSalva { get; private set; }

        public void SalvarMedicao(MedicaoVolume medicao)
        {
            MedicaoSalva = medicao;
        }

        public List<MedicaoVolume> ObterUltimasMedicoes(int quantidade)
        {
            return Medicoes;
        }

        public List<MedicaoVolume> ObterMedicoesEmOrdemCrescente(
            int quantidade,
            string usuario,
            string empresa)
        {
            if (ErroAoConsultar != null)
            {
                throw ErroAoConsultar;
            }

            return Medicoes;
        }

        public void SalvarHistorico(HistoricoOcupacao historico)
        {
        }

        public List<HistoricoOcupacao> ObterHistoricoPorEspaco(int espacoId)
        {
            return new List<HistoricoOcupacao>();
        }

        public List<HistoricoOcupacao> ObterUltimosHistoricos(int quantidade)
        {
            return new List<HistoricoOcupacao>();
        }
    }

    internal static class ViewModelFactory
    {
        internal static SessaoUsuario CriarSessaoValida()
        {
            return new SessaoUsuario
            {
                Usuario = "Usuário Teste",
                Empresa = "Empresa Teste",
                Email = "teste@empresa.com",
                Token = "123456"
            };
        }

        internal static MainViewModel CriarMainViewModel(
            KinectRepositoryFake repository = null,
            AutenticacaoMvcServiceFake autenticacao = null)
        {
            return new MainViewModel(
                CriarSessaoValida(),
                new KinectService(),
                new SignalRService(),
                repository ?? new KinectRepositoryFake(),
                autenticacao ?? new AutenticacaoMvcServiceFake(),
                false);
        }
    }
}
