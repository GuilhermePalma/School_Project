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

        private const int ERROR = -1;
        public string Error_operation { get; set; }
        public bool IsAvalibleDatabase { get; set; }

        public Database()
        {
            try
            {
                mysqlConnection = new MySqlConnection(
                   ConfigurationManager.ConnectionStrings["connection"].ConnectionString);

                if(mysqlConnection == null)
                {
                    Error_operation = "Não foi criar a Conexão com o Banco de Dados";
                    IsAvalibleDatabase = false;
                }

                mysqlConnection.Open();

                if (mysqlConnection.State != ConnectionState.Open)
                {
                    Error_operation = "Não foi possivel conectar ao Banco de Dados";
                    IsAvalibleDatabase = false;
                }
                else IsAvalibleDatabase = true;
            }
            catch(ConfigurationErrorsException ex)
            {
                Error_operation = "Erro ao recuperar a String de Conexão do Banco de Dados";
                System.Diagnostics.Debug.WriteLine("Erro ao recuperar a String Connection. " +
                    "Exception: " + ex);
                IsAvalibleDatabase = false;

            }
            catch (Exception ex)
            {
                Error_operation = "Erro ao conectar com o Banco de Dados";
                System.Diagnostics.Debug.WriteLine("Erro ao conectar com o Banco de Dados. " +
                    "Exception: " + ex);
                IsAvalibleDatabase = false;
            }
        }

        public int ExecuteCommand(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                Error_operation = "Solicitação Invaldia. Comando SQL não Encontrado";
                return ERROR;
            }

            try
            {
                MySqlCommand command = new MySqlCommand
                {
                    CommandText = query,
                    CommandType = CommandType.Text,
                    Connection = mysqlConnection
                };

                if (command == null)
                {
                    System.Diagnostics.Debug.WriteLine("Erro ao Criar o Comando: "
                        + command.ToString());
                    Error_operation = "Solicitação Invalida. Formação do " +
                        "comando SQL Incorreta.";
                    return ERROR;
                }

                int row_affected = command.ExecuteNonQuery();

                if (row_affected <= 0)
                {
                    Error_operation = "Não foi Possivel Executar o Comando.";
                    System.Diagnostics.Debug.WriteLine("Erro ao Executar o Comando: "
                        + command.CommandText);
                    return ERROR;
                }
                else return row_affected;

            }
            catch (Exception ex)
            {
                Error_operation = "Houve um Erro ao Executar o Comando.";
                System.Diagnostics.Debug.WriteLine("Não foi Possivel Executar o " +
                    "Comando. Exception: \n" + ex);
                return ERROR;
            }
        }

        public MySqlDataReader ReaderTable(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                Error_operation = "Solicitação Invaldia. Comando SQL não Encontrado";
                return null;
            }

            try
            {
                MySqlCommand command = new MySqlCommand
                {
                    CommandText = query,
                    CommandType = CommandType.Text,
                    Connection = mysqlConnection
                };

                if(command == null)
                {
                    System.Diagnostics.Debug.WriteLine("Erro ao Criar o Comando: "
                        + command.ToString());
                    Error_operation = "Solicitação Invaldia. Formação do " +
                        "comando SQL Incorreta.";
                    return null;
                }

                dataReader = command.ExecuteReader();

                if (dataReader == null || !dataReader.HasRows)
                {
                    Error_operation = "Dados não encontrados no Banco de Dados";
                    System.Diagnostics.Debug.WriteLine("Erro ao Ler a Tabela: "
                        + command.CommandText);
                    return null;
                } else return dataReader;
            }
            catch (Exception ex)
            {
                Error_operation = "Não foi Possivel acessar o Banco de Dados";
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