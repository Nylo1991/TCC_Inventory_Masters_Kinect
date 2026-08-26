using System.ComponentModel;
using TCC_Inventory_Masters_Kinect.ViewModel;
using Xunit;

namespace TCC_Inventory_Masters_Kinect.Diulie.Tests.ViewModel
{
    public class BaseViewModelTests
    {
        #region Teste SetProperty altera valor diferente

        [Fact]
        public void SetProperty_ValorDiferente_AlteraValorERetornaTrue()
        {
            // Arrange
            var viewModel = new BaseViewModelTeste();

            // Act
            var resultado = viewModel.AlterarNome("Produto");

            // Assert
            Assert.True(resultado);
            Assert.Equal("Produto", viewModel.Nome);
        }

        #endregion


        #region Teste SetProperty dispara PropertyChanged

        [Fact]
        public void SetProperty_ValorDiferente_DisparaPropertyChanged()
        {
            // Arrange
            var viewModel = new BaseViewModelTeste();

            string propriedadeAlterada = null;

            viewModel.PropertyChanged += (sender, e) =>
            {
                propriedadeAlterada = e.PropertyName;
            };

            // Act
            viewModel.AlterarNome("Produto");

            // Assert
            Assert.Equal("Nome", propriedadeAlterada);
        }

        #endregion


        #region Teste SetProperty com valor igual

        [Fact]
        public void SetProperty_ValorIgual_NaoAlteraERetornaFalse()
        {
            // Arrange
            var viewModel = new BaseViewModelTeste();

            viewModel.AlterarNome("Produto");

            // Act
            var resultado = viewModel.AlterarNome("Produto");

            // Assert
            Assert.False(resultado);
            Assert.Equal("Produto", viewModel.Nome);
        }

        #endregion


        #region Teste SetProperty valor igual não dispara PropertyChanged

        [Fact]
        public void SetProperty_ValorIgual_NaoDisparaPropertyChanged()
        {
            // Arrange
            var viewModel = new BaseViewModelTeste();

            viewModel.AlterarNome("Produto");

            var quantidadeEventos = 0;

            viewModel.PropertyChanged += (sender, e) =>
            {
                quantidadeEventos++;
            };

            // Act
            viewModel.AlterarNome("Produto");

            // Assert
            Assert.Equal(0, quantidadeEventos);
        }

        #endregion


        /// <summary>
        /// Classe auxiliar criada apenas para testar os métodos
        /// protegidos da BaseViewModel.
        /// </summary>
        private class BaseViewModelTeste : BaseViewModel
        {
            private string _nome;

            public string Nome
            {
                get => _nome;
                set => SetProperty(ref _nome, value);
            }

            public bool AlterarNome(string valor)
            {
                return SetProperty(ref _nome, valor, nameof(Nome));
            }
        }
    }
}