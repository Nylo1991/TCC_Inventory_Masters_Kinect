using System;
using System.Data.Entity;
using System.Data.SQLite;
using System.Data.SQLite.EF6;
using System.IO;
using System.Linq;
using TCC_Inventory_Masters_Kinect.Model;

namespace TCC_Inventory_Masters_Kinect.Data
{
    /// <summary>
    /// Classe de contexto do Entity Framework para acesso ao banco SQLite.
    /// responsavel por arzamenar as medições volumétricas, históricos de ocupação, usuários de acesso e logs do sistema.
    /// gerencia a conexão e as consultas queres serão realizadas ao banco de dados local.
    /// </summary>
    [DbConfigurationType(typeof(SQLiteConfigurationInternal))]
    public class AppDbContext : DbContext
    {
        /// <summary>
        /// Responsavel por criar a conexão com o banco SQLite principal de acesso.
        /// Este banco é usado para login e cadastro local temporário.
        /// </summary>
        public AppDbContext()
            : this(null)
        {
        }

        /// <summary>
        /// Responsavel por criar a conexão com o banco SQLite de acordo com a empresa logada.
        /// Quando a empresa não é informada, utiliza o banco principal de acesso.
        /// </summary>
        /// <param name="empresa">Nome da empresa logada no sistema.</param>
        public AppDbContext(string empresa)
            : base(CreateSQLiteConnection(empresa), true)
        {
            Database.SetInitializer<AppDbContext>(null);
            CriarTabelaManual(empresa);
        }

        /// <summary>
        /// Cria a conexão com o banco SQLite utilizando o nome da empresa quando informado.
        /// Se a empresa estiver vazia, cria ou utiliza o banco inventorymasters_acesso.db.
        /// </summary>
        /// <param name="empresa">Nome da empresa usada para criar o banco específico.</param>
        /// <returns>Uma conexão SQLite configurada para o banco de dados local.</returns>
        private static SQLiteConnection CreateSQLiteConnection(string empresa)
        {
            string nomeBanco = string.IsNullOrWhiteSpace(empresa)
                ? "inventorymasters_acesso.db"
                : $"inventorymasters_{NormalizarNomeBanco(empresa)}.db";

            string dbPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                nomeBanco
            );

            return new SQLiteConnection($"Data Source={dbPath};Version=3;");
        }

        /// <summary>
        /// Normaliza o nome da empresa para ser usado como nome de arquivo do banco SQLite.
        /// Remove caracteres especiais e substitui espaços por underline.
        /// </summary>
        /// <param name="empresa">Nome original da empresa.</param>
        /// <returns>Nome seguro para arquivo de banco de dados.</returns>
        private static string NormalizarNomeBanco(string empresa)
        {
            empresa = empresa.Trim().ToLower();

            char[] caracteres = empresa
                .Select(c => char.IsLetterOrDigit(c) ? c : '_')
                .ToArray();

            string nome = new string(caracteres);

            while (nome.Contains("__"))
            {
                nome = nome.Replace("__", "_");
            }

            return nome.Trim('_');
        }

        /// <summary>
        /// Responsavel por receber as entidades do modelo e
        /// mapear para as tabelas correspondentes no banco SQLite.
        /// </summary>
        public DbSet<MedicaoVolume> MedicaoVolumes { get; set; }
        public DbSet<HistoricoOcupacao> HistoricosOcupacao { get; set; }
        public DbSet<Log> Logs { get; set; }

