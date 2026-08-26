using System;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
using Microsoft.Extensions.Logging;
using Moq;
using MVC_InventoryMasters.Models;
using MVC_InventoryMasters.Repositories;
using MVC_InventoryMasters.Services;
using Xunit;

public class LogsSistemaRepositoryTests
{
    private readonly Mock<FirebaseService> _firebaseServiceMock;
    private readonly Mock<ILogger<LogsSistemaRepository>> _loggerMock;
    private readonly Mock<ContextoUsuarioService> _contextoUsuarioMock;
    private readonly LogsSistemaRepository _repository;

    public LogsSistemaRepositoryTests()
    {
        // Instancia o mock sem parâmetros, utilizando o construtor público vazio criado no FirebaseService
        _firebaseServiceMock = new Mock<FirebaseService>();

        _loggerMock = new Mock<ILogger<LogsSistemaRepository>>();
        _contextoUsuarioMock = new Mock<ContextoUsuarioService>(MockBehavior.Loose);

        _repository = new LogsSistemaRepository(
            _firebaseServiceMock.Object,
            _loggerMock.Object,
            _contextoUsuarioMock.Object
        );
    }

    #region Registrar
    [Fact]
    public async Task Registrar_ParametrosValidosComEmpresaIdInformado_AdicionaComSucesso()
    {
        // Arrange
        string acao = "Inserir";
        string mensagem = "Registro inserido com sucesso.";
        string nivel = "Informacao";
        string email = "teste@email.com";
        string usuarioId = "user-01";
        string empresaIdInformado = "empresa-999";

        // Act
        var exception = await Record.ExceptionAsync(() =>
            _repository.Registrar(acao, mensagem, nivel, email, usuarioId, empresaIdInformado));

        // Assert
        Assert.Null(exception);
        _contextoUsuarioMock.Verify(c => c.ObterEmpresaId(), Times.Never);
    }

    [Fact]
    public async Task Registrar_EmpresaIdNuloOuBranco_ObtemEmpresaDoContextoEAdicionaComSucesso()
    {
        // Arrange
        string acao = "Atualizar";
        string mensagem = "Registro atualizado.";
        string empresaContexto = "empresa-contexto-123";

        _contextoUsuarioMock.Setup(c => c.ObterEmpresaId()).Returns(empresaContexto);

        // Act
        var exception = await Record.ExceptionAsync(() =>
            _repository.Registrar(acao, mensagem, empresaId: null));

        // Assert
        Assert.Null(exception);
        _contextoUsuarioMock.Verify(c => c.ObterEmpresaId(), Times.Once);
    }

    [Fact]
    public async Task Registrar_ErroNoBanco_CapturaExcecaoELogaErroSemLancar()
    {
        // Arrange
        string acao = "Deletar";
        string mensagem = "Tentativa de exclusão.";
        _firebaseServiceMock.Setup(f => f.Firestore).Throws(new Exception("Erro de conexão com o Firestore"));

        // Act
        var exception = await Record.ExceptionAsync(() =>
            _repository.Registrar(acao, mensagem));

        // Assert
        Assert.Null(exception);

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