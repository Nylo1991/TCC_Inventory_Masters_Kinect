using System;
using System.Collections.Generic;
using TCC_Inventory_Masters_Kinect.Model;
using Xunit;

namespace TCC_Inventory_Masters_Kinect.Diulie.Tests.ViewModel
{
    public class MainViewModelHistoricoTests
    {
        [Fact]
        public void Construtor_ComMedicoesNoRepositorio_DeveCarregarHistorico()
        {
            // Arrange
            var repository = new KinectRepositoryFake
            {
                Medicoes = new List<MedicaoVolume>
                {
                    new MedicaoVolume { Id = 1, VolumeCm3 = 1000000 },
                    new MedicaoVolume { Id = 2, VolumeCm3 = 1500000 }
                }
            };

            // Act
            var viewModel = ViewModelFactory.CriarMainViewModel(repository);

            // Assert
            Assert.Equal(2, viewModel.HistoricoMedicoes.Count);
            Assert.Equal(1, viewModel.HistoricoMedicoes[0].Id);
            Assert.Contains("2 medicoes carregadas", viewModel.StatusSQLite);
        }

        [Fact]
        public void CarregarHistoricoMedicoes_QuandoRepositorioMuda_DeveSubstituirColecao()
        {
            // Arrange
            var repository = new KinectRepositoryFake();
            var viewModel = ViewModelFactory.CriarMainViewModel(repository);
            var colecaoInicial = viewModel.HistoricoMedicoes;
            repository.Medicoes = new List<MedicaoVolume>
            {
                new MedicaoVolume { Id = 10, Status = "Medição manual" }
            };

            // Act
            viewModel.CarregarHistoricoMedicoes();

            // Assert
            Assert.NotSame(colecaoInicial, viewModel.HistoricoMedicoes);
            Assert.Single(viewModel.HistoricoMedicoes);
            Assert.Equal(10, viewModel.HistoricoMedicoes[0].Id);
        }

        [Fact]
        public void CarregarHistoricoMedicoes_QuandoRepositorioFalha_DeveExibirEstadoDeErro()
        {
            // Arrange
            var repository = new KinectRepositoryFake();
            var viewModel = ViewModelFactory.CriarMainViewModel(repository);
            repository.ErroAoConsultar = new InvalidOperationException("Falha simulada");

            // Act
            viewModel.CarregarHistoricoMedicoes();

            // Assert
            Assert.Empty(viewModel.HistoricoMedicoes);
            Assert.Equal("SQLite: erro ao carregar historico", viewModel.StatusSQLite);
        }
    }
}
