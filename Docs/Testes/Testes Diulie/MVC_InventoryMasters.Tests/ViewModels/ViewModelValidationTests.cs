using System.ComponentModel.DataAnnotations;
using MVC_InventoryMasters.ViewModels;

namespace MVC_InventoryMasters.Diulie.Tests.ViewModels;

public class ViewModelValidationTests
{
    public static IEnumerable<object[]> EmailModelsInvalidos()
    {
        yield return new object[] { new LoginEmailViewModel(), nameof(LoginEmailViewModel.Email) };
        yield return new object[] { new LoginEmailViewModel { Email = "email-invalido" }, nameof(LoginEmailViewModel.Email) };
        yield return new object[] { new SolicitarTokenKinectRequest(), nameof(SolicitarTokenKinectRequest.Email) };
        yield return new object[] { new SolicitarTokenKinectRequest { Email = "email-invalido" }, nameof(SolicitarTokenKinectRequest.Email) };
    }

    public static IEnumerable<object[]> EmailModelsValidos()
    {
        yield return new object[] { new LoginEmailViewModel { Email = "usuario@exemplo.com" } };
        yield return new object[] { new SolicitarTokenKinectRequest { Email = "usuario@exemplo.com" } };
    }

    [Theory]
    [MemberData(nameof(EmailModelsInvalidos))]
    public void EmailInvalido_DeveFalharValidacao(object model, string propriedade)
    {
        var resultados = Validar(model);

        Assert.Contains(resultados, resultado =>
            resultado.MemberNames.Contains(propriedade));
    }

    [Theory]
    [MemberData(nameof(EmailModelsValidos))]
    public void EmailValido_DevePassarValidacao(object model)
    {
        Assert.Empty(Validar(model));
    }

    [Fact]
    public void LoginEmailViewModel_DisplayName_DeveSerEmail()
    {
        var propriedade = typeof(LoginEmailViewModel)
            .GetProperty(nameof(LoginEmailViewModel.Email));
        var atributo = propriedade!.GetCustomAttributes(typeof(DisplayAttribute), false)
            .Cast<DisplayAttribute>()
            .Single();

        Assert.Equal("E-mail", atributo.Name);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("12345")]
    [InlineData("1234567")]
    [InlineData("123456789012")]
    [InlineData("1234567890123")]
    public void ValidarTokenViewModel_TamanhoInvalido_DeveFalhar(string? token)
    {
        var model = new ValidarTokenViewModel { Token = token };

        Assert.NotEmpty(Validar(model));
    }

    [Theory]
    [InlineData("123456")]
    public void ValidarTokenViewModel_TamanhoPermitido_DevePassar(string token)
    {
        var model = new ValidarTokenViewModel { Token = token };

        Assert.Empty(Validar(model));
    }

    [Fact]
    public void ValidarTokenRequest_DeveArmazenarToken()
    {
        var model = new ValidarTokenRequest { Token = "ABC123" };

        Assert.Equal("ABC123", model.Token);
    }

    [Fact]
    public void ValidacaoTokenResultadoViewModel_DeveArmazenarResultadoCompleto()
    {
        var model = new ValidacaoTokenResultadoViewModel
        {
            TokenValido = true,
            EmailValidado = true,
            Usuario = "Usuário",
            Empresa = "Inventory Masters",
            Email = "usuario@exemplo.com",
            Mensagem = "Token validado."
        };

        Assert.True(model.TokenValido);
        Assert.True(model.EmailValidado);
        Assert.Equal("Usuário", model.Usuario);
        Assert.Equal("Inventory Masters", model.Empresa);
        Assert.Equal("usuario@exemplo.com", model.Email);
        Assert.Equal("Token validado.", model.Mensagem);
    }

    private static List<ValidationResult> Validar(object model)
    {
        var resultados = new List<ValidationResult>();
        Validator.TryValidateObject(
            model,
            new ValidationContext(model),
            resultados,
            validateAllProperties: true);
        return resultados;
    }
}
