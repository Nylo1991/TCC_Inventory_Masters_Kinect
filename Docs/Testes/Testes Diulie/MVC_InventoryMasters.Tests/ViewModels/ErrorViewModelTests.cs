using MVC_InventoryMasters.Models;

namespace MVC_InventoryMasters.Diulie.Tests.ViewModels;

public class ErrorViewModelTests
{
    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("REQ-123", true)]
    public void ShowRequestId_DeveIndicarSeExisteIdentificador(
        string? requestId,
        bool esperado)
    {
        var model = new ErrorViewModel { RequestId = requestId };

        Assert.Equal(esperado, model.ShowRequestId);
    }
}
