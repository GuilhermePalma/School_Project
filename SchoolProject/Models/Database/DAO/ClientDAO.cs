using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace SchoolProject.Models.Database.DAO
{
    public class ClientDAO
    {
        public const string TABLE_CLIENT = "client";
        public const string CPF = "cpf";
        public const string NAME = "name";
        public const string STATE_CITY = "code_state_city";
        public const string ADDRESS = "code_address";
        public const string NUMBER = "residential_number";
        public const string COMPLEMENT = "complement";

        private MySqlDataReader reader;
        private const int ERROR = -1;
        private const int NOT_FOUND = 0;

        public string Error_operation { get; set; }

        // Verifica se um Usuario existe no Banco de Dados
        public bool ExistsClient(string cpf)
        {
            Client client = new Client();
            if (!client.ValidationCPF(cpf))
            {
                Error_operation = client.Error_Validation;
                return false;
            }

            string count_formatted, command;
            try
            {
                count_formatted = string.Format("COUNT({0})", CPF);
                command = string.Format("SELECT {0} FROM {1} WHERE {2}='{3}'",
                        count_formatted, TABLE_CLIENT, CPF, cpf);
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
                    Error_operation = "Não foi possivel consultar a Tabela";
                    return false;
                }

                try
                {
                    int quantity = NOT_FOUND;
                    reader.Read();
                    quantity = reader.GetInt32(reader.GetOrdinal(count_formatted));

                    if (quantity > 1)
                    {
                        Error_operation = "Houve um erro no Banco de Dados. " +
                            "Mais de 1 Registro Encontrado";
                        return false;
                    }
                    else if (quantity == 1)
                    {
                        return true;
                    }
                    else
                    {
                        Error_operation = "Registro não Encontrado no Banco de Dados";
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Error_operation = "Não foi possivel obter o Verificar o Cliente.";
                    System.Diagnostics.Debug.WriteLine(Error_operation + " Exceção: " + ex);
                    return false;
                }
                finally
                {
                    if (reader != null) reader.Close();
                }
            }
        }

        // Insere um Usuario (Com dados Normalizados) se for Validado e não Existir
        public bool InsertClient(Client user)
        {
            if (user == null)
            {
                Error_operation = "Usuario não Informado";
                return false;
            }
            else if (ExistsClient(user.Cpf))
            {
                Error_operation = "Usuario já Cadastrado no Sistema";
                return false;
            }

            // Intancia das Classes de Endereço
            StateCity stateCity = new StateCity();
            stateCity.Estado = user.Estado;
            stateCity.Cidade = user.Cidade;

            Address address = new Address();
            address.Logradouro = user.Logradouro;

            // Metodos responsaveis por Buscar/Inserir (Se não Existir) o Estado/Cidade/Endereço
            int code_state_city = new StateCityDAO().CodeStateCityValid(stateCity);
            int code_address = new AddressDAO().CodeAddressValid(address);

            if (code_state_city == ERROR || code_state_city == NOT_FOUND) return false;
            if (code_address == ERROR || code_address == NOT_FOUND) return false;

            string command;
            try
            {
                command = string.Format("INSERT INTO {0}({1},{2},{3},{4},{5},{6}) " +
                    "VALUE('{7}', '{8}', {9}, {10}, {11}, '{12}')", TABLE_CLIENT, CPF,
                    NAME, STATE_CITY, ADDRESS, NUMBER, COMPLEMENT, user.Cpf, user.Name,
                    code_state_city, code_address, user.Numero, user.Complemento);
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

                // Erro na Inserção do Banco de DAdos
                if (database.ExecuteCommand(command) <= 0)
                {
                    Error_operation = database.Error_operation;
                    return false;
                }
                else return true;
            }
        }

        // Exlcui o Usuario e se Existir
        public bool DeleteClient(string cpf)
        {
            Client user = new Client();
            user = SelectClient(cpf);

            if (user == null) return false;

            AddressDAO addressDAO = new AddressDAO();
            Address address = new Address();
            address.Logradouro = user.Logradouro;

            // Verificar se o Usuario é o unico usando aquele endereço 
            if (addressDAO.IsOnlyAddress(address))
            {
                // É o unico com aquele Endereço ---> Exclui o Endereço
                bool is_deleted_address = addressDAO.
                    DeleteAddress(addressDAO.ReturnCodeAddress(address));

                if (!is_deleted_address)
                {
                    Error_operation = "Não foi Possivel excluir o Endereço " +
                        "do Banco de Dados. ";
                    return false;
                }
            }

            StateCityDAO stateCityDAO = new StateCityDAO();
            StateCity stateCity = new StateCity();
            stateCity.Estado = user.Estado;
            stateCity.Cidade = user.Cidade;

            // Verificar se o Usuario é o unico usando aquele endereço 
            if (stateCityDAO.IsOnlyStateCity(stateCity))
            {
                // É o unico com aquela Cidade e Estado ---> Exclui ambos do banco de dados
                bool is_deleted_stateCity = stateCityDAO.
                    DeleteStateCity(stateCityDAO.ReturnCodeStateCity(stateCity));

                if (!is_deleted_stateCity)
                {
                    Error_operation = "Não foi Possivel excluir o Estado e Ciade" +
                        " do Banco de Dados. ";
                    return false;
                }
            }

            string command;
            try
            {
                command = string.Format("DELETE FROM {0} WHERE {1}='{2}'",
                    TABLE_CLIENT, CPF, cpf);
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

                if (database.ExecuteCommand(command) <= 0)
                {
                    Error_operation = database.Error_operation;
                    return false;
                }
                else return true;
            }
        }

        // Atualiza (Com dados Normalizados) um Usuario caso Exista
        public bool UpdateClient(Client client)
        {
            if (client == null)
            {
                Error_operation = "Usuario não Informado";
                return false;
            }
            else if (!ExistsClient(client.Cpf)) return false;

            // Obtem a Cidade/Estado/Endereço antes de Atualizar
            Client oldUser = new Client();
            oldUser = SelectClient(client.Cpf);

            StateCityDAO stateCityDAO = new StateCityDAO();
            int code_state_city = NOT_FOUND;

            // Usado para obter o Antigo Codigo do Estado/Cidade
            StateCity oldStateCity = new StateCity();
            oldStateCity.Estado = oldUser.Estado;
            oldStateCity.Cidade = oldUser.Cidade;

            StateCity newStateCity = new StateCity();
            newStateCity.Estado = client.Estado;
            newStateCity.Cidade = client.Cidade;

            code_state_city = stateCityDAO.ReturnCodeStateCity(newStateCity);

            if (code_state_city == NOT_FOUND)
            {
                // Verifica se o Usuario é o Unico com aquele Estado/Cidade
                if (stateCityDAO.IsOnlyStateCity(oldStateCity))
                {
                    // Obtem o codigo da Antiga Cidade/Estado e Atualiza os Valores
                    code_state_city = stateCityDAO.ReturnCodeStateCity(oldStateCity);
                    if (!stateCityDAO.UpdateStateCity(code_state_city, newStateCity))
                    {
                        Error_operation = "Não foi Possivel Atualizar o Estado e Cidade";
                        return false;
                    }
                }
                else
                {
                    // Usuario não é o Unico com Cidade/Estado Antigo e o Endereço não existe no BD
                    if (stateCityDAO.InsertStateCity(newStateCity))
                    {
                        // Conseguiu Inserir a Cidade/Estado no Banco ---> Retorna o Codigo
                        code_state_city = stateCityDAO.ReturnCodeStateCity(newStateCity);
                    }
                    else
                    {
                        Error_operation = "Não foi Possivel Cadastrar o novo Estado e Cidade";
                        return false;
                    }
                }
            }

            // Erro ao obter o Codigo do Estado/Cidade ou ao Inserir o novo no Banco de Dados  
            if (code_state_city == ERROR)
            {
                Error_operation = "Houve um erro na Atualização do Estado e Cidade";
                return false;
            }

            AddressDAO addressDAO = new AddressDAO();
            int code_address = NOT_FOUND;

            // Usado para obter o antigo Codigo do Endereço
            Address oldAddress = new Address();
            oldAddress.Logradouro = oldUser.Logradouro;

            Address newAdrress = new Address();
            newAdrress.Logradouro = client.Logradouro;

            // Obtem o Codigo do novo Endereço (Codigo ou NOT_FOUND ou ERROR)
            code_address = addressDAO.ReturnCodeAddress(newAdrress);

            // Verifica se o Novo endereço já existe
            if (code_address == NOT_FOUND)
            {
                // Verifica se o Usuario é o Unico com aquele Endereço Antigo
                if (addressDAO.IsOnlyAddress(oldAddress))
                {
                    // Obtem o Codigo dos Antigos valores do Endereço e Atualiza
                    code_address = addressDAO.ReturnCodeAddress(oldAddress);
                    if (!addressDAO.UpdateAddress(code_address, newAdrress))
                    {
                        Error_operation = "Não foi Possivel Atualizar o Logradouro";
                        return false;
                    }
                }
                else
                {
                    // Usuario não é o unico com endereço antigo e o Endereço não existe no Banco de Dados
                    if (addressDAO.InsertAddress(newAdrress))
                    {
                        // Conseguiu inserir no banco de dados ---> Obtem o Codigo
                        code_address = addressDAO.ReturnCodeAddress(newAdrress);
                    }
                    else
                    {
                        Error_operation = "Não foi Possivel Cadastrar o novo Logradouro";
                        return false;
                    }
                }
            }

            // Erro ao obter o Codigo do Endereço ou ao Inserir o novo no banco
            if (code_address == ERROR)
            {
                Error_operation = "Houve um erro na Atualização do Logradouro";
                return false;
            }

            string command;
            try
            {
                command = string.Format("UPDATE {0} SET {1}='{2}',{3}={4}, " +
                    "{5}={6}, {7}={8}, {9}='{10}' WHERE {11}='{12}'", TABLE_CLIENT,
                    NAME, client.Name, STATE_CITY, code_state_city, ADDRESS, code_address,
                    NUMBER, client.Numero, COMPLEMENT, client.Complemento, CPF, client.Cpf);
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

            // Codigos ja obtidos (Inseridos ou Atualizados ou Obtidos) --> Atualiza o Usuario
            using (Database database = new Database())
            {
                if (!database.IsAvalibleDatabase)
                {
                    Error_operation = database.Error_operation;
                    return false;
                }

                if (database.ExecuteCommand(command) <= 0)
                {
                    Error_operation = database.Error_operation;
                    return false;
                }
                else return true;
            }

        }

        // Retorna um User se o usuario existir e obter seus Dados
        public Client SelectClient(string cpf)
        {
            if (!ExistsClient(cpf)) return null;

            string command;
            try
            {
                command = string.Format("SELECT * FROM {0} WHERE {1}='{2}'",
                    TABLE_CLIENT, CPF, cpf);
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

            int code_state_city = 0, code_address = 0;
            Client userDatabase = new Client();
            // Usuario existe no Banco de Dados
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
                    Error_operation = "Não foi possivel consultar a Tabela";
                    return null;
                }
                try
                {
                    reader.Read();

                    userDatabase.Cpf = reader.GetString(reader.GetOrdinal(CPF));
                    userDatabase.Name = reader.GetString(reader.GetOrdinal(NAME));
                    userDatabase.Numero = reader.GetInt32(reader.GetOrdinal(NUMBER));

                    if (!reader.IsDBNull(reader.GetOrdinal(COMPLEMENT)))
                    {
                        userDatabase.Complemento = reader.GetString(reader.GetOrdinal(COMPLEMENT));
                    }

                    code_state_city = reader.GetInt32(reader.GetOrdinal(STATE_CITY));
                    code_address = reader.GetInt32(reader.GetOrdinal(ADDRESS));
                }
                catch (Exception ex)
                {
                    Error_operation = "Não foi Possivel Selecionar o Cliente no Banco de dados.";
                    System.Diagnostics.Debug.WriteLine(Error_operation + " Exceção: " + ex);
                    return null;
                }
                finally
                {
                    if (reader != null) reader.Close();
                }
            }

            // Formata os Dados Normalizados do Banco de Dados
            StateCity stateCity = new StateCity();
            stateCity = new StateCityDAO().SelectStateCity(code_state_city);
            Address addressClass = new Address();
            addressClass = new AddressDAO().SelectAddress(code_address);

            if (stateCity == null || string.IsNullOrEmpty(stateCity.Cidade)
                || string.IsNullOrEmpty(stateCity.Estado))
            {
                stateCity = new StateCity();
                stateCity.Cidade = "Cidade não Definida";
                stateCity.Estado = "Estado não Definido";
            }
            else if (addressClass == null
                || string.IsNullOrEmpty(addressClass.Logradouro))
            {
                addressClass = new Address();
                addressClass.Logradouro = "Endereço não Definido";
            }
            else
            {
                userDatabase.Cidade = stateCity.Cidade;
                userDatabase.Estado = stateCity.Estado;
                userDatabase.Logradouro = addressClass.Logradouro;
            }
            return userDatabase;
        }

        // Lista os Usuarios Cadastrados no Banco de Dados
        public List<Client> ListClients()
        {
            string command;
            try
            {
                command = string.Format("SELECT * FROM {0}", TABLE_CLIENT);
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

            // Vendedor existe no Banco de Dados
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
                    Error_operation = "Não foi possivel consultar a Tabela";
                    return null;
                }

                try
                {
                    int code_state_city = NOT_FOUND, code_address = NOT_FOUND;
                    List<Client> listClients = new List<Client>();

                    while (reader.Read())
                    {
                        Client clientDatabase = new Client();

                        clientDatabase.Cpf = reader.GetString(reader.GetOrdinal(CPF));
                        clientDatabase.Name = reader.GetString(reader.GetOrdinal(NAME));
                        clientDatabase.Numero = reader.GetInt32(reader.GetOrdinal(NUMBER));

                        if (!reader.IsDBNull(reader.GetOrdinal(COMPLEMENT)))
                        {
                            clientDatabase.Complemento = reader.GetString(reader.GetOrdinal(COMPLEMENT));
                        }

                        code_state_city = reader.GetInt32(reader.GetOrdinal(STATE_CITY));
                        code_address = reader.GetInt32(reader.GetOrdinal(ADDRESS));

                        // Formata os Dados Normalizados do Banco de Dados
                        StateCity stateCity = new StateCity();
                        stateCity = new StateCityDAO().SelectStateCity(code_state_city);
                        Address addressClass = new Address();
                        addressClass = new AddressDAO().SelectAddress(code_address);

                        if (stateCity == null || string.IsNullOrEmpty(stateCity.Cidade)
                            || string.IsNullOrEmpty(stateCity.Estado))
                        {
                            stateCity = new StateCity();
                            stateCity.Cidade = "Cidade não Definida";
                            stateCity.Estado = "Estado não Definido";
                        }
                        else if (addressClass == null
                            || string.IsNullOrEmpty(addressClass.Logradouro))
                        {
                            addressClass = new Address();
                            addressClass.Logradouro = "Endereço não Definido";
                        }
                        else
                        {
                            clientDatabase.Cidade = stateCity.Cidade;
                            clientDatabase.Estado = stateCity.Estado;
                            clientDatabase.Logradouro = addressClass.Logradouro;
                        }
                        listClients.Add(clientDatabase);
                    }

                    return listClients;
                }
                catch (Exception ex)
                {
                    Error_operation = "Não foi Possivel Sel ecionar o Vendedor no Banco de dados.";
                    System.Diagnostics.Debug.WriteLine(Error_operation + " Exceção: " + ex);
                    return null;
                }
                finally
                {
                    if (reader != null) reader.Close();
                }
            }
        }


    }
}