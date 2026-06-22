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
            // A chamada é local; não deve depender de proxy corporativo/configurado no Windows.
            var handler = new HttpClientHandler
            {
                UseProxy = false
            };

            _httpClient = new HttpClient(handler)
            {
                Timeout = System.TimeSpan.FromSeconds(15)
            };
        }

        public async Task<TokenSolicitadoResultado> SolicitarTokenAsync(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    return new TokenSolicitadoResultado
                    {
                        Sucesso = false,
                        Mensagem = "Informe o e-mail cadastrado."
                    };
                }

                var dados = new
                {
                    Email = email.Trim()
                };

                string json = JsonConvert.SerializeObject(dados);

                var conteudo = new StringContent(
                    json,
                    Encoding.UTF8,
                    "application/json"
                );

                // O Kinect apenas solicita; quem gera e registra o token continua sendo o MVC.
                var resposta = await _httpClient.PostAsync(
                    KinectConfig.UrlSolicitarTokenMvc,
                    conteudo
                );

                string retornoJson = await resposta.Content.ReadAsStringAsync();

                var resultado = JsonConvert.DeserializeObject<TokenSolicitadoResultado>(
                    retornoJson
                );

                if (resultado != null)
                    return resultado;

                return new TokenSolicitadoResultado
                {
                    Sucesso = false,
                    Mensagem = resposta.IsSuccessStatusCode
                        ? "Resposta invalida do MVC."
                        : "Nao foi possivel solicitar o token no MVC."
                };
            }
            catch (HttpRequestException ex)
            {
                LoggerService.Erro("Erro de conexao ao solicitar token no MVC: " + ex.Message);

                return new TokenSolicitadoResultado
                {
                    Sucesso = false,
                    Mensagem = "Nao foi possivel conectar ao MVC em " + KinectConfig.UrlSolicitarTokenMvc
                };
            }
            catch (System.Exception ex)
            {
                LoggerService.Erro("Erro ao solicitar token no MVC: " + ex.Message);

                return new TokenSolicitadoResultado
                {
                    Sucesso = false,
                    Mensagem = "Nao foi possivel solicitar o token no MVC: " + ex.Message
                };
            }
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
            catch (HttpRequestException ex)
            {
                LoggerService.Erro("Erro de conexao ao validar token no MVC: " + ex.Message);

                return new ValidacaoTokenResultado
                {
                    TokenValido = false,
                    EmailValidado = false,
                    Mensagem = "Nao foi possivel conectar ao MVC em " + KinectConfig.UrlValidarTokenMvc
                };
            }
            catch (TaskCanceledException ex)
            {
                LoggerService.Erro("Tempo esgotado ao validar token no MVC: " + ex.Message);

                return new ValidacaoTokenResultado
                {
                    TokenValido = false,
                    EmailValidado = false,
                    Mensagem = "Tempo esgotado ao validar token no MVC."
                };
            }
            catch (System.Exception ex)
            {
                LoggerService.Erro("Erro ao validar token no MVC: " + ex.Message);

                return new ValidacaoTokenResultado
                {
                    TokenValido = false,
                    EmailValidado = false,
                    Mensagem = "Nao foi possivel validar o token no MVC: " + ex.Message
                };
            }
        }
    }
}
