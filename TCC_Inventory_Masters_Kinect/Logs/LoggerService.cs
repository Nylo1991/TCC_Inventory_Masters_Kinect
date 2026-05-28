using System;
using System.IO;
using TCC_Inventory_Masters_Kinect.Logs;

namespace TCC_Inventory_Masters_Kinect.Logs
{
    public static class LoggerService
    {
        private static readonly object _lock = new object();
        public static void Info(string mensagem)
        {
            EscreverLog("INFO", mensagem);
        }

        public static void Erro(string mensagem, Exception ex = null)
        {
            string texto = mensagem;
            if (texto != null)
            {
                texto += " | Detalhes: " + ex.InnerException.Message;
            }
            EscreverLog("ERRO", texto);
        }

        private static void EscreverLog(string tipo, string mensagem)
        {
            try
            {
                string pastaLogs = Path.Combine(
                   AppDomain.CurrentDomain.BaseDirectory, "Logs");

                if (!Directory.Exists(pastaLogs))
                {
                    Directory.CreateDirectory(pastaLogs);
                }

                string arquivoLog = Path.Combine(
                    pastaLogs, $"log_" +
                    $"{DateTime.Now:yyyyMMdd}.txt");

                string linha = 
                    $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] " +
                    $"[{tipo}] {mensagem}";

                lock (_lock)
                {
                   File.AppendAllText(
                       arquivoLog, 
                       linha + Environment.NewLine);
                }
            }
            catch
            {
                // Não lança erro para não quebrar o sistema caso o log falhe.
            }
        }

     }
}
