using System;
using System.Data.Entity;
using System.Data.SQLite;
using System.Data.SQLite.EF6;
using System.IO;
using TCC_Inventory_Masters_Kinect.Model;

namespace TCC_Inventory_Masters_Kinect.Data
{
    [DbConfigurationType(typeof(SQLiteConfigurationInternal))]
    public class AppDbContext : DbContext
    {
        public AppDbContext()
            : base(CreateSQLiteConnection(), true)
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<AppDbContext>());
            CriarTabelaManual();
        }

        private static SQLiteConnection CreateSQLiteConnection()
        {
            string dbPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "inventorymasters.db"
            );

            return new SQLiteConnection($"Data Source={dbPath};Version=3;");
        }

        public DbSet<MedicaoVolume> MedicaoVolumes { get; set; }
        public DbSet<EspacoMapeado> EspacosMapeados { get; set; }
        public DbSet<Point3DData> Points3D { get; set; }
        public DbSet<HistoricoOcupacao> HistoricosOcupacao { get; set; }
        public DbSet<SnapshotEspacial> SnapshotsEspaciais { get; set; }
        public DbSet<Log> Logs { get; set; }

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

                    CREATE TABLE IF NOT EXISTS EspacosMapeados (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        NomeEspaco TEXT NOT NULL,
                        VolumeMaximoCm3 REAL NOT NULL,
                        PercentualAlerta REAL NOT NULL,
                        DataCriacao TEXT NOT NULL
                    );

                    CREATE TABLE IF NOT EXISTS Point3DData (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        EspacoMapeadoId INTEGER NOT NULL,
                        PosicaoX REAL NOT NULL,
                        PosicaoY REAL NOT NULL,
                        PosicaoZ REAL NOT NULL
                    );

                    CREATE TABLE IF NOT EXISTS HistoricosOcupacao (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        EspacoMapeadoId INTEGER NOT NULL,
                        VolumeAtualCm3 REAL NOT NULL,
                        PercentualOcupacao REAL NOT NULL,
                        DataHora TEXT NOT NULL
                    );

                    CREATE TABLE IF NOT EXISTS SnapshotsEspaciais (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        EspacoMapeadoId INTEGER NOT NULL,
                        NomeSnapshot TEXT,
                        CaminhoArquivo TEXT,
                        DataCaptura TEXT NOT NULL
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
            }
        }
    }

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