using System.Threading.Tasks;
using TCC_Inventory_Masters_Kinect.Model;

namespace TCC_Inventory_Masters_Kinect.Service
{
    /// <summary>
    /// Contrato de autenticação utilizado pelas ViewModels.
    /// Permite testar os fluxos sem acessar o MVC real.
    /// </summary>
    public interface IAutenticacaoMvcService
    {
        Task<TokenSolicitadoResultado> SolicitarTokenAsync(string email);
        Task<ValidacaoTokenResultado> ValidarTokenAsync(string token);
    }
}
