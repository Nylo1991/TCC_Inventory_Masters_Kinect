using System;
using System.Data.Entity;
using System.Data.SQLite;
using System.Data.SQLite.EF6;
using System.IO;
using TCC_Inventory_Masters_Kinect.Model;

namespace TCC_Inventory_Masters_Kinect.Data
{
    /// <summary>
    /// Contexto principal do banco SQLite da aplicação Inventory Masters Kinect.
    /// Responsável pelo mapeamento das entidades e criação automática das tabelas locais.
    /// </summary>
    [DbConfigurationType(typeof(SQLiteConfigurationInternal))]
    public class AppDbContext : DbContext
    {
        private static readonly object _lockInicializacao = new object();
        private static bool _bancoInicializado = false;

        /// <summary>
        /// Inicializa o contexto SQLite e garante a criação das tabelas necessárias.
        /// A estrutura do banco é verificada apenas uma vez por execução da aplicação.
        /// </summary>
        public AppDbContext()
            : base(CreateSQLiteConnection(), true)
        {
            Database.SetInitializer<AppDbContext>(null);

            GarantirBancoInicializado();
        }

        /// <summary>
        /// Tabela de medições volumétricas realizadas pelo Kinect.
        /// </summary>
        public DbSet<MedicaoVolume> MedicaoVolumes { get; set; }

        /// <summary>
        /// Tabela de histórico de ocupação dos espaços monitorados.
        /// </summary>
        public DbSet<HistoricoOcupacao> HistoricosOcupacao { get; set; }

        /// <summary>
        /// Tabela de usuários com acesso ao sistema local.
        /// </summary>
        public DbSet<UsuarioAcesso> UsuariosAcesso { get; set; }

        /// <summary>
        /// Tabela de logs da aplicação.
        /// </summary>
        public DbSet<Log> Logs { get; set; }

        /// <summary>
        /// Cria a conexão SQLite apontando para o arquivo local inventorymasters.db.
        /// </summary>
        private static SQLiteConnection CreateSQLiteConnection()
        {
            string dbPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "inventorymasters.db"
            );

            return new SQLiteConnection($"Data Source={dbPath};Version=3;");
        }

        /// <summary>
        /// Mapeia as entidades para suas respectivas tabelas no SQLite.
        /// </summary>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MedicaoVolume>().ToTable("MedicaoVolumes");
            modelBuilder.Entity<HistoricoOcupacao>().ToTable("HistoricosOcupacao");
            modelBuilder.Entity<UsuarioAcesso>().ToTable("UsuariosAcesso");
            modelBuilder.Entity<Log>().ToTable("Logs");
        }

        /// <summary>
        /// Garante que o banco seja inicializado apenas uma vez durante a execução da aplicação.
        /// Evita múltiplas verificações desnecessárias ao criar vários AppDbContext.
        /// </summary>
        private static void GarantirBancoInicializado()
        {
            if (_bancoInicializado)
            {
                return;
            }

            lock (_lockInicializacao)
            {
                if (_bancoInicializado)
                {
                    return;
                }

                CriarTabelaManual();

                _bancoInicializado = true;
            }
        }

        /// <summary>
        /// Cria manualmente as tabelas necessárias caso ainda não existam.
        /// Também executa validações simples de colunas para compatibilidade com versões anteriores.
        /// </summary>
        private static void CriarTabelaManual()
        {
            using (var conn = CreateSQLiteConnection())
            {
                conn.Open();

                string sql = @"
                    CREATE TABLE IF NOT EXISTS MedicaoVolumes (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        VolumeCm3 REAL NOT NULL,
                        DataHora TEXT NOT NULL,
                        KinectLigado INTEGER NOT NULL,
                        Calibrado INTEGER NOT NULL,
                        Status TEXT
                    );

                    CREATE TABLE IF NOT EXISTS HistoricosOcupacao (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        EspacoMapeadoId INTEGER NOT NULL,
                        VolumeAtualCm3 REAL NOT NULL,
                        VolumeMaximoCm3 REAL NOT NULL,
                        EspacoLivreCm3 REAL NOT NULL,
                        PercentualOcupacao REAL NOT NULL,
                        LimiteUltrapassado INTEGER NOT NULL,
                        NivelOcupacao TEXT,
                        Status TEXT,
                        DataHora TEXT NOT NULL
                    );

                    CREATE TABLE IF NOT EXISTS Logs (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        DataHora TEXT NOT NULL,
                        Nivel TEXT NOT NULL,
                        Mensagem TEXT NOT NULL
                    );

                    CREATE TABLE IF NOT EXISTS UsuariosAcesso (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Usuario TEXT NOT NULL,
                        Email TEXT NOT NULL,
                        Senha TEXT NOT NULL,
                        Perfil TEXT NOT NULL,
                        CriadoEm TEXT NOT NULL,
                        Ativo INTEGER NOT NULL
                    );
                ";

                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                GarantirColuna(conn, "UsuariosAcesso", "Usuario", "TEXT");
                GarantirColuna(conn, "UsuariosAcesso", "Email", "TEXT");
                GarantirColuna(conn, "UsuariosAcesso", "Senha", "TEXT");
                GarantirColuna(conn, "UsuariosAcesso", "Perfil", "TEXT");
                GarantirColuna(conn, "UsuariosAcesso", "CriadoEm", "TEXT");
                GarantirColuna(conn, "UsuariosAcesso", "Ativo", "INTEGER");

                GarantirColuna(conn, "HistoricosOcupacao", "VolumeMaximoCm3", "REAL");
                GarantirColuna(conn, "HistoricosOcupacao", "EspacoLivreCm3", "REAL");
                GarantirColuna(conn, "HistoricosOcupacao", "LimiteUltrapassado", "INTEGER");
                GarantirColuna(conn, "HistoricosOcupacao", "NivelOcupacao", "TEXT");
                GarantirColuna(conn, "HistoricosOcupacao", "Status", "TEXT");
            }
        }

        /// <summary>
        /// Garante que uma coluna exista em determinada tabela.
        /// Caso a coluna não exista, ela é adicionada por meio de ALTER TABLE.
        /// </summary>
        private static void GarantirColuna(SQLiteConnection conn, string tabela, string coluna, string tipo)
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
    /// Configuração interna do provider SQLite para uso com Entity Framework 6.
    /// </summary>
    public class SQLiteConfigurationInternal : DbConfiguration
    {
        /// <summary>
        /// Registra os providers necessários para o funcionamento do SQLite com EF6.
        /// </summary>
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