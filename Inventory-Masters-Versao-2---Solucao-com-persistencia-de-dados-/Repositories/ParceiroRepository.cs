using InventoryMaster.Models;
using Microsoft.Data.Sqlite;

namespace InventoryMaster.Data;

public class ParceiroRepository
{
    public void Inserir(Parceiro parceiro)
    {
        using var conn = DataBase.GetConnection();
        conn.Open();

        var cmd = conn.CreateCommand();

        cmd.CommandText = @"
        INSERT INTO Parceiro
        (Nome, Empresa, Email, Telefone, Endereco, Data_Cadastro, Ativo)
        VALUES
        (@Nome, @Empresa, @Email, @Telefone, @Endereco, datetime('now'), 1)
        ";

        cmd.Parameters.AddWithValue("@Nome", parceiro.Nome);
        cmd.Parameters.AddWithValue("@Empresa", parceiro.Empresa ?? "");
        cmd.Parameters.AddWithValue("@Email", parceiro.Email);
        cmd.Parameters.AddWithValue("@Telefone", parceiro.Telefone);
        cmd.Parameters.AddWithValue("@Endereco", parceiro.Endereco ?? "");

        cmd.ExecuteNonQuery();
    }

    public List<Parceiro> ListarParceiro()
    {
        var lista = new List<Parceiro>();

        using var conn = DataBase.GetConnection();
        conn.Open();

        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT Id, Nome, Empresa, Email, Telefone, Endereco FROM Parceiro";

        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            lista.Add(new Parceiro
            {
                Id = reader.GetInt32(0),
                Nome = reader.GetString(1),
                Empresa = reader.GetString(2),
                Email = reader.GetString(3),
                Telefone = reader.GetString(4),
                Endereco = reader.GetString(5)
            });
        }

        return lista;
    }
}