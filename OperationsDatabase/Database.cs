using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpreationsDatabase
{
    public class Database : IDisposable
    {
        MySqlConnection mysqlConnection;

        public Database()
        {
            mysqlConnection = new MySqlConnection(
                ConfigurationManager.ConnectionStrings["connection"].ConnectionString);
            try
            {
                mysqlConnection.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Não foi Possivel iniciar o Banco de Dados. Exception: \n" + ex);
            }

        }

        public int runCommand(string querry)
        {
            var command = new MySqlCommand
            {
                CommandText = querry,
                CommandType = CommandType.Text,
                Connection = mysqlConnection
            };
            try
            {
                command.ExecuteNonQuery();

                return command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Não foi Possivel Executar o Comando. Exception: \n" + ex);
                return 0;
            }
        }

        public MySqlDataReader readerTable(string querry)
        {
            var command = new MySqlCommand(querry, mysqlConnection);
            try
            {
                MySqlDataReader reader = command.ExecuteReader();
                return reader;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Não foi Possivel Ler a Tabela. Exception: \n" + ex);
                return null;
            }
        }

        public void Dispose()
        {
            if (mysqlConnection.State == ConnectionState.Open) mysqlConnection.Close();

        }

    }
}
