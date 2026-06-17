using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TCC_Inventory_Masters_Kinect.ConfigKinect;
using TCC_Inventory_Masters_Kinect.Logs;
using TCC_Inventory_Masters_Kinect.Model;

namespace TCC_Inventory_Masters_Kinect.Service
{
    public class AutenticacaoMvcService
    {
        private readonly HttpClient _httpClient;

        public AutenticacaoMvcService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<ValidacaoTokenResultado> ValidarTokenAsync(string token)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(token))
                {
                    return new ValidacaoTokenResultado
                    {
                        TokenValido = false,
                        EmailValidado = false,
                        Mensagem = "Informe o token de acesso."
                    };
                }

                var dados = new
                {
                    Token = token
                };

                string json = JsonConvert.SerializeObject(dados);

                var conteudo = new StringContent(
                    json,
                    Encoding.UTF8,
                    "application/json"
                );

                var resposta = await _httpClient.PostAsync(
                    KinectConfig.UrlValidarTokenMvc,
                    conteudo
                );

                if (!resposta.IsSuccessStatusCode)
                {
                    LoggerService.LogWarning("MVC recusou a validacao do token.");

                    return new ValidacaoTokenResultado
                    {
                        TokenValido = false,
                        EmailValidado = false,
                        Mensagem = "Token invalido ou expirado."
                    };
                }

                string retornoJson = await resposta.Content.ReadAsStringAsync();

                var resultado = JsonConvert.DeserializeObject<ValidacaoTokenResultado>(
                    retornoJson
                );

                if (resultado == null)
                {
                    return new ValidacaoTokenResultado
                    {
                        TokenValido = false,
                        EmailValidado = false,
                        Mensagem = "Resposta invalida do MVC."
                    };
                }

                return resultado;
            }
            catch
            {
                LoggerService.Erro("Erro ao validar token no MVC.");

                return new ValidacaoTokenResultado
                {
                    TokenValido = false,
                    EmailValidado = false,
                    Mensagem = "Nao foi possivel validar o token no MVC."
                };
            }
        }
    }
}