using System;
using System.Diagnostics;
using System.IO;
using TCC_Inventory_Masters_Kinect.Data;
using TCC_Inventory_Masters_Kinect.Model;

namespace TCC_Inventory_Masters_Kinect.Logs
{
    public static class LoggerService
    {
        private static readonly object _lock = new object();

        public static void Info(string mensagem)
        {
            SalvarLog("INFO", mensagem);
        }

        public static void Erro(string mensagem, Exception ex = null)
        {
            string detalhe = ex == null
                ? mensagem
                : $"{mensagem} | Erro: {ex.Message}";

            if (ex?.InnerException != null)
            {
                detalhe += $" | Detalhes: {ex.InnerException.Message}";
            }

            SalvarLog("ERRO", detalhe);
        }

        public static void LogInformation(string mensagem)
        {
            Info(mensagem);
        }

        public static void LogWarning(string mensagem)
        {
            SalvarLog("AVISO", mensagem);
        }

        public static void LogError(string mensagem, Exception ex = null)
        {
            Erro(mensagem, ex);
        }

        private static void SalvarLog(string nivel, string mensagem)
        {
            try
            {
                
                string linha = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{nivel}] {mensagem}";
                Debug.WriteLine(linha);
                Trace.WriteLine(linha);

                lock (_lock)
                {
                    using (var context = new AppDbContext())
                    {
                        var log = new Log
                        {
                            DataHora = DateTime.Now,
                            Nivel = nivel,
                            Mensagem = mensagem
                        };

                        context.Logs.Add(log);
                        context.SaveChanges();
                    }
                }
            }
            catch
            {
                // Nunca deixar erro de log derrubar o sistema
            }
        }
    }
}
