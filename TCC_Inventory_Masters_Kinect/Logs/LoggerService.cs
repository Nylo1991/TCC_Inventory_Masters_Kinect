using System;
using System.Diagnostics;
using TCC_Inventory_Masters_Kinect.Data;
using TCC_Inventory_Masters_Kinect.Model;

namespace TCC_Inventory_Masters_Kinect.Logs
{
    /// <summary>
    /// Serviço centralizado de logs da aplicação.
    /// Registra mensagens no Debug, Trace e na tabela Logs do SQLite.
    /// </summary>
    public static class LoggerService
    {

        /// <summary>
        /// O lock é necessário para garantir que múltiplas threads não tentem acessar o banco de dados ao mesmo tempo,
        /// evitando conflitos e garantindo a integridade dos logs.
        /// </summary>
        private static readonly object _lock = new object();

        private static readonly Action<string> _logger = linha =>
        {
            Debug.WriteLine(linha);
            Trace.WriteLine(linha);
        };

        /// <summary>
        /// Registra uma mensagem informativa.
        /// </summary>
        public static void Info(string mensagem)
        {
            SalvarLog("INFO", mensagem);
        }

        /// <summary>
        /// Registra uma mensagem de erro.
        /// O parâmetro Exception é opcional para permitir chamadas simples ou com exceção.
        /// </summary>
        public static void Erro(string mensagem, Exception ex = null)
        {
            string detalhe = mensagem;

            if (ex != null)
            {
                detalhe += $" | Erro: {ex.Message}";

                if (ex.InnerException != null)
                {
                    detalhe += $" | Detalhes: {ex.InnerException.Message}";
                }

                if (!string.IsNullOrWhiteSpace(ex.StackTrace))
                {
                    detalhe += $" | StackTrace: {ex.StackTrace}";
                }
            }

            SalvarLog("ERRO", detalhe);
        }

        /// <summary>
        /// Alias para Info.
        /// </summary>
        public static void LogInformation(string mensagem)
        {
            Info(mensagem);
        }

        /// <summary>
        /// Registra uma mensagem de aviso.
        /// </summary>
        public static void LogWarning(string mensagem)
        {
            SalvarLog("AVISO", mensagem);
        }

        /// <summary>
        ///  evento  que indica algo crítico que impediu uma funcionalidade de completar sua tarefa..
        /// </summary>
        public static void LogError(string mensagem, Exception ex = null)
        {
            Erro(mensagem, ex);
        }

        /// <summary>
        /// Salva a mensagem de log no console de depuração e no banco SQLite.
        /// </summary>
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