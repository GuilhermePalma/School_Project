using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Data;

namespace SchoolProject.Models.Database
{
    // ADO = Active Data Objects - Objeto de Dados Ativos
    class Database : IDisposable
    {
        private readonly MySqlConnection mysqlConnection;
        private MySqlDataReader dataReader;

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
                System.Diagnostics.Debug.WriteLine("Não foi Possivel iniciar o " +
                    "Banco de Dados. Exception: \n" + ex);
            }

        }

        public int runCommand(string querry)
        {
            MySqlCommand command = new MySqlCommand
            {
                CommandText = querry,
                CommandType = CommandType.Text,
                Connection = mysqlConnection
            };
            try
            {
                return command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Não foi Possivel Executar o " +
                    "Comando. Exception: \n" + ex);
                return 0;
            }
        }

        public MySqlDataReader readerTable(string querry)
        {   
            try
            {
                MySqlCommand command = new MySqlCommand
                {
                    CommandText = querry,
                    CommandType = CommandType.Text,
                    Connection = mysqlConnection
                };

                dataReader = command.ExecuteReader();
                return dataReader;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Não foi Possivel Ler a Tabela. " +
                    "Exception: \n" + ex);
                return null;
            }
        }

        public void Dispose()
        {
            if (mysqlConnection.State == ConnectionState.Open) mysqlConnection.Close();
            if (dataReader != null) dataReader.Close();
        }
    }
}