        /// <summary>
        /// Configura o mapeamento das entidades para as tabelas do banco SQLite.
        /// </summary>
        /// <param name="modelBuilder">O construtor de modelo usado para configurar as entidades.</param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MedicaoVolume>().ToTable("MedicaoVolumes");
            modelBuilder.Entity<HistoricoOcupacao>().ToTable("HistoricosOcupacao");
            modelBuilder.Entity<Log>().ToTable("Logs");
        }

        /// <summary>
        /// Responsavel por criar manualmente as tabelas no banco SQLite caso elas não existam.
        /// desta forma , garantimos que a estrutura do banco esteja correta mesmo
        /// sem usar migrações do Entity Framework,e o banco pode rodar em ambientes mesmo que não
        /// possuam o Entity Framework instalado ou configurado.
        /// </summary>
        /// <param name="empresa">Nome da empresa usada para criar o banco específico.</param>
        private void CriarTabelaManual(string empresa)
        {
            using (var conn = CreateSQLiteConnection(empresa))
            {
                conn.Open();

                string sql = @"
                    CREATE TABLE IF NOT EXISTS MedicaoVolumes (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        VolumeCm3 REAL NOT NULL,
                        DataHora TEXT NOT NULL,
                        KinectLigado INTEGER NOT NULL,
                        Calibrado INTEGER NOT NULL,
                        Status TEXT,
                        Empresa TEXT,
                        Usuario TEXT,
                        NomeEspaco TEXT,
                        LimiteOcupacaoPercentual REAL NOT NULL DEFAULT 0
                    );

                    CREATE TABLE IF NOT EXISTS HistoricosOcupacao (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        MedicaoVolumeId  INTEGER NOT NULL,
                        VolumeAtualCm3 REAL NOT NULL,
                        VolumeMaximoCm3 REAL NOT NULL,
                        EspacoLivreCm3 REAL NOT NULL,
                        PercentualOcupacao REAL NOT NULL,
                        LimiteUltrapassado INTEGER NOT NULL,
                        NivelOcupacao TEXT,
                        Status TEXT,
                        DataHora TEXT NOT NULL,
                        Empresa TEXT
                    );

                    CREATE TABLE IF NOT EXISTS Logs (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        DataHora TEXT NOT NULL,
                        Nivel TEXT NOT NULL,
                        Mensagem TEXT NOT NULL
                    );

                  
                ";

                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.ExecuteNonQuery();
                }


                GarantirColuna(conn, "MedicaoVolumes", "Empresa", "TEXT");
                GarantirColuna(conn, "MedicaoVolumes", "Usuario", "TEXT");
                GarantirColuna(conn, "MedicaoVolumes", "NomeEspaco", "TEXT");

                GarantirColuna(
                    conn,
                    "MedicaoVolumes",
                    "LimiteOcupacaoPercentual",
                    "REAL NOT NULL DEFAULT 0");

                // Corrige registros de bancos antigos que ainda possuem valor nulo.
                using (var atualizar = new SQLiteCommand(@"
                    UPDATE MedicaoVolumes
                    SET LimiteOcupacaoPercentual = 0
                    WHERE LimiteOcupacaoPercentual IS NULL;
                      ", conn))
                 {
                    atualizar.ExecuteNonQuery();
                 }

                GarantirColuna(
                    conn,
                    "HistoricosOcupacao",
                    "Empresa",
                    "TEXT");
            }
        }


        /// <summary>
        /// Verifica se uma coluna existe em determinada tabela.
        /// Caso não exista, adiciona a coluna manualmente para manter compatibilidade com bancos antigos.
        /// </summary>
        /// <param name="conn">Conexão SQLite aberta.</param>
        /// <param name="tabela">Nome da tabela.</param>
        /// <param name="coluna">Nome da coluna.</param>
        /// <param name="tipo">Tipo SQLite da coluna.</param>
        private void GarantirColuna(SQLiteConnection conn, string tabela, string coluna, string tipo)
        {
            bool existe = false;

            using (var cmd = new SQLiteCommand($"PRAGMA table_info({tabela});", conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    if (reader["name"].ToString() == coluna)
                    {
                        existe = true;
                        break;
                    }
                }
            }

            if (!existe)
            {
                using (var alter = new SQLiteCommand($"ALTER TABLE {tabela} ADD COLUMN {coluna} {tipo};", conn))
                {
                    alter.ExecuteNonQuery();
                }
            }
        }
    }
    /// <summary>
    /// A configuração da persistência foi abstraída pela classe SQLiteConfigurationInternal,
    /// responsável por registrar as fábricas de conexão (SQLiteFactory) e os serviços de provisão do Entity Framework. 
    /// Essa implementação isola a infraestrutura de acesso a dados da lógica de negócio, garantindo que o 
    /// mapeamento objeto-relacional seja executado com sucesso no ambiente SQLite.
    /// </summary>
    public class SQLiteConfigurationInternal : DbConfiguration
    {
        public SQLiteConfigurationInternal()
        {
            SetProviderFactory("System.Data.SQLite", SQLiteFactory.Instance);
            SetProviderFactory("System.Data.SQLite.EF6", SQLiteProviderFactory.Instance);
            SetProviderServices(
                "System.Data.SQLite",
                (System.Data.Entity.Core.Common.DbProviderServices)
                SQLiteProviderFactory.Instance.GetService(
                    typeof(System.Data.Entity.Core.Common.DbProviderServices)
                )
            );
        }
    }
}