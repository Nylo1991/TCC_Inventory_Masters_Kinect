using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TCC_Inventory_Masters_Kinect.ConfigKinect;
using TCC_Inventory_Masters_Kinect.Logs;
using TCC_Inventory_Masters_Kinect.Model;

namespace TCC_Inventory_Masters_Kinect.Service
{
    public class ApiMedicaoService
    {
        // ==========================================
        // HTTP CLIENT
        // ==========================================

        private static readonly HttpClient _httpClient =
            new HttpClient();

        // ==========================================
        // ENVIAR MEDIÇÃO PARA API MVC
        // ==========================================

        public async Task<bool> EnviarMedicaoAsync(
            MedicaoVolume medicao)
        {
            try
            {
                string json =
                    JsonConvert.SerializeObject(
                        new
                        {
                            volumeCm3 = medicao.VolumeCm3,
                            dataHora = medicao.DataHora,
                            kinectLigado = medicao.KinectLigado,
                            calibrado = medicao.Calibrado,
                            status = medicao.Status
                        });

                using (var content =
                    new StringContent(
                        json,
                        Encoding.UTF8,
                        "application/json"))
                {
                    HttpResponseMessage response =
                        await _httpClient
                            .PostAsync(
                                KinectConfig.UrlApiMedicoes,
                                content);

                    if (response.IsSuccessStatusCode)
                    {
                        LoggerService.Info(
                            $"Medição enviada para API MVC com sucesso. Volume: {medicao.VolumeCm3:F0} cm³");

                        return true;
                    }

                    string respostaErro =
                        await response.Content
                            .ReadAsStringAsync();

                    LoggerService.Info(
                        "Falha ao enviar medição para API MVC. " +
                        "StatusCode: " + response.StatusCode +
                        " | Resposta: " + respostaErro);

                    return false;
                }
            }
            catch (Exception ex)
            {
                LoggerService.Erro(
                    "Erro ao enviar medição para API MVC.",
                    ex);

                return false;
            }
        }
    }
}