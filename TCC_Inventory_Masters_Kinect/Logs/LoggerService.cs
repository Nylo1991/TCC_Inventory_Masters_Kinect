using System;
using System.Diagnostics;
using TCC_Inventory_Masters_Kinect.Data;
using TCC_Inventory_Masters_Kinect.Model;

namespace TCC_Inventory_Masters_Kinect.Logs
{
    public static class LoggerService
    {
        private static readonly object _lock = new object();

        private static readonly Action<string> _logger = linha =>
        {
            Debug.WriteLine(linha);
            Trace.WriteLine(linha);
        };

        public static void Info(string mensagem)
        {
            SalvarLog("INFO", mensagem);
        }

        public static void Erro(string mensagem)
        {
            SalvarLog("ERRO", mensagem);
        }

        public static void LogInformation(string mensagem)
        {
            Info(mensagem);
        }

        public static void LogWarning(string mensagem)
        {
            SalvarLog("AVISO", mensagem);
        }

        public static void LogError(string mensagem)
        {
            Erro(mensagem);
        }

        private static void SalvarLog(string nivel, string mensagem)
        {
            try
            {
                string linha = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{nivel}] {mensagem}";

                _logger(linha);

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
                string linhaErro = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [ERRO] Falha ao salvar log";
                _logger(linhaErro);
            }
        }
    }
}