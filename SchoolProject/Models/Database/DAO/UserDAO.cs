using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace SchoolProject.Models.Database.DAO
{
    public class UserDAO
    {

        private MySqlDataReader reader;
        private string command;
        public string error_operation = "";

        public const string CPF = "cpf";
        public const string NAME = "name";
        public const string STATE_CITY = "code_state_city";
        public const string ADDRESS = "code_address";
        public const string NUMBER = "residential_number";
        public const string COMPLEMENT = "complement";

        private const int ERROR = -1;
        private const int NOT_FOUND = 0;

        // Verifica se um Usuario existe no Banco de Dados
        public bool existsUser(string cpf_user)
        {
            if (cpf_user == null || cpf_user.Length != 11)
            {
                error_operation = "CPF Invalido. CPF deve conter 11 Caracteres";
                return false;
            }

            try
            {
                using (Database database = new Database())
                {

                    command = String.Format("SELECT COUNT({0}) FROM user WHERE {0}='{1}'",
                        CPF, cpf_user);

                    reader = database.readerTable(command);

                    if (reader == null)
                    {
                        error_operation = "Não foi possivel consultar a Tabela";
                        return false;
                    }

                    if (reader.HasRows)
                    {
                        int quantity = NOT_FOUND;

                        while (reader.Read())
                        {
                            quantity = reader.GetInt32(reader.GetOrdinal("COUNT(cpf)"));
                        }

                        return quantity == 1 ? true : false;
                    }
                    else
                    {
                        error_operation = "Dados não Encontrados no Banco de Dados";
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                error_operation = "Não foi Possivel verificar no Banco de dados. Exceção:" + ex;
                System.Diagnostics.Debug.WriteLine(error_operation);
                return false;
            }
            finally
            {
                if (reader != null) reader.Close();
            }
        }

        // Insere um Usuario (Com dados Normalizados) se for Validado e não Existir
        public bool insertUser(User user)
        {
            if (user == null)
            {
                error_operation = "Usuario não Informado";
                return false;
            }
            else if (existsUser(user.Cpf))
            {
                error_operation = "Usuario já Cadastrado no Sistema";
                return false;
            }

            if (user.Complemento == "")
            {
                user.Complemento = "";
            }

            try
            {
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

                // Metodos responsaveis por Buscar/Inserir (Se não Existir) o Estado/Cidade/Endereço
                int code_state_city = new StateCityDAO().codeStateCityValid(stateCity);
                int code_address = new AddressDAO().codeAddressValid(address);

                if (code_state_city == ERROR || code_state_city == NOT_FOUND) return false;
                if (code_address == ERROR || code_address == NOT_FOUND) return false;

                using (Database database = new Database())
                {

                    command = String.Format("INSERT INTO user({0},{1},{2},{3},{4},{5}) VALUE(" +
                        "'{6}', '{7}', {8}, {9}, {10}, '{11}')", CPF, NAME, STATE_CITY, ADDRESS, NUMBER,
                        COMPLEMENT, user.Cpf, user.Name, code_state_city, code_address,
                        user.Numero, user.Complemento);

                    // Erro na Inserção do Banco de DAdos
                    if (database.runCommand(command) == 0)
                    {
                        error_operation = "Não foi Possivel Cadastrar no Banco de Dados";
                        return false;
                    }
                    else return true;
                }

            }
            catch (Exception ex)
            {
                error_operation = "Não foi Cadastrar o Usuario no Banco de dados. Exceção:" + ex;
                System.Diagnostics.Debug.WriteLine(error_operation);
                return false;
            }
        }

        // Exlcui o Usuario e se Existir
        public bool deleteUser(string cpf)
        {
            try
            {
                if (!existsUser(cpf))
                {
                    error_operation = "Usuario não Existe no Banco de Dados";
                    return false;
                }

                User user = new User();
                user = selectUser(cpf);
                if (user == null)
                {
                    error_operation = "Usuario não Localizado no Banco de Dados";
                    return false;
                }

                AddressDAO addressDAO = new AddressDAO();
                Address address = new Address();
                address.Logradouro = user.Logradouro;

                // Verificar se o Usuario é o unico usando aquele endereço 
                if (addressDAO.isOnlyAddress(address))
                {
                    // É o unico com aquele Endereço ---> Exclui o Endereço
                    bool is_deleted_address = addressDAO.
                        deleteAddress(addressDAO.returnCodeAddress(address));

                    if (!is_deleted_address)
                    {
                        error_operation = "Não foi Possivel excluir o Endereço " +
                            "do Banco de Dados. ";
                        return false;
                    }
                }

                StateCityDAO stateCityDAO = new StateCityDAO();
                StateCity stateCity = new StateCity();
                stateCity.Estado = user.Estado;
                stateCity.Cidade = user.Cidade;

                // Verificar se o Usuario é o unico usando aquele endereço 
                if (stateCityDAO.isOnlyStateCity(stateCity))
                {
                    // É o unico com aquela Cidade e Estado ---> Exclui ambos do banco de dados
                    bool is_deleted_stateCity = stateCityDAO.
                        deleteStateCity(stateCityDAO.returnCodeStateCity(stateCity));

                    if (!is_deleted_stateCity)
                    {
                        error_operation = "Não foi Possivel excluir o Estado e Ciade" +
                            " do Banco de Dados. ";
                        return false;
                    }
                }

                using (Database database = new Database())
                {
                    command = String.Format("DELETE FROM user WHERE {0}='{1}'", CPF, cpf);

                    if (database.runCommand(command) == 0)
                    {
                        error_operation = "Não foi Possivel Excluir do Banco de Dados";
                        return false;
                    }
                    else return true;
                }
            }
            catch (Exception ex)
            {
                error_operation = "Não foi possivel Excluir do Usuario no Banco de dados. Exceção:" + ex;
                System.Diagnostics.Debug.WriteLine(error_operation);
                return false;
            }
        }

        // Atualiza (Com dados Normalizados) um Usuario caso Exista
        public bool updateUser(User user)
        {
            if (user == null)
            {
                error_operation = "Usuario não Informado";
                return false;
            }
            else if (!existsUser(user.Cpf))
            {
                error_operation = "Usuario não Cadastrado no Sistema";
                return false;
            }

            if (user.Complemento == "")
            {
                user.Complemento = null;
            }

            try
            {

                // Obtem a Cidade/Estado/Endereço antes de Atualizar
                User oldUser = new User();
                oldUser = selectUser(user.Cpf);

                StateCityDAO stateCityDAO = new StateCityDAO();
                int code_state_city = NOT_FOUND;

                // Usado para obter o Antigo Codigo do Estado/Cidade
                StateCity oldStateCity = new StateCity();
                oldStateCity.Estado = oldUser.Estado;
                oldStateCity.Cidade = oldUser.Cidade;

                StateCity newStateCity = new StateCity();
                newStateCity.Estado = user.Estado;
                newStateCity.Cidade = user.Cidade;

                code_state_city = stateCityDAO.returnCodeStateCity(newStateCity);

                if(code_state_city == NOT_FOUND)
                {
                    // Verifica se o Usuario é o Unico com aquele Estado/Cidade
                    if (stateCityDAO.isOnlyStateCity(oldStateCity))
                    {
                        // Obtem o codigo da Antiga Cidade/Estado e Atualiza os Valores
                        code_state_city = stateCityDAO.returnCodeStateCity(oldStateCity);
                        if (!stateCityDAO.updateStateCity(code_state_city, newStateCity))
                        {
                            error_operation = "Não foi Possivel Atualizar o Estado e Cidade";
                            return false;
                        }
                    }
                    else
                    {
                        // Usuario não é o Unico com Cidade/Estado Antigo e o Endereço não existe no BD
                        if (stateCityDAO.insertStateCity(newStateCity))
                        {
                            // Conseguiu Inserir a Cidade/Estado no Banco ---> Retorna o Codigo
                            code_state_city = stateCityDAO.returnCodeStateCity(newStateCity);
                        }
                        else
                        {
                            error_operation = "Não foi Possivel Cadastrar o novo Estado e Cidade";
                            return false;
                        }
                    }
                }
              /* else
                {
                    // Novo Estado/Cidade já Existente no Banco de Dados.
                    // Verifica se o Usuario é o Unico com aquele Estado/Cidade
                    if (stateCityDAO.isOnlyStateCity(oldStateCity))
                    {
                        // Exclui o Endereço Antigos e Assume o Já Existente
                        int old_code_stateCity = stateCityDAO.returnCodeStateCity(oldStateCity);
                        stateCityDAO.deleteStateCity(old_code_stateCity);
                    }
                }
              */
                // Erro ao obter o Codigo do Estado/Cidade ou ao Inserir o novo no Banco de Dados  
                if (code_state_city == ERROR)
                {
                    error_operation = "Houve um erro na Atualização do Estado e Cidade";
                    return false;
                }

                AddressDAO addressDAO = new AddressDAO();
                int code_address = NOT_FOUND;

                // Usado para obter o antigo Codigo do Endereço
                Address oldAddress = new Address();
                oldAddress.Logradouro = oldUser.Logradouro;

                Address newAdrress = new Address();
                newAdrress.Logradouro = user.Logradouro;

                // Obtem o Codigo do novo Endereço (Codigo ou NOT_FOUND ou ERROR)
                code_address = addressDAO.returnCodeAddress(newAdrress);

                // Verifica se o Novo endereço já existe
                if (code_address == NOT_FOUND)
                {
                    // Verifica se o Usuario é o Unico com aquele Endereço Antigo
                    if (addressDAO.isOnlyAddress(oldAddress))
                    {
                        // Obtem o Codigo dos Antigos valores do Endereço e Atualiza
                        code_address = addressDAO.returnCodeAddress(oldAddress);
                        if (!addressDAO.updateAddress(code_address, newAdrress))
                        {
                            error_operation = "Não foi Possivel Atualizar o Logradouro";
                            return false;
                        }
                    }
                    else
                    {
                        // Usuario não é o unico com endereço antigo e o Endereço não existe no Banco de Dados
                        if (addressDAO.insertAddress(newAdrress))
                        {
                            // Conseguiu inserir no banco de dados ---> Obtem o Codigo
                            code_address = addressDAO.returnCodeAddress(newAdrress);
                        }
                        else
                        {
                            error_operation = "Não foi Possivel Cadastrar o novo Logradouro";
                            return false;
                        }
                    }
                }
             /*   else
                {
                    // Verifica se o Usuario é o Unico com aquele Endereço Antigo
                    if (addressDAO.isOnlyAddress(oldAddress))
                    {
                        // Exclui o Endereço Antigos e Assume o Já Existente
                        int old_code_address = addressDAO.returnCodeAddress(oldAddress);
                        addressDAO.deleteAddress(old_code_address);
                    }
                }
             */
                

                // Erro ao obter o Codigo do Endereço ou ao Inserir o novo no banco
                if (code_address == ERROR)
                {
                    error_operation = "Houve um erro na Atualização do Logradouro";
                    return false;
                }

                // Codigos ja obtidos (Inseridos ou Atualizados ou Obtidos) --> Atualiza o Usuario
                using (Database database = new Database())
                {
                    command = String.Format("UPDATE user SET {0}='{1}',{2}={3}, {4}={5}, {6}={7}, {8}='{9}'" +
                        " WHERE {10}='{11}'", NAME, user.Name, STATE_CITY, code_state_city, ADDRESS,
                        code_address, NUMBER, user.Numero, COMPLEMENT, user.Complemento, CPF, user.Cpf);

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
                error_operation = "Não foi possivel Alterar o Usuario no Banco de dados. Exceção:" + ex;
                System.Diagnostics.Debug.WriteLine(error_operation);
                return false;
            }
        }

        // Retorna um User se o usuario existir e obter seus Dados
        public User selectUser(string cpf)
        {
            if (cpf == null || cpf.Length != 11)
            {
                error_operation = "CPF Invalido. CPF deve conter 11 Caracteres";
                return null;
            }

            try
            {
                if (!existsUser(cpf))
                {
                    error_operation = "Usuario não Cadastrado no Sistema";
                    return null;
                }

                // Usuario existe no Banco de Dados
                using (Database database = new Database())
                {

                    command = String.Format("SELECT * FROM user WHERE {0}='{1}'", CPF, cpf);

                    reader = database.readerTable(command);

                    if (reader == null)
                    {
                        error_operation = "Não foi possivel consultar a Tabela";
                        return null;
                    }

                    if (reader.HasRows)
                    {
                        User user = new User();

                        int code_state_city = 0, code_address = 0;

                        while (reader.Read())
                        {
                            user.Cpf = reader.GetString(reader.GetOrdinal(CPF));
                            user.Name = reader.GetString(reader.GetOrdinal(NAME));
                            user.Numero = reader.GetInt32(reader.GetOrdinal(NUMBER));

                            if (!reader.IsDBNull(reader.GetOrdinal(COMPLEMENT)))
                            {
                                user.Complemento = reader.GetString(reader.GetOrdinal(COMPLEMENT));
                            }

                            code_state_city = reader.GetInt32(reader.GetOrdinal(STATE_CITY));
                            code_address = reader.GetInt32(reader.GetOrdinal(ADDRESS));
                        }

                        // Formata os Dados Normalizados do Banco de Dados
                        StateCity stateCity = new StateCity();
                        stateCity = new StateCityDAO().selectStateCity(code_state_city);

                        if (stateCity == null || stateCity.Cidade == null)
                        {
                            stateCity = new StateCity();
                            stateCity.Cidade = "Cidade não Definida";
                            stateCity.Estado = "Estado não Definido";
                        }

                        user.Cidade = stateCity.Cidade;
                        user.Estado = stateCity.Estado;

                        Address addressClass = new Address();
                        addressClass = new AddressDAO().selectAddress(code_address);

                        if (addressClass == null)
                        {
                            addressClass = new Address();
                            addressClass.Logradouro = "Endereço não Definido";
                        }

                        user.Logradouro = addressClass.Logradouro;

                        return user;
                    }
                    else
                    {
                        error_operation = "Erro na Leitura dos Dados do Banco de Dados";
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                error_operation = "Não foi possivel Encontrar o Usuario no Banco de dados. Exceção:" + ex;
                System.Diagnostics.Debug.WriteLine(error_operation);
                return null;
            }
            finally
            {
                if (reader != null) reader.Close();
            }
        }

    }
}