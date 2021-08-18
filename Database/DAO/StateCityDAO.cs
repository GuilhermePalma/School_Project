using MySql.Data.MySqlClient;
using OpreationsDatabase.DLLs;
using System;

namespace Database.DAO
{
    public class StateCityDAO
    {
        private Database database;
        private string command;
        private string error_operation = "";

        private const string CODE = "code_state_city";
        private const string CITY = "city";
        private const string STATE = "state";

        private const int ERROR = -1;
        private const int NOT_FOUND = 0;


        public bool existsStateCity(int code)
        {
            command = String.Format("SELECT COUNT({0}) FROM state_city WHERE {0}={1}",
                CODE, code);

            try
            {
                database = new Database();

                MySqlDataReader reader = database.readerTable(command);

                if (reader == null)
                {
                    error_operation = "Não foi possivel consultar a Tabela";
                    return false;
                }

                if (reader.HasRows)
                {
                    int quantity = 0;

                    while (reader.Read())
                    {
                        quantity = reader.GetInt32(reader.GetOrdinal("COUNT(code_state_city)"));
                    }

                    return quantity == 1 ? true : false;
                }
                else
                {
                    error_operation = "Dados não Encontrados no Banco de Dados";
                    return false;
                }
            }
            catch (Exception ex)
            {
                error_operation = "Não foi Possivel verificar no Banco de dados. Exceção:" + ex;
                Console.WriteLine(error_operation);
                return false;
            }
        }

        public bool insertStateCity(StateCity stateCity)
        {
            try
            {
                command = String.Format("INSERT INTO state_city({0},{1}) VALUE" +
                    "('{2}','{3}')", CITY, STATE, stateCity.Cidade, stateCity.Estado);

                database = new Database();

                if (database.runCommand(command) == 0)
                {
                    error_operation = "Não foi possivel Cadastrar no Banco de Dados";
                    return false;
                }
                else return true;
            }
            catch (Exception ex)
            {
                error_operation = "Não foi Cadastrar o Estado e Cidade no Banco de dados. Exceção:" + ex;
                Console.WriteLine(error_operation);
                return false;
            }
        }

        public bool deleteStateCity(int code)
        {
            try
            {
                command = String.Format("DELETE FROM state_city WHERE {0}={1}",
                    CODE, code);

                database = new Database();

                if (database.runCommand(command) == 0)
                {
                    error_operation = "Não foi possivel Excluir do Banco de Dados";
                    return false;
                }
                else return true;
            }
            catch (Exception ex)
            {
                error_operation = "Não foi Excluir o Estado e Cidade no Banco de dados. Exceção:" + ex;
                Console.WriteLine(error_operation);
                return false;
            }
        }

        public StateCity selectStateCity(int code)
        { 
            try
            {
                if (existsStateCity(code))
                {
                    database = new Database();

                    command = String.Format("SELECT * FROM state_city WHERE {0}={1}", CODE, code);

                    MySqlDataReader reader = database.readerTable(command);

                    if (reader == null)
                    {
                        error_operation = "Não foi Possivel Acessar o Banco de Dados";
                        return null;
                    }

                    if (reader.HasRows)
                    {
                        StateCity stateCity = new StateCity();

                        while (reader.Read())
                        {
                            stateCity.Code_statecity = reader.GetInt32(reader.GetOrdinal(CODE));
                            stateCity.Cidade = reader.GetString(reader.GetOrdinal(CITY));
                            stateCity.Estado = reader.GetString(reader.GetOrdinal(STATE));
                        }

                        return stateCity;
                    }
                }

                // Caso não Exista no Banco de Dados ou não Exista Linhas de Registro
                error_operation = "Dados não Encontrados no Banco de Dados";
                return null;

            }
            catch (Exception ex)
            { 
                error_operation = "Não foi Excluir o Usuario no Banco de dados. Exceção:" + ex;
                Console.WriteLine(error_operation);
                return null;
            }
        }

        public int returnCodeStateCity(StateCity stateCity)
        {
            try
            {
                database = new Database();

                command = String.Format("SELECT {0} FROM state_city WHERE " +
                    "{2}='{3}' AND {4}='{5}'", CODE, CITY, stateCity.Cidade,
                    STATE, stateCity.Estado);

                MySqlDataReader reader = database.readerTable(command);

                if (reader == null)
                {
                    error_operation = "Não foi Possivel Acessar o Banco de Dados";
                    return ERROR;
                }

                if (reader.HasRows)
                {
                    return reader.GetInt32(reader.GetOrdinal(CODE));
                }
                else
                {
                    error_operation = "Dados não Encontrados mo Banco de Dados";
                    return NOT_FOUND;
                }
            }
            catch (Exception ex)
            {
                error_operation = "Não foi Excluir o Usuario no Banco de dados. Exceção:" + ex;
                Console.WriteLine(error_operation);
                return ERROR;
            }
        }

    }
}
