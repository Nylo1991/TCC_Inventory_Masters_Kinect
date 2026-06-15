using System;
using System.Data.Entity;
using System.Data.SQLite;
using System.Data.SQLite.EF6;
using System.IO;
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
        /// Responsavel por criar a conexão com o banco SQLite e garantir que as tabelas necessárias existam.
        /// </summary>
        public AppDbContext()
            : base(CreateSQLiteConnection(), true)
        {
            Database.SetInitializer<AppDbContext>(null);
            CriarTabelaManual();
        }
        /// <summary>
        /// Cria a conexão com o banco SQLite utilizando o caminho do arquivo
        /// "inventorymasters.db" localizado no diretório base da aplicação.
        /// </summary>
        /// <returns>Uma conexão SQLite configurada para o banco de dados local.</returns>
        private static SQLiteConnection CreateSQLiteConnection()
        {
            string dbPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "inventorymasters.db"
            );

            return new SQLiteConnection($"Data Source={dbPath};Version=3;");
        }
        /// <summary>
        /// Responsavel por receber as entidades do modelo e
        /// mapear para as tabelas correspondentes no banco SQLite.
        /// </summary>
        public DbSet<MedicaoVolume> MedicaoVolumes { get; set; }
        public DbSet<HistoricoOcupacao> HistoricosOcupacao { get; set; }
        public DbSet<UsuarioAcesso> UsuariosAcesso { get; set; }
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
            modelBuilder.Entity<UsuarioAcesso>().ToTable("UsuariosAcesso");
            modelBuilder.Entity<Log>().ToTable("Logs");
        }
        /// <summary>
        /// Responsavel por criar manualmente as tabelas no banco SQLite caso elas não existam. 
        /// desta forma , garantimos que a estrutura do banco esteja correta mesmo 
        /// sem usar migrações do Entity Framework,e o banco pode rodar em ambientes mesmo que não
        /// possuam o Entity Framework instalado ou configurado.
        /// </summary>
        private void CriarTabelaManual()
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

                /// responsavel por criar as tabelas no banco SQLite caso elas não existam, 
                /// garantindo a estrutura correta do banco mesmo sem usar migrações do Entity Framework.
                /// 
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
        /// Verifica se uma coluna específica existe em uma tabela do banco SQLite e,
        /// caso contrário, adiciona a coluna com o tipo especificado.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="tabela"></param>
        /// <param name="coluna"></param>
        /// <param name="tipo"></param>
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
    /// Configuração personalizada do Entity Framework para garantir a 
    /// compatibilidade com o provedor SQLite.
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