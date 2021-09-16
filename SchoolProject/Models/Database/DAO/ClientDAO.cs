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
        public const string PHONE = "phone";
        public const string DDD = "ddd";

        private MySqlDataReader reader;
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
                catch (IndexOutOfRangeException ex)
                {
                    Error_operation = "Não foi possivel Obter os Dados.";
                    System.Diagnostics.Debug.WriteLine(Error_operation + " Exceção: " + ex);
                    return false;
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
            StateCity stateCity = new StateCity()
            {
                Estado = user.Estado,
                Cidade = user.Cidade
            };

            Address address = new Address()
            {
                Logradouro = user.Logradouro
            };

            StateCityDAO stateCityDAO = new StateCityDAO();
            AddressDAO addressDAO = new AddressDAO();

            // Metodos responsaveis por Buscar/Inserir (Se não Existir) o Estado/Cidade/Endereço
            int code_state_city = stateCityDAO.CodeStateCityValid(stateCity);
            int code_address = addressDAO.CodeAddressValid(address);

            if (code_state_city < 1)
            {
                Error_operation = stateCityDAO.Error_operation;
                return false;
            }
            if (code_address < 1)
            {
                Error_operation = addressDAO.Error_operation;
                return false;
            }

            string command;
            try
            {
                command = string.Format("INSERT INTO {0}({1},{2},{3},{4},{5},{6},{7},{8}) " +
                    "VALUE('{9}', '{10}', {11}, {12}, {13}, '{14}', '{15}', '{16}')", 
                    TABLE_CLIENT, CPF, NAME, STATE_CITY, ADDRESS, NUMBER, COMPLEMENT, PHONE,
                    DDD, user.Cpf, user.Name, code_state_city, code_address, user.Numero, 
                    user.Complemento, user.Telefone, user.Ddd);
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
            Client client = SelectClient(cpf);

            if (client == null) return false;

            // Verificar se o Usuario é o unico usando aquele endereço ---> Exclui
            AddressDAO addressDAO = new AddressDAO();
            StateCityDAO stateCityDAO = new StateCityDAO();
            Address address = new Address()
            {
                Logradouro = client.Logradouro
            };

            StateCity stateCity = new StateCity()
            {
                Estado = client.Estado,
                Cidade = client.Cidade,
            };

            if (!addressDAO.DeleteOnlyAddress(address)
                && !string.IsNullOrEmpty(addressDAO.Error_operation))
            {
                Error_operation = addressDAO.Error_operation;
                return false;
            }
            else if (!stateCityDAO.DeleteOnlyStateCity(stateCity)
                && !string.IsNullOrEmpty(stateCityDAO.Error_operation))
            {
                Error_operation = stateCityDAO.Error_operation;
                return false;
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
            Client oldClient = SelectClient(client.Cpf);

            if (oldClient == null) return false;

            StateCityDAO stateCityDAO = new StateCityDAO();
            AddressDAO addressDAO = new AddressDAO();

            // Usado para obter o Antigo Codigo do Estado/Cidade
            StateCity oldStateCity = new StateCity()
            {
                Estado = oldClient.Estado,
                Cidade = oldClient.Cidade
            };
            StateCity newStateCity = new StateCity()
            {
                Estado = client.Estado,
                Cidade = client.Cidade
            };
            Address oldAddress = new Address()
            {
                Logradouro = oldClient.Logradouro
            };
            Address newAddress = new Address()
            {
                Logradouro = client.Logradouro
            };

            // Verificar se o Usuario é o unico usando aquele Estado/Cidade/Logradouro
            if (!stateCityDAO.UpdateOnlyStateCity(oldStateCity, newStateCity))
            {
                Error_operation = stateCityDAO.Error_operation;
                return false;
            }
            else if (!addressDAO.UpdateOnlyStateCity(oldAddress, newAddress))
            {
                Error_operation = addressDAO.Error_operation;
                return false;
            }
            else
            {
                int code_stateCity = NOT_FOUND;
                int code_address = NOT_FOUND;
                code_stateCity = stateCityDAO.ReturnCodeStateCity(newStateCity);
                code_address = addressDAO.ReturnCodeAddress(newAddress);

                if (code_stateCity < 1)
                {
                    Error_operation = stateCityDAO.Error_operation;
                    return false;
                }
                else if (code_address < 1)
                {
                    Error_operation = addressDAO.Error_operation;
                    return false;
                }
                else
                {
                    newStateCity.Code_stateCity = code_stateCity;
                    newAddress.Code_address = code_address;
                }
            }

            string command;
            try
            {
                command = string.Format("UPDATE {0} SET {1}='{2}',{3}={4}, {5}={6}, " +
                    "{7}={8}, {9}='{10}', {11}='{12}', {13}='{14}' WHERE {15}='{16}'", 
                    TABLE_CLIENT, NAME, client.Name, STATE_CITY, newStateCity.Code_stateCity, 
                    ADDRESS, newAddress.Code_address, NUMBER, client.Numero, COMPLEMENT,
                    client.Complemento, DDD, client.Ddd, PHONE, client.Telefone, CPF, client.Cpf);
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
            Client userDatabase;
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
                    userDatabase = new Client()
                    {
                        Cpf = reader.GetString(reader.GetOrdinal(CPF)),
                        Name = reader.GetString(reader.GetOrdinal(NAME)),
                        Numero = reader.GetInt32(reader.GetOrdinal(NUMBER)),
                        Ddd = reader.GetString(reader.GetOrdinal(DDD)),
                        Telefone = reader.GetString(reader.GetOrdinal(PHONE))
                    };

                    if (!reader.IsDBNull(reader.GetOrdinal(COMPLEMENT)))
                    {
                        userDatabase.Complemento = reader.GetString(reader.GetOrdinal(COMPLEMENT));
                    }

                    code_state_city = reader.GetInt32(reader.GetOrdinal(STATE_CITY));
                    code_address = reader.GetInt32(reader.GetOrdinal(ADDRESS));
                }
                catch (IndexOutOfRangeException ex)
                {
                    Error_operation = "Não foi possivel Obter os Dados.";
                    System.Diagnostics.Debug.WriteLine(Error_operation + " Exceção: " + ex);
                    return null;
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
            StateCityDAO stateCityDAO = new StateCityDAO();
            AddressDAO addressDAO = new AddressDAO();
            StateCity stateCity = stateCityDAO.SelectStateCity(code_state_city);
            Address addressClass = addressDAO.SelectAddress(code_address);

            if (stateCity == null)
            {
                Error_operation = stateCityDAO.Error_operation;
                return null;
            }
            else if (addressClass == null)
            {
                Error_operation = addressDAO.Error_operation;
                return null;
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
                        Client clientDatabase = new Client()
                        {
                            Cpf = reader.GetString(reader.GetOrdinal(CPF)),
                            Name = reader.GetString(reader.GetOrdinal(NAME)),
                            Numero = reader.GetInt32(reader.GetOrdinal(NUMBER)),
                            Telefone = reader.GetString(reader.GetOrdinal(PHONE)),
                            Ddd = reader.GetString(reader.GetOrdinal(DDD))
                        };

                        if (!reader.IsDBNull(reader.GetOrdinal(COMPLEMENT)))
                        {
                            clientDatabase.Complemento = reader.GetString(reader.GetOrdinal(COMPLEMENT));
                        }

                        code_state_city = reader.GetInt32(reader.GetOrdinal(STATE_CITY));
                        code_address = reader.GetInt32(reader.GetOrdinal(ADDRESS));

                        // Formata os Dados Normalizados do Banco de Dados
                        StateCityDAO stateCityDAO = new StateCityDAO();
                        AddressDAO addressDAO = new AddressDAO();
                        StateCity stateCity = stateCityDAO.SelectStateCity(code_state_city);
                        Address addressClass = addressDAO.SelectAddress(code_address);

                        if (stateCity == null)
                        {
                            Error_operation = stateCityDAO.Error_operation;
                            return null;
                        }
                        else if (addressClass == null)
                        {
                            Error_operation = addressDAO.Error_operation;
                            return null;   
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
                catch (IndexOutOfRangeException ex)
                {
                    Error_operation = "Não foi possivel Obter os Dados.";
                    System.Diagnostics.Debug.WriteLine(Error_operation + " Exceção: " + ex);
                    return null;
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