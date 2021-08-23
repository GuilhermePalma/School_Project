using MySql.Data.MySqlClient;
using SchoolProject.Models;
using System;

namespace Database.DAO
{
    public class StateCityDAO
    {
        private Database database;
        private MySqlDataReader reader;
        private string command;
        private string error_operation = "";

        private const string CODE = "code_state_city";
        private const string CITY = "city";
        private const string STATE = "state";

        private const int ERROR = -1;
        private const int NOT_FOUND = 0;


        // Verifica se Existe um Estado e Cidade com o Codigo Informado
        public bool existsStateCity(int code)
        {

            if(code <= 0)
            {
                error_operation = "Codigo do Estado e Cidade Invalido. O Codigo tem que " +
                    "ser um valor Positivo e Diferente de 0. ";
                return false;
            }

            try
            {
                database = new Database();

                command = String.Format("SELECT COUNT({0}) FROM state_city WHERE {0}={1}",
                    CODE, code);

                reader = database.readerTable(command);

                if (reader == null)
                {
                    error_operation = "Não foi possivel Ler os dados do Banco de Dados. ";
                    return false;
                }

                if (reader.HasRows)
                {
                    int quantity = NOT_FOUND;

                    while (reader.Read())
                    {
                        quantity = reader.GetInt32(reader.GetOrdinal("COUNT(code_state_city)"));
                    }

                    return quantity == 1 ? true : false;
                }
                else
                {
                    error_operation = "Dados não Encontrados no Banco de Dados. ";
                    return false;
                }
            }
            catch (Exception ex)
            {
                error_operation = "Não foi Possivel verificar no Banco de dados. Exceção:" + ex;
                Console.WriteLine(error_operation);
                return false;
            }
            finally
            {
                if(reader != null) reader.Close();
            }
        }

        // Insere um Estado/Cidade se não Existir
        public bool insertStateCity(StateCity stateCity)
        {
            try
            {                   
                if (existsStateCity(returnCodeStateCity(stateCity)))
                {
                    error_operation += "Estado e Cidade já Cadastrado no Banco de Dados. ";
                    return false;
                }

                command = String.Format("INSERT INTO state_city({0},{1}) VALUE" +
                    "('{2}','{3}')", CITY, STATE, stateCity.Cidade, stateCity.Estado);


                database = new Database();

                if (database.runCommand(command) == 0)
                {
                    error_operation = "Não foi possivel Cadastrar no Banco de Dados. ";
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

        // Exclui um Estado e Cidade se existir
        public bool deleteStateCity(int code)
        {
            try
            {
                if (!existsStateCity(code))
                {
                    error_operation += "Estado e Cidade não Cadastrado no Banco de Dados. ";
                    return false;
                }

                command = String.Format("DELETE FROM state_city WHERE {0}={1}",
                    CODE, code);

                database = new Database();

                if (database.runCommand(command) == 0)
                {
                    error_operation = "Não foi possivel Excluir do Banco de Dados. ";
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

        // Retorna os Dados se o Estado e Cidade existir
        public StateCity selectStateCity(int code)
        {
            try
            {
                if (!existsStateCity(code))
                {
                    error_operation += "Estado e Cidade não Cadastrado no Banco de Dados. ";
                    return null;
                }

                database = new Database();

                command = String.Format("SELECT * FROM state_city WHERE {0}={1}", CODE, code);

                reader = database.readerTable(command);

                if (reader == null)
                {
                    error_operation = "Não foi Possivel Acessar os Dados do Banco de Dados. ";
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
                else
                {
                    error_operation += "Estado e Cidade não Encontrados no Banco de Dados. ";
                    return null;
                }
            }
            catch (Exception ex)
            {
                error_operation = "Não foi Excluir o Usuario no Banco de dados. Exceção:" + ex;
                Console.WriteLine(error_operation);
                return null;
            }
            finally
            {
                if (reader != null) reader.Close();
            }
        }

        // Por meio do Estado e Cidade Informado, obtem o codigo
        public int returnCodeStateCity(StateCity stateCity)
        {
            if (stateCity == null || stateCity.Cidade.Length < 5 || stateCity.Estado.Length != 2)
            {
                error_operation = "Estado e/ou Cidade Invalido. A Cidade e Estado são " +
                    "valores Obrigatorios. ";
                return ERROR;
            }

            try
            {
                database = new Database();

                command = String.Format("SELECT {0} FROM state_city WHERE " +
                    "{1}='{2}' AND {3}='{4}'", CODE, CITY, stateCity.Cidade,
                    STATE, stateCity.Estado);

                reader = database.readerTable(command);

                if (reader == null)
                {
                    error_operation = "Não foi Possivel Acessar os Dados do Banco de Dados. ";
                    return ERROR;
                }

                if (reader.HasRows)
                {
                    int code = NOT_FOUND;

                    while (reader.Read())
                    {
                        code = reader.GetInt32(reader.GetOrdinal(CODE));
                    }
                    // Retorna o Codigo do Banco de Dados ou 0 (Não encontrado)
                    return code;
                }
                else
                {
                    error_operation = "Dados não Encontrados no Banco de Dados. ";
                    return NOT_FOUND;
                }
            }
            catch (Exception ex)
            {
                error_operation = "Não foi Possivel Encontrar o Codigo no Banco de dados. Exceção:" + ex;
                Console.WriteLine(error_operation);
                return ERROR;
            }
            finally
            {
                if (reader != null) reader.Close();
            }
        }

    }
}
