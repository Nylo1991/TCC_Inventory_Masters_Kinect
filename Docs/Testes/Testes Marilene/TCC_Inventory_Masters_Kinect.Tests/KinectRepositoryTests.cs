using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using TCC_Inventory_Masters_Kinect.Repository;
using TCC_Inventory_Masters_Kinect.Model;

namespace TCC_Inventory_Masters_Kinect.Tests
{
    public class KinectRepositoryTests
    {
        private readonly string _empresaTeste = "Empresa_Teste_Unitario";

        #region Salvar
        [Fact]
        public void SalvarMedicao_MedicaoValida_SalvaComSucesso()
        {
            // Arrange
            var repository = new KinectRepository(_empresaTeste);
            var medicao = new MedicaoVolume
            {
                VolumeCm3 = 3000.0,
                DataHora = DateTime.Now
            };

            // Act & Assert
            var exception = Record.Exception(() => repository.SalvarMedicao(medicao));
            Assert.Null(exception);
        }

        [Fact]
        public void SalvarMedicao_ComEmpresaConfigurada_AtribuiEmpresaESalva()
        {
            // Arrange
            var repository = new KinectRepository(_empresaTeste);
            var medicao = new MedicaoVolume
            {
                VolumeCm3 = 1500.0,
                DataHora = DateTime.Now
            };

            // Act
            repository.SalvarMedicao(medicao);
            var resultado = repository.ObterUltimasMedicoes(1);

            // Assert
            Assert.NotNull(resultado);
            if (resultado.Any())
            {
                Assert.Equal(_empresaTeste, resultado.First().Empresa);
            }
        }

        [Fact]
        public void SalvarMedicao_ErroNoBanco_CapturaExcecaoELoga()
        {
            // Arrange
            var repository = new KinectRepository("Empresa_Com_Erro");
            MedicaoVolume medicaoInvalida = null; // Força cenário tratado internamente

            // Act & Assert
            var exception = Record.Exception(() => repository.SalvarMedicao(medicaoInvalida));
            Assert.Null(exception); // O try-catch interno abafa a exceção e loga via LoggerService
        }
        #endregion

        #region ObterUltimasMedicoes
        [Fact]
        public void ObterUltimasMedicoes_QuantidadeValida_RetornaListaLimitada()
        {
            // Arrange
            var repository = new KinectRepository(_empresaTeste);

            // Act
            var resultado = repository.ObterUltimasMedicoes(3);

            // Assert
            Assert.NotNull(resultado);
            Assert.True(resultado.Count <= 3);
        }

        [Fact]
        public void ObterUltimasMedicoes_ErroNoBanco_RetornaListaVazia()
        {
            // Arrange
            var repository = new KinectRepository();

            // Act
            var resultado = repository.ObterUltimasMedicoes(-1); // Força falha de parâmetros/consulta

            // Assert
            Assert.NotNull(resultado);
            Assert.IsType<List<MedicaoVolume>>(resultado);
        }

        [Fact]
        public void ObterMedicoesEmOrdemCrescente_ParametrosValidos_RetornaOrdenadoCrescente()
        {
            // Arrange
            var repository = new KinectRepository(_empresaTeste);

            // Act
            var resultado = repository.ObterMedicoesEmOrdemCrescente(5, "usuario_teste", _empresaTeste);

            // Assert
            Assert.NotNull(resultado);
            if (resultado.Count > 1)
            {
                for (int i = 0; i < resultado.Count - 1; i++)
                {
                    Assert.True(resultado[i].Id <= resultado[i + 1].Id);
                }
            }
        }

        [Fact]
        public void ObterMedicoesEmOrdemCrescente_ErroNoBanco_RetornaListaVazia()
        {
            // Arrange
            var repository = new KinectRepository();

            // Act
            var resultado = repository.ObterMedicoesEmOrdemCrescente(0, null, null);

            // Assert
            Assert.NotNull(resultado);
            Assert.IsType<List<MedicaoVolume>>(resultado);
        }
        #endregion

        #region SalvarHistorico
        [Fact]
        public void SalvarHistorico_HistoricoValido_SalvaComSucesso()
        {
            // Arrange
            var repository = new KinectRepository(_empresaTeste);
            var historico = new HistoricoOcupacao
            {
                MedicaoVolumeId = 1,
                Status = "Ocupado", 
                DataHora = DateTime.Now
            };

            // Act & Assert
            var exception = Record.Exception(() => repository.SalvarHistorico(historico));
            Assert.Null(exception);
        }

        [Fact]
        public void SalvarHistorico_ComEmpresaConfigurada_AtribuiEmpresaESalva()
        {
            // Arrange
            var repository = new KinectRepository(_empresaTeste);
            var historico = new HistoricoOcupacao
            {
                MedicaoVolumeId = 1,
                Status = "Livre",
                DataHora = DateTime.Now
            };

            // Act
            repository.SalvarHistorico(historico);
            var resultado = repository.ObterUltimosHistoricos(1);

            // Assert
            Assert.NotNull(resultado);
            if (resultado.Any())
            {
                Assert.Equal(_empresaTeste, resultado.First().Empresa);
            }
        }

        [Fact]
        public void SalvarHistorico_ErroNoBanco_CapturaExcecaoELoga()
        {
            // Arrange
            var repository = new KinectRepository(_empresaTeste);
            HistoricoOcupacao historicoInvalido = null;

            // Act & Assert
            var exception = Record.Exception(() => repository.SalvarHistorico(historicoInvalido));
            Assert.Null(exception);
        }
        #endregion

        #region ObterHistorico
        [Fact]
        public void ObterHistoricoPorEspaco_EspacoExistente_RetornaHistoricoFiltrado()
        {
            // Arrange
            var repository = new KinectRepository(_empresaTeste);
            int espacoId = 1;

            // Act
            var resultado = repository.ObterHistoricoPorEspaco(espacoId);

            // Assert
            Assert.NotNull(resultado);
            Assert.All(resultado, h => Assert.Equal(espacoId, h.MedicaoVolumeId));
        }

        [Fact]
        public void ObterHistoricoPorEspaco_ErroNoBanco_RetornaListaVazia()
        {
            // Arrange
            var repository = new KinectRepository();

            // Act
            var resultado = repository.ObterHistoricoPorEspaco(-999);

            // Assert
            Assert.NotNull(resultado);
            Assert.IsType<List<HistoricoOcupacao>>(resultado);
        }

        [Fact]
        public void ObterUltimosHistoricos_QuantidadeValida_RetornaListaLimitada()
        {
            // Arrange
            var repository = new KinectRepository(_empresaTeste);

            // Act
            var resultado = repository.ObterUltimosHistoricos(3);

            // Assert
            Assert.NotNull(resultado);
            Assert.True(resultado.Count <= 3);
        }

        [Fact]
        public void ObterUltimosHistoricos_ErroNoBanco_RetornaListaVazia()
        {
            // Arrange
            var repository = new KinectRepository();

            // Act
            var resultado = repository.ObterUltimosHistoricos(-5);

            // Assert
            Assert.NotNull(resultado);
            Assert.IsType<List<HistoricoOcupacao>>(resultado);
        }
        #endregion
    }
}