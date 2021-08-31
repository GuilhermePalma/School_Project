using MySql.Data.MySqlClient;
using System;

namespace SchoolProject.Models.Database.DAO
{
    public class StateCityDAO
    {

        private MySqlDataReader reader;
        private string command;
        private string error_operation = "";

        public const string TABLE_STATE_CITY = "state_city";
        public const string CODE = "code_state_city";
        public const string CITY = "city";
        public const string STATE = "state";

        private const int ERROR = -1;
        private const int NOT_FOUND = 0;


        // Verifica se Existe um Estado e Cidade com o Codigo Informado
        public bool existsStateCity(int code)
        {

            if (code <= 0)
            {
                error_operation = "Codigo do Estado e Cidade Invalido. O Codigo tem que " +
                    "ser um valor Positivo e Diferente de 0. ";
                return false;
            }

            try
            {
                using (Database database = new Database())
                {
                    command = String.Format("SELECT COUNT({0}) FROM {1} WHERE {0}={2}",
                        CODE, TABLE_STATE_CITY, code);

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
            }
            catch (Exception ex)
            {
                error_operation = "Não foi Possivel verificar no Banco de dados. Exceção:" + ex;
                Console.WriteLine(error_operation);
                return false;
            }
            finally
            {
                if (reader != null) reader.Close();
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

                command = String.Format("INSERT INTO {0}({1},{2}) VALUE" +
                    "('{3}','{4}')", TABLE_STATE_CITY, CITY, STATE,
                    stateCity.Cidade, stateCity.Estado);

                using (Database database = new Database())
                {

                    if (database.runCommand(command) == 0)
                    {
                        error_operation = "Não foi possivel Cadastrar no Banco de Dados. ";
                        return false;
                    }
                    else return true;
                }
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

                command = String.Format("DELETE FROM {0} WHERE {1}={2}",
                    TABLE_STATE_CITY, CODE, code);

                using (Database database = new Database())
                {

                    if (database.runCommand(command) == 0)
                    {
                        error_operation = "Não foi possivel Excluir do Banco de Dados. ";
                        return false;
                    }
                    else return true;
                }
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
                using (Database database = new Database())
                {

                    command = String.Format("SELECT * FROM {0} WHERE {1}={2}",
                        TABLE_STATE_CITY, CODE, code);

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

        // Atualiza o Estado e Cidade no Banco de Dados
        public bool updateStateCity(int code_stateCity, StateCity stateCity)
        {
            if (stateCity == null || stateCity.Cidade == null ||
                stateCity.Estado == null)
            {
                error_operation = "Estado/Cidade não Informado";
                return false;
            }

            stateCity.Code_statecity = code_stateCity;

            if (!existsStateCity(stateCity.Code_statecity))
            {
                error_operation = "Estado/Cidade não Cadastrado no Sistema";
                return false;
            }

            try
            {
                using (Database database = new Database())
                {
                    command = String.Format("UPDATE {0} SET {1}='{2}',{3}='{4}'" +
                        " WHERE {5}={6}", TABLE_STATE_CITY, CITY, stateCity.Cidade,
                        STATE, stateCity.Estado, CODE, stateCity.Code_statecity);

                    if (database.runCommand(command) == 0)
                    {
                        error_operation = "Não foi Possivel Atualizar no Banco de Dados";
                        return false;
                    }
                    else return true;
                }
            }
            catch (Exception ex)
            {
                error_operation = "Não foi possivel Alterar o Estado e Cidade " +
                    "no Banco de dados. Exceção:" + ex;
                System.Diagnostics.Debug.WriteLine(error_operation);
                return false;
            }
        }

        // Obtem o Codigo a partir do Estado e Cidade
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
                using (Database database = new Database())
                {

                    command = String.Format("SELECT {0} FROM {1} WHERE " +
                    "{2}='{3}' AND {4}='{5}'", CODE, TABLE_STATE_CITY, CITY,
                    stateCity.Cidade, STATE, stateCity.Estado);

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

        // Obtem o Codigo do EstadoCidade. Caso não exista, tenta Inserir
        public int codeStateCityValid(StateCity stateCity)
        {
            if (stateCity == null)
            {
                error_operation += " Estado e/ou Cidade não Informado";
                return ERROR;
            }

            try
            {
                // Obtem o Codigo State_city
                int code_state_city = new StateCityDAO().returnCodeStateCity(stateCity);

                // Erro nos Metodos do StateCity = Reader = null ou Problema na Execução (catch)
                if (code_state_city == ERROR)
                {
                    error_operation += " Erro na Busca do Codigo da Cidade e/ou Estado";
                    return ERROR;
                }
                // Caso não exista no Banco de Dados ---> Insere
                if (code_state_city == NOT_FOUND)
                {
                    bool is_insert = new StateCityDAO().insertStateCity(stateCity);

                    if (!is_insert)
                    {
                        error_operation = "Não foi Possivel Inserir o Estado e/ou Cidadeno Banco de Dados";
                        return ERROR;
                    }
                    else
                    {
                        // Validação = Busca no Banco de Dados os Dados Inseridos (Estado e Cidade)
                        code_state_city = new StateCityDAO().returnCodeStateCity(stateCity);
                    }
                }

                if (code_state_city == ERROR || code_state_city == NOT_FOUND)
                {
                    error_operation += "Estado e/ou Cidade não Localizados no Banco de Dados";
                    return ERROR;
                }
                else
                {
                    return code_state_city;
                }
            }
            catch (Exception ex)
            {
                error_operation = "Erro ao buscar Estado e/ou Cidade no Banco de dados. Exceção:" + ex;
                Console.WriteLine(error_operation);
                return ERROR;
            }
        }

        // Verifica se o Usuario é o Unico com aquela Cidade e Estado
        public bool isOnlyStateCity(StateCity stateCity)
        {

            if (stateCity == null)
            {
                error_operation = "Cidade e Estado Invalido. Confira os Dados Inseridos ";
                return false;
            }

            StateCityDAO stateCityDAO = new StateCityDAO();

            try
            {

                stateCity.Code_statecity = stateCityDAO.returnCodeStateCity(stateCity);
                if (stateCity.Code_statecity <= 0)
                {
                    error_operation = "Codigo de Cidade e Estado não Encontrado. ";
                    return false;
                }

                using (Database database = new Database())
                {
                    // Obtem a Quantiade de vezes que o codigo é usado
                    command = String.Format("SELECT COUNT({0}) FROM {1} WHERE {0}={2}",
                   ClientDAO.STATE_CITY, ClientDAO.TABLE_CLIENT, stateCity.Code_statecity);

                    reader = database.readerTable(command);

                    if (reader == null)
                    {
                        error_operation = "Não foi possivel consultar a Tabela";
                        return false;
                    }

                    if (reader.HasRows)
                    {
                        String formated_count = String.Format("COUNT({0})", ClientDAO.STATE_CITY);
                        int quantity_register = NOT_FOUND;

                        while (reader.Read())
                        {
                            quantity_register = reader.GetInt32(reader.GetOrdinal(formated_count));
                        }

                        // Se for o Unico ---> Retorna True. Se não ---> Retorna False
                        return quantity_register == 1;
                    }
                    else
                    {
                        error_operation = "Erro na Leitura dos Dados do Banco de Dados";
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                error_operation = "Não foi possivel Consultar o Estado e Cidade no Banco de dados. Exceção:" + ex;
                System.Diagnostics.Debug.WriteLine(error_operation);
                return false;
            }
            finally
            {
                if (reader != null) reader.Close();
            }
        }


    }
}
