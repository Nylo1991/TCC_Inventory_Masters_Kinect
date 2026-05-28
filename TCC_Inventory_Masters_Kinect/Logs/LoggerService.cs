using System;
using System.Diagnostics;
using System.IO;

namespace TCC_Inventory_Masters_Kinect.Logs
{
    public static class LoggerService
    {
        private static readonly object _lock = new object();

        private static readonly string _logDirectory =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, 
                "Logs");

        private static readonly string _logFile =
            Path.Combine(_logDirectory, 
                "kinect_log.txt");

        public static void Info(string mensagem)
        {
            Escrever("INFO", mensagem);
        }

        public static void Erro(string mensagem, Exception ex = null)
        {
            string detalhe = ex == null
                ? mensagem
                : mensagem + " | Erro: " + ex.Message;

            if (ex != null && ex.InnerException != null)
            {
                detalhe += " | Detalhes: " + ex.InnerException.Message;
            }

            Escrever("ERRO", detalhe);
        }

        // ==========================================
        // PADRÃO LogInformation
        // ==========================================

        public static void LogInformation(string mensagem)
        {
            Info(mensagem);
        }

        public static void LogWarning(string mensagem)
        {
            Escrever("AVISO", mensagem);
        }

        public static void LogError(string mensagem, Exception ex = null)
        {
            Erro(mensagem, ex);
        }

        private static void Escrever(string tipo, string mensagem)
        {
            try
            {
                string linha =
                    $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{tipo}] {mensagem}";

                // Aparece na janela Output/Saída do Visual Studio
                Debug.WriteLine(linha);
                Trace.WriteLine(linha);

                // Também grava em arquivo
                lock (_lock)
                {
                    if (!Directory.Exists(_logDirectory))
                    {
                        Directory.CreateDirectory(_logDirectory);
                    }

                    File.AppendAllText(
                        _logFile,
                        linha + Environment.NewLine);
                }
            }
            catch
            {
                // Nunca deixar erro de log derrubar o sistema.
            }
        }
    }
}