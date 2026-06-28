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
        /// <summary>
        /// HttpClient utilizado para realizar requisições HTTP ao MVC, 
        /// configurado para não usar proxy e com timeout de 15 segundos.
        /// </summary>
        private readonly HttpClient _httpClient;

        public AutenticacaoMvcService()
        {
           
            var handler = new HttpClientHandler
            {
                UseProxy = false
            };

            _httpClient = new HttpClient(handler)
            {
                Timeout = System.TimeSpan.FromSeconds(15)
            };
        }

        /// <summary>
        /// Solicita um token de acesso ao MVC para o e-mail fornecido.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Valida o token de acesso fornecido junto ao MVC, retornando informações sobre a validade 
        /// do token e se o e-mail foi validado.
        /// </summary>
        /// <param name="token">Token de acesso a ser validado.</param>
        /// <returns>Resultado da validação do token.</returns>
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
