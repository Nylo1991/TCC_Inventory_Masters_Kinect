using Microsoft.Data.Sqlite;
using System;
using System.IO;

namespace InventoryMaster.Data;

public static class DataBase
{
    private static readonly string caminhoBanco;

    static DataBase()
    {
        var pasta = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "InventoryMaster"
        );

        if (!Directory.Exists(pasta))
            Directory.CreateDirectory(pasta);

        caminhoBanco = Path.Combine(pasta, "inventorymaster.db");

        if (!File.Exists(caminhoBanco))
            CriarBanco();
    }

    public static SqliteConnection GetConnection()
    {
        return new SqliteConnection($"Data Source={caminhoBanco}");
    }

    private static void CriarBanco()
    {
        using var conn = GetConnection();
        conn.Open();

        var cmd = conn.CreateCommand();
        cmd.CommandText = @"

        CREATE TABLE IF NOT EXISTS Usuario (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Nome TEXT NOT NULL,
            Email TEXT UNIQUE NOT NULL,
            Senha TEXT NOT NULL,
            Perfil TEXT,
            Data_Cadastro TEXT DEFAULT CURRENT_TIMESTAMP,
            Ativo INTEGER DEFAULT 1
        );

        CREATE TABLE IF NOT EXISTS MedicaoVolume (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Data_Hora TEXT DEFAULT CURRENT_TIMESTAMP,
            Volume_Medido REAL,
            Origem_Leitura TEXT,
            fk_Usuario_Id INTEGER,
            FOREIGN KEY (fk_Usuario_Id) REFERENCES Usuario(Id)
        );

        CREATE TABLE IF NOT EXISTS ParametrosSistema (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Volume_Maximo REAL,
            Volume_Minimo REAL,
            Email_Notificacao_Ativo INTEGER DEFAULT 0,
            Data_Atualizacao TEXT
        );

        CREATE TABLE IF NOT EXISTS Parceiro (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Nome TEXT NOT NULL,
            Empresa TEXT,
            Email TEXT UNIQUE,
            Telefone TEXT,
            Endereco TEXT,
            Data_Cadastro TEXT DEFAULT CURRENT_TIMESTAMP,
            Ativo INTEGER DEFAULT 1
        );

        CREATE TABLE IF NOT EXISTS Notificacao (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Quantidade_Destinatario INTEGER,
            Mensagem TEXT,
            Status_Envio TEXT,
            Volume_Momento REAL,
            Data_Envio TEXT DEFAULT CURRENT_TIMESTAMP,
            fk_Usuario_Id INTEGER,
            FOREIGN KEY (fk_Usuario_Id) REFERENCES Usuario(Id)
        );

        CREATE TABLE IF NOT EXISTS NotificacaoParceiro (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            fk_Parceiro_Id INTEGER,
            fk_Notificacao_Id INTEGER,
            Status_Entrega TEXT,
            FOREIGN KEY (fk_Parceiro_Id) REFERENCES Parceiro(Id),
            FOREIGN KEY (fk_Notificacao_Id) REFERENCES Notificacao(Id)
        );

        ";

        cmd.ExecuteNonQuery();
    }
}