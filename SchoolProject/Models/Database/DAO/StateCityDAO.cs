using MySql.Data.MySqlClient;
using System;

namespace SchoolProject.Models.Database.DAO
{
    public class StateCityDAO
    {
        public const string TABLE_STATE_CITY = "state_city";
        public const string CODE = "code_state_city";
        public const string CITY = "city";
        public const string STATE = "state";

        private MySqlDataReader reader;
        private const int ERROR = -1;
        private const int NOT_FOUND = 0;

        public string Error_operation { get; set; }


        // Verifica se Existe um Estado e Cidade com o Codigo Informado
        public bool existsStateCity(int code)
        {

            if (code <= 0)
            {
                Error_operation = "Codigo do Estado e Cidade Invalido. O Codigo tem que " +
                    "ser um valor Positivo e Diferente de 0. ";
                return false;
            }

            string count_formatted, command;
            try
            {
                count_formatted = string.Format("COUNT({0})", CODE);
                command = string.Format("SELECT {0} FROM {1} WHERE {2}={3}",
                        count_formatted, TABLE_STATE_CITY, CODE, code);
            }
            catch (ArgumentNullException ex)
            {
                Error_operation = "Erro: Argumento Nulo na Criação da Query";
                System.Diagnostics.Debug.WriteLine(Error_operation + " Exceção: " + ex);
                return false;
            }
            catch (FormatException ex)
            {
                Error_operation = "Erro: Formação da String SQL Invalida";
                System.Diagnostics.Debug.WriteLine(Error_operation + " Exceção: " + ex);
                return false;
            }

            using (Database database = new Database())
            {
                if (!database.IsAvalibleDatabase)
                {
                    Error_operation = database.Error_operation;
                    return false;
                }

                reader = database.ReaderTable(command);
                if (reader == null)
                {
                    Error_operation = "Não foi possivel Ler os dados do Banco de Dados. ";
                    return false;
                }

                try
                {
                    int quantity = NOT_FOUND;

                    reader.Read();
                    quantity = reader.GetInt32(reader.GetOrdinal(count_formatted));
                    return quantity == 1;
                }
                catch (Exception ex)
                {
                    Error_operation = "Não foi possivel Verificar o Estado e Cidade.";
                    System.Diagnostics.Debug.WriteLine(Error_operation + " Exceção: " + ex);
                    return false;
                }
                finally
                {
                    if (reader != null) reader.Close();
                }
            }
        }

        // Insere um Estado/Cidade se não Existir
        public bool insertStateCity(StateCity stateCity)
        {
            if (existsStateCity(returnCodeStateCity(stateCity)))
            {
                Error_operation += "Estado e Cidade já Cadastrado no Banco de Dados. ";
                return false;
            }

            string command;
            try
            {
                command = string.Format("INSERT INTO {0}({1},{2}) VALUE" +
                "('{3}','{4}')", TABLE_STATE_CITY, CITY, STATE, stateCity.Cidade,
                stateCity.Estado);
            }
            catch (ArgumentNullException ex)
            {
                Error_operation = "Erro: Argumento Nulo na Criação da Query";
                System.Diagnostics.Debug.WriteLine(Error_operation + " Exceção: " + ex);
                return false;
            }
            catch (FormatException ex)
            {
                Error_operation = "Erro: Formação da String SQL Invalida";
                System.Diagnostics.Debug.WriteLine(Error_operation + " Exceção: " + ex);
                return false;
            }

            using (Database database = new Database())
            {
                if (!database.IsAvalibleDatabase)
                {
                    Error_operation = database.Error_operation;
                    return false;
                }
                else if (database.ExecuteCommand(command) <= 0)
                {
                    Error_operation = database.Error_operation;
                    return false;
                }
                else return true;
            }
        }

        // Exclui um Estado e Cidade se existir
        public bool deleteStateCity(int code)
        {
            if (!existsStateCity(code))
            {
                Error_operation += "Estado e Cidade não Cadastrado no Banco de Dados. ";
                return false;
            }

            string command;
            try
            {
                command = string.Format("DELETE FROM {0} WHERE {1}={2}",
                TABLE_STATE_CITY, CODE, code);
            }
            catch (ArgumentNullException ex)
            {
                Error_operation = "Erro: Argumento Nulo na Criação da Query";
                System.Diagnostics.Debug.WriteLine(Error_operation + " Exceção: " + ex);
                return false;
            }
            catch (FormatException ex)
            {
                Error_operation = "Erro: Formação da String SQL Invalida";
                System.Diagnostics.Debug.WriteLine(Error_operation + " Exceção: " + ex);
                return false;
            }

            using (Database database = new Database())
            {
                if (!database.IsAvalibleDatabase)
                {
                    Error_operation = database.Error_operation;
                    return false;
                }
                else if (database.ExecuteCommand(command) <= 0)
                {
                    Error_operation = database.Error_operation;
                    return false;
                }
                else return true;
            }
        }

        // Retorna os Dados se o Estado e Cidade existir
        public StateCity selectStateCity(int code)
        {
            if (!existsStateCity(code)) return null;

            string command;
            try
            {
                command = string.Format("SELECT * FROM {0} WHERE {1}={2}",
                TABLE_STATE_CITY, CODE, code);
            }
            catch (ArgumentNullException ex)
            {
                Error_operation = "Erro: Argumento Nulo na Criação da Query";
                System.Diagnostics.Debug.WriteLine(Error_operation + " Exceção: " + ex);
                return null;
            }
            catch (FormatException ex)
            {
                Error_operation = "Erro: Formação da String SQL Invalida";
                System.Diagnostics.Debug.WriteLine(Error_operation + " Exceção: " + ex);
                return null;
            }

            using (Database database = new Database())
            {
                if (!database.IsAvalibleDatabase)
                {
                    Error_operation = database.Error_operation;
                    return null;
                }

                reader = database.ReaderTable(command);
                if (reader == null)
                {
                    Error_operation = database.Error_operation;
                    return null;
                }

                try
                {
                    reader.Read();

                    StateCity stateCity = new StateCity();
                    stateCity.Code_statecity = reader.GetInt32(reader.GetOrdinal(CODE));
                    stateCity.Cidade = reader.GetString(reader.GetOrdinal(CITY));
                    stateCity.Estado = reader.GetString(reader.GetOrdinal(STATE));

                    return stateCity;
                }
                catch (Exception ex)
                {
                    Error_operation = "Não foi possivel Obter o Estado e Cidade.";
                    System.Diagnostics.Debug.WriteLine(Error_operation + " Exceção: " + ex);
                    return null;
                }
                finally
                {
                    if (reader != null) reader.Close();
                }
            }
        }

        // Atualiza o Estado e Cidade no Banco de Dados
        public bool updateStateCity(int code_stateCity, StateCity stateCity)
        {
            if (stateCity == null || string.IsNullOrEmpty(stateCity.Cidade)
                || string.IsNullOrEmpty(stateCity.Estado))
            {
                Error_operation = "Estado/Cidade não Informado";
                return false;
            }
            else if (!existsStateCity(code_stateCity)) return false;

            stateCity.Code_statecity = code_stateCity;

            string command;
            try
            {
                command = string.Format("UPDATE {0} SET {1}='{2}',{3}='{4}'" +
                        " WHERE {5}={6}", TABLE_STATE_CITY, CITY, stateCity.Cidade,
                        STATE, stateCity.Estado, CODE, stateCity.Code_statecity);
            }
            catch (ArgumentNullException ex)
            {
                Error_operation = "Erro: Argumento Nulo na Criação da Query";
                System.Diagnostics.Debug.WriteLine(Error_operation + " Exceção: " + ex);
                return false;
            }
            catch (FormatException ex)
            {
                Error_operation = "Erro: Formação da String SQL Invalida";
                System.Diagnostics.Debug.WriteLine(Error_operation + " Exceção: " + ex);
                return false;
            }

            using (Database database = new Database())
            {
                if (!database.IsAvalibleDatabase)
                {
                    Error_operation = database.Error_operation;
                    return false;
                }
                else if (database.ExecuteCommand(command) <= 0)
                {
                    Error_operation = database.Error_operation;
                    return false;
                }
                else return true;
            }
        }

        // Obtem o Codigo a partir do Estado e Cidade
        public int returnCodeStateCity(StateCity stateCity)
        {
            if (stateCity == null || stateCity.Cidade.Length < 5
                || stateCity.Estado.Length != 2)
            {
                Error_operation = "Estado e/ou Cidade Invalido. A Cidade e Estado são " +
                    "valores Obrigatorios. ";
                return ERROR;
            }

            string command;
            try
            {
                command = string.Format("SELECT {0} FROM {1} WHERE {2}='{3}' " +
                    "AND {4}='{5}'", CODE, TABLE_STATE_CITY, CITY,
                    stateCity.Cidade, STATE, stateCity.Estado);
            }
            catch (ArgumentNullException ex)
            {
                Error_operation = "Erro: Argumento Nulo na Criação da Query";
                System.Diagnostics.Debug.WriteLine(Error_operation + " Exceção: " + ex);
                return ERROR;
            }
            catch (FormatException ex)
            {
                Error_operation = "Erro: Formação da String SQL Invalida";
                System.Diagnostics.Debug.WriteLine(Error_operation + " Exceção: " + ex);
                return ERROR;
            }

            using (Database database = new Database())
            {
                if (!database.IsAvalibleDatabase)
                {
                    Error_operation = database.Error_operation;
                    return ERROR;
                }

                reader = database.ReaderTable(command);
                if (reader == null)
                {
                    Error_operation = database.Error_operation;
                    return NOT_FOUND;
                }
                try
                {
                    int code = NOT_FOUND;

                    reader.Read();
                    code = reader.GetInt32(reader.GetOrdinal(CODE));

                    // Retorna o Codigo do Banco de Dados ou 0 (Não encontrado)
                    return code;
                }
                catch (Exception ex)
                {
                    Error_operation = "Não foi possivel obter o Codigo do Estado e Cidade.";
                    System.Diagnostics.Debug.WriteLine(Error_operation + " Exceção: " + ex);
                    return ERROR;
                }
                finally
                {
                    if (reader != null) reader.Close();
                }
            }
        }

        // Obtem o Codigo do EstadoCidade. Caso não exista, tenta Inserir
        public int codeStateCityValid(StateCity stateCity)
        {
            if (stateCity == null)
            {
                Error_operation = " Estado e/ou Cidade não Informado";
                return ERROR;
            }

            // Obtem o Codigo State_city
            int code_state_city = returnCodeStateCity(stateCity);

            if (code_state_city == ERROR)
            {
                return ERROR;
            } 
            else if (code_state_city == NOT_FOUND)
            {
                // Dados não encontrados ---> Insere 
                if (!insertStateCity(stateCity)) return ERROR;

                // Validação = Busca no Banco de Dados os Dados Inseridos (Estado e Cidade)
                code_state_city = returnCodeStateCity(stateCity);
            }

            // Retorna um Erro ou o Codigo Valido
            return code_state_city <= 0 ? ERROR : code_state_city;
        }

        // Verifica se o Usuario é o Unico com aquela Cidade e Estado
        public bool isOnlyStateCity(StateCity stateCity)
        {

            if (stateCity == null)
            {
                Error_operation = "Cidade e Estado Invalido. Confira os Dados Inseridos ";
                return false;
            }

            StateCityDAO stateCityDAO = new StateCityDAO();


            stateCity.Code_statecity = stateCityDAO.returnCodeStateCity(stateCity);
            if (stateCity.Code_statecity <= 0)
            {
                Error_operation = "Codigo de Cidade e Estado não Encontrado. ";
                return false;
            }

            string command, count_formatted;
            try
            {
                count_formatted = string.Format("COUNT({0})", ClientDAO.STATE_CITY);
                command = string.Format("SELECT {0} FROM {1} WHERE {2}={2}",
                   count_formatted, ClientDAO.TABLE_CLIENT, ClientDAO.STATE_CITY,
                   stateCity.Code_statecity);
            }
            catch (ArgumentNullException ex)
            {
                Error_operation = "Erro: Argumento Nulo na Criação da Query";
                System.Diagnostics.Debug.WriteLine(Error_operation + " Exceção: " + ex);
                return false;
            }
            catch (FormatException ex)
            {
                Error_operation = "Erro: Formação da String SQL Invalida";
                System.Diagnostics.Debug.WriteLine(Error_operation + " Exceção: " + ex);
                return false;
            }

            using (Database database = new Database())
            {
                if (!database.IsAvalibleDatabase)
                {
                    Error_operation = database.Error_operation;
                    return false;
                }

                reader = database.ReaderTable(command);
                if (reader == null)
                {
                    Error_operation = database.Error_operation;
                    return false;
                }

                try
                {
                    int quantity_register = NOT_FOUND;

                    reader.Read();
                    quantity_register = reader.GetInt32(reader.GetOrdinal(count_formatted));

                    // Retorna True (Se for o Unico
                    return quantity_register == 1;
                }
                catch (Exception ex)
                {
                    Error_operation = "Não foi possivel Consultar o Estado e Cidade.";
                    System.Diagnostics.Debug.WriteLine(Error_operation + " Exceção: " + ex);
                    return false;
                }
                finally
                {
                    if (reader != null) reader.Close();
                }
            }
        }


    }
}
