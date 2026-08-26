using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Microsoft.AspNetCore.Http;
using Xunit;
using MVC_InventoryMasters.Models;
using MVC_InventoryMasters.Repositories;
using MVC_InventoryMasters.Services;

namespace MVC_InventoryMasters.Tests.Repository
{
    public class ParametrosSistemaRepositoryTests
    {
        private ParametrosSistemaRepository CriarRepositorio(string projetoId = "inventorymasters")
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    {"Firebase:CredentialPath", "Config/inventorymasters_firebase.json"},
                    {"Firebase:ProjectId", projetoId}
                })
                .Build();

            var firebaseService = new FirebaseService(configuration);
            var logger = NullLogger<ParametrosSistemaRepository>.Instance;

            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            var contextoUsuario = new ContextoUsuarioService(httpContextAccessorMock.Object);

            return new ParametrosSistemaRepository(firebaseService, logger, contextoUsuario);
        }

        #region BuscarPorEmpresa
        [Fact]
        public void BuscarPorEmpresa_EmpresaExistente_RetornaParametrosConfigurados()
        {
            // Arrange
            var repository = CriarRepositorio();
            string empresaIdTeste = "empresa_teste_01";

            var parametrosOriginais = new ParametrosSistema
            {
                EmpresaId = empresaIdTeste,
                CapacidadeMaxima = 500,
                CapacidadeMinima = 10,
                PercentualAlerta = 75,
                NotificacaoAutomatica = true
            };

            repository.Salvar(parametrosOriginais);

            // Act
            var resultado = repository.BuscarPorEmpresa(empresaIdTeste);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(empresaIdTeste, resultado.EmpresaId);
            Assert.Equal(500, resultado.CapacidadeMaxima);
            Assert.Equal(10, resultado.CapacidadeMinima);
            Assert.Equal(75, resultado.PercentualAlerta);
            Assert.True(resultado.NotificacaoAutomatica);
        }

        [Fact]
        public void BuscarPorEmpresa_EmpresaInexistenteComFallbackGlobal_RetornaConfiguracaoGlobal()
        {
            // Arrange
            var repository = CriarRepositorio();
         
            var configuracaoGlobal = new ParametrosSistema
            {
                EmpresaId = "global",
                CapacidadeMaxima = 1000,
                PercentualAlerta = 50
            };
            repository.Salvar(configuracaoGlobal);
            
            string empresaInexistenteId = "empresa_sem_cadastro_99";

            // Act
            var resultado = repository.BuscarPorEmpresa(empresaInexistenteId);

            // Assert
            Assert.NotNull(resultado);            
            Assert.Equal(empresaInexistenteId, resultado.EmpresaId);           
            Assert.Equal(1000, resultado.CapacidadeMaxima);
            Assert.Equal(50, resultado.PercentualAlerta);
        }

        [Fact]
        public void BuscarPorEmpresa_ErroNoBanco_RetornaParametrosPadrao()
        {
            // Arrange 
            var repositoryErro = CriarRepositorio("projeto_invalido_inexistente_xyz");
            string empresaId = "qualquer_empresa";

            // Act
            var resultado = repositoryErro.BuscarPorEmpresa(empresaId);

            // Assert            
            Assert.NotNull(resultado);            
            Assert.IsType<ParametrosSistema>(resultado);
        }
        #endregion

        #region ObterPadroes
        [Fact]
        public void ObterPadroes_ChamadaPadrao_RetornaValoresIniciaisPredefinidos()
        {
            // Arrange
            var repository = CriarRepositorio();

            // Act
            var resultado = repository.ObterPadroes();

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(0, resultado.CapacidadeMinima);
            Assert.True(resultado.PercentualAlerta >= 0);
        }
        #endregion

        #region Salvar
        [Fact]
        public void Salvar_ParametrosValidos_SalvaComSucesso()
        {
            // Arrange
            var repository = CriarRepositorio();
            string empresaIdTeste = "empresa_salvar_01";

            var novosParametros = new ParametrosSistema
            {
                EmpresaId = empresaIdTeste,
                CapacidadeMaxima = 800,
                CapacidadeMinima = 20,
                PercentualAlerta = 80,
                NotificacaoAutomatica = true
            };

            // Act & Assert
            var exception = Record.Exception(() => repository.Salvar(novosParametros));
            Assert.Null(exception);

            var resultadoPersistido = repository.BuscarPorEmpresa(empresaIdTeste);
            Assert.NotNull(resultadoPersistido);
            Assert.Equal(800, resultadoPersistido.CapacidadeMaxima);
            Assert.Equal(80, resultadoPersistido.PercentualAlerta);
        }

        [Fact]
        public void Salvar_ErroNoBanco_LancaExcecao()
        {
            // Arrange 
            var repositoryErro = CriarRepositorio("projeto_invalido_inexistente_xyz");

            var parametrosInvalidos = new ParametrosSistema
            {
                EmpresaId = "empresa_erro",
                CapacidadeMaxima = 100
            };

            // Act & Assert
            Assert.Throws<Exception>(() => repositoryErro.Salvar(parametrosInvalidos));
        }
        #endregion

        #region CalcularPercentualOcupacao
        [Fact]
        public void CalcularPercentualOcupacao_ValoresValidos_RetornaPercentualCorreto()
        {
            // Arrange
            var repository = CriarRepositorio();

            int quantidadeAtual = 50;
            int capacidadeMaxima = 200;

            // Act
            double percentual = repository.CalcularPercentualOcupacao(quantidadeAtual, capacidadeMaxima);

            // Assert            
            Assert.Equal(25.0, percentual);
        }
        

        [Fact]
        public void CalcularPercentualOcupacao_CapacidadeMaximaZeroOuNegativa_RetornaZero()
        {
            // Arrange
            var repository = CriarRepositorio();

            int quantidadeAtual = 50;
            int capacidadeZero = 0;
            int capacidadeNegativa = -10;

            // Act
            double resultadoZero = repository.CalcularPercentualOcupacao(quantidadeAtual, capacidadeZero);
            double resultadoNegativo = repository.CalcularPercentualOcupacao(quantidadeAtual, capacidadeNegativa);

            // Assert           
            Assert.Equal(0.0, resultadoZero);
            Assert.Equal(0.0, resultadoNegativo);
        }
        #endregion

        #region KinectSensorValidations
        [Fact]
        public void Salvar_RaioDeteccaoKinect_DentroDosLimitesPermitidos_SalvaComSucesso()
        {
            // Arrange
            var repository = CriarRepositorio();
            var parametros = new ParametrosSistema
            {
                EmpresaId = "empresa_kinect_01",
                RaioDeteccaoKinect = 2.5 
            };

            // Act & Assert
            var exception = Record.Exception(() => repository.Salvar(parametros));
            Assert.Null(exception);

            var resultado = repository.BuscarPorEmpresa("empresa_kinect_01");
            Assert.Equal(2.5, resultado.RaioDeteccaoKinect);
        }

        [Theory]
        [InlineData(0.2)] 
        [InlineData(5.5)] 
        public void Validar_RaioDeteccaoKinect_LimitesHardware(double raioTeste)
        {
            // Arrange
            var repository = CriarRepositorio();
            var parametros = new ParametrosSistema
            {
                EmpresaId = "empresa_kinect_limites",
                RaioDeteccaoKinect = raioTeste
            };

            // Act
            repository.Salvar(parametros);
            var resultado = repository.BuscarPorEmpresa("empresa_kinect_limites");

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(raioTeste, resultado.RaioDeteccaoKinect);
        }
        #endregion

        #region Buscar
        [Fact]
        public void Buscar_ChamadaSemParametros_UtilizaContextoUsuarioEfetuaBusca()
        {
            // Arrange
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    {"Firebase:CredentialPath", "Config/inventorymasters_firebase.json"},
                    {"Firebase:ProjectId", "inventorymasters"}
                })
                .Build();

            var firebaseService = new FirebaseService(configuration);
            var logger = NullLogger<ParametrosSistemaRepository>.Instance;
            
            string empresaUsuario = "empresa_contexto_01";
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
           
            var contextoUsuario = new ContextoUsuarioService(httpContextAccessorMock.Object);

            var repository = new ParametrosSistemaRepository(firebaseService, logger, contextoUsuario);

            // Act & Assert          
            var exception = Record.Exception(() => repository.Buscar());
            Assert.Null(exception);
        }
        #endregion
    }
}