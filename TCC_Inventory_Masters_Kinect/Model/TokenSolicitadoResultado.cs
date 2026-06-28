namespace TCC_Inventory_Masters_Kinect.Model
{
    /// <summary>
    /// Classe que representa o resultado da solicitação de um token, 
    /// incluindo informações sobre o sucesso da operação, o e-mail associado e uma mensagem de retorno.
    /// </summary>
    public class TokenSolicitadoResultado
    {
        public bool Sucesso { get; set; }
        public string Email { get; set; }
        public string Mensagem { get; set; }
    }
}
