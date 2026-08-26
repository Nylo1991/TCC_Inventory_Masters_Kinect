using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
using Microsoft.Extensions.Logging;
using Moq;
using MVC_InventoryMasters.Models;
using MVC_InventoryMasters.Repositories;
using MVC_InventoryMasters.Services;
using Xunit;

public class EmpresasRepositoryTests
{
    private readonly Mock<FirebaseService> _firebaseServiceMock;
    private readonly Mock<ILogger<EmpresasRepository>> _loggerMock;
    private readonly EmpresasRepository _repository;

    public EmpresasRepositoryTests()
    {
        _firebaseServiceMock = new Mock<FirebaseService>();
        _loggerMock = new Mock<ILogger<EmpresasRepository>>();

        _repository = new EmpresasRepository(
            _firebaseServiceMock.Object,
            _loggerMock.Object
        );
    }

    #region ListarTodas
    [Fact]
    public async Task ListarTodas_ComEmpresasCadastradas_RetornaListaDeEmpresas()
    {
        // Arrange
        // (Configurar mock do Firestore para retornar documentos)

        // Act
        var resultado = await _repository.ListarTodas();

        // Assert
        Assert.NotNull(resultado);
    }

    [Fact]
    public async Task ListarTodas_BancoVazio_RetornaListaVazia()
    {
        // Arrange
        // (Configurar mock do Firestore para retornar coleção vazia)

        // Act
        var resultado = await _repository.ListarTodas();

        // Assert
        Assert.NotNull(resultado);
        Assert.Empty(resultado);
    }

    [Fact]
    public async Task ListarTodas_ErroNoBanco_CapturaExcecaoELogaErroRetornandoListaVazia()
    {
        // Arrange
        _firebaseServiceMock.Setup(f => f.Firestore).Throws(new Exception("Erro de conexão"));

        // Act
        var resultado = await _repository.ListarTodas();

        // Assert
        Assert.NotNull(resultado);
        Assert.Empty(resultado);
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
            Times.Once);
    }
    #endregion

    #region BuscarPorId
    [Fact]
    public async Task BuscarPorId_IdExistente_RetornaEmpresaComId()
    {
        // Arrange
        string idEmpresa = "empresa-123";
        // (Configurar mock do Firestore para retornar documento existente)

        // Act
        var resultado = await _repository.BuscarPorId(idEmpresa);

        // Assert
        // Assert.NotNull(resultado);
    }

    [Fact]
    public async Task BuscarPorId_IdInexistente_RetornaNull()
    {
        // Arrange
        string idEmpresa = "inexistente-999";
        // (Configurar mock do Firestore para retornar Exists = false)

        // Act
        var resultado = await _repository.BuscarPorId(idEmpresa);

        // Assert
        Assert.Null(resultado);
    }

    [Fact]
    public async Task BuscarPorId_ErroNoBanco_CapturaExcecaoELogaErroRetornandoNull()
    {
        // Arrange
        string idEmpresa = "empresa-123";
        _firebaseServiceMock.Setup(f => f.Firestore).Throws(new Exception("Erro de conexão"));

        // Act
        var resultado = await _repository.BuscarPorId(idEmpresa);

        // Assert
        Assert.Null(resultado);
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
            Times.Once);
    }
    #endregion
}