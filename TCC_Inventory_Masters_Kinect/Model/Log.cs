using System;

namespace TCC_Inventory_Masters_Kinect.Model
{

    /// <summary>
    /// classe responsável por representar os dados de log da aplicação esse dados serão armazenados em uma tabela Logs do 
    /// SQLite, e também serão registrados no Debug e Trace para facilitar a depuração e monitoramento da aplicação.
    /// </summary>
    public class Log
    {
        public int Id { get; set; }
        public DateTime DataHora { get; set; } = DateTime.Now;
        public string Nivel { get; set; }
        public string Mensagem { get; set; }
    }
}