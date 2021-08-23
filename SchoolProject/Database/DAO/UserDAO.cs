using MySql.Data.MySqlClient;
using SchoolProject.Models;
using System;

namespace Database.DAO
{
    public class UserDAO
    {

        private Database database;
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
            if(cpf_user == null || cpf_user.Length != 11)
            {
                error_operation = "CPF Invalido. CPF deve conter 11 Caracteres";
                return false;
            }

            try
            {
                database = new Database();

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
            catch (Exception ex)
            {
                error_operation = "Não foi Possivel verificar no Banco de dados. Exceção:" + ex;
                System.Diagnostics.Debug.WriteLine(error_operation);
                return false;
            }
            finally
            {
                reader.Close();
            }
        }

        // Caso um usuario não exista e passe pelas validações = Insere
        // Insere/Obtem os dados de Endereço já Normalizados
        public bool insertUser(User user)
        {
            if (user == null)
            {
                error_operation = "Usuario não Informado";
                return false;
            } else if (existsUser(user.Cpf))
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
                int code_state_city = codeStateCityValid(stateCity);
                int code_address = codeAddressValid(address);

                if (code_state_city == ERROR || code_state_city == NOT_FOUND) return false;
                if (code_address == ERROR || code_address == NOT_FOUND) return false;

                database = new Database();

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
            catch (Exception ex)
            {
                error_operation = "Não foi Cadastrar o Usuario no Banco de dados. Exceção:" + ex;
                System.Diagnostics.Debug.WriteLine(error_operation);
                return false;
            }
        }

        // Busca o Usuario e se Existir = Exclui
        public bool deleteUser(string cpf)
        {
            try
            {
                if (!existsUser(cpf))
                {
                    error_operation = "Usuario não Existe no Banco de Dados";
                    return false;
                }

         /*       // Verificar se o Usuario é o unico usando aquele endereço/estado/cidade
                if (!isOnlyAddress(cpf)) System.Diagnostics.Debug.WriteLine("Não foi Possivel " +
                     "Excluir o Endereço do Banco de Dados. Erro: " + error_operation);


                if (!isOnlyStateCity(cpf)) System.Diagnostics.Debug.WriteLine("Não foi Possivel " +
                     "Excluir o Estado e cidade do Banco de Dados. Erro: " + error_operation);
         */
                database = new Database();
                command = String.Format("DELETE FROM user WHERE {0}='{1}'", CPF, cpf);

                if (database.runCommand(command) == 0)
                {
                    error_operation = "Não foi Possivel Excluir do Banco de Dados";
                    return false;
                }
                else return true;
            }
            catch (Exception ex)
            {
                error_operation = "Não foi possivel Excluir do Usuario no Banco de dados. Exceção:" + ex;
                System.Diagnostics.Debug.WriteLine(error_operation);
                return false;
            }
        }

        // Busca o Usuario e se Existir = Atualiza, ja inserindo os dados Normalizados
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
                StateCity stateCity = new StateCity
                {
                    Estado = user.Estado,
                    Cidade = user.Cidade
                };
                Address address = new Address
                {
                    Logradouro = user.Logradouro
                };

                // Metodos responsaveis por Buscar/Inserir (Se não Existir) o Estado/Cidade/Endereço
                int code_state_city = codeStateCityValid(stateCity);
                int code_address= codeAddressValid(address);

                if (code_state_city == ERROR || code_state_city == NOT_FOUND) return false;
                if (code_address == ERROR || code_address == NOT_FOUND) return false;

                database = new Database();

                command = String.Format("UPDATE user SET {0}='{1}',{2}={3}, {4}={5}, {6}={7}, {8}='{9}'" +
                    " WHERE {10}='{11}'", NAME, user.Name, STATE_CITY, code_state_city, ADDRESS,
                    code_address, NUMBER, user.Numero, COMPLEMENT, user.Complemento, CPF, user.Cpf);

                System.Diagnostics.Debug.WriteLine(command);

                if (database.runCommand(command) == 0)
                {
                    error_operation = "Não foi Possivel Atualizar no Banco de Dados";
                    return false;
                }
                else return true;

            }
            catch (Exception ex)
            {
                error_operation = "Não foi possivel Alterar o Usuario no Banco de dados. Exceção:" + ex;
                System.Diagnostics.Debug.WriteLine(error_operation);
                return false;
            }
        }

        // Retorna um User se o usuario existir e se conseguiu obter os Dados (reader)
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
                database = new Database();

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
                        user.Cidade = "Cidade não Definida";
                        user.Estado = "Estado não Definido";
                    }

                    user.Cidade = stateCity.Cidade;
                    user.Estado = stateCity.Estado;

                    Address addressClass = new Address();
                    addressClass = new AddressDAO().selectAddress(code_address);

                    if (addressClass == null)
                    {
                        user.Logradouro = "Endereço não Definido";
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
            catch (Exception ex)
            {
                error_operation = "Não foi possivel Encontrar o Usuario no Banco de dados. Exceção:" + ex;
                System.Diagnostics.Debug.WriteLine(error_operation);
                return null;
            }
            finally
            {
                if(reader !=null) reader.Close();
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

        // Obtem o Codigo do Endereço. Caso não exista, tenta Inserir
        public int codeAddressValid(Address address)
        {
            if (address == null)
            {
                error_operation += " Logradouro não Informado";
                return ERROR;
            }

            try
            {
                // Obtem o Codigo Address
                int code_address = new AddressDAO().returnCodeAddress(address);

                // Erro nos Metodos do Address = Reader = null ou Problema na Execução (catch)
                if (code_address == ERROR)
                {
                    error_operation += " Erro na Busca do Codigo do Endereço";
                    return ERROR;
                }

                // Caso não exista no Banco de Dados ---> Insere
                if (code_address == NOT_FOUND)
                {
                    bool is_insert = new AddressDAO().insertAddress(address);

                    if (!is_insert)
                    {
                        error_operation = "Não foi Possivel Inserir o Logradouro no Banco de Dados";
                        return ERROR;
                    }
                    else
                    {
                        // Validação = Busca no Banco de Dados os Dados Inseridos (Logradouro)
                        code_address = new AddressDAO().returnCodeAddress(address);
                    }
                }

                if (code_address == ERROR || code_address == NOT_FOUND)
                {
                    error_operation += " Logradouro não Localizado no Banco de Dados";
                    return ERROR;
                }
                else
                {
                    return code_address;
                }
            }
            catch (Exception ex)
            {
                error_operation = "Erro ao buscar o Logradouro no Banco de dados. Exceção:" + ex;
                Console.WriteLine(error_operation);
                return ERROR;
            }
        }

        // Verifica se o Usuario excluido é o Unico com aquele Endereço
 /*       public bool isOnlyAddress(string cpf)
        {
            if (cpf.Length != 11)
            {
                error_operation = "CPF Invalido. O CPF deve ter 11 Digitos. ";
                return false;
            }

            User user = new User();
            user = new UserDAO().selectUser(cpf);
            if (user == null) 
            {
                error_operation = "Usuario não Encontrado. ";
                return false;
            }

            Address address = new Address();
            address.Logradouro = user.Logradouro;
            if (address == null || address.Logradouro.Length < 5)
            {
                error_operation = "Endereço não Encontrado. ";
                return false;
            }

            AddressDAO addressDAO = new AddressDAO();

            try
            {
                address.Code_address = addressDAO.returnCodeAddress(address);
                if (address.Code_address <= 0)
                {
                    error_operation = "Codigo de Endereço não Encontrado. ";
                    return false;
                }

                // Usuario existe no Banco de Dados
                database = new Database();

                command = String.Format("SELECT COUNT({0}) FROM user WHERE {0}={1}",
                    ADDRESS, address.Code_address);

                reader = database.readerTable(command);

                if (reader == null)
                {
                    error_operation = "Não foi possivel consultar a Tabela";
                    return false;
                }

                if (reader.HasRows)
                {
                    int quantity_register = NOT_FOUND;

                    while (reader.Read())
                    {
                        quantity_register = reader.GetInt32(reader.GetOrdinal("COUNT(code_address)"));
                    }

                    if (quantity_register == 1)
                    {
                        bool is_deleted_adress = addressDAO.deleteAddress(address.Code_address);

                        if (!is_deleted_adress)
                        {
                            error_operation = "Não foi Possivel excluir o Endereço do Banco de Dados. ";
                            return false;
                        }
                        else return true;

                    }
                    else return false;
                }
                else
                {
                    error_operation = "Erro na Leitura dos Dados do Banco de Dados";
                    return false;
                }
            }
            catch (Exception ex)
            {
                error_operation = "Não foi possivel Consultar o Endereço no Banco de dados. Exceção:" + ex;
                System.Diagnostics.Debug.WriteLine(error_operation);
                return false;
            }
            finally
            {
                reader.Close();
            }
        }

        // Verifica se o Usuario excluido é o Unico com aquela Cidade e Estado
        public bool isOnlyStateCity(string cpf)
        {
            if (cpf.Length != 11)
            {
                error_operation = "CPF Invalido. O CPF deve ter 11 Digitos. ";
                return false;
            }

            User user = new User();
            user = new UserDAO().selectUser(cpf);
            if (user == null)
            {
                error_operation = "Usuario não Encontrado. ";
                return false;
            }

            StateCity stateCity = new StateCity();
            stateCity.Cidade = user.Cidade;
            stateCity.Estado = user.Estado;
            if (stateCity == null)
            {
                error_operation = "Cidade e Estado não Encontrado. ";
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

                // Usuario existe no Banco de Dados
                database = new Database();

                command = String.Format("SELECT COUNT({0}) FROM user WHERE {0}={1}",
                    STATE_CITY, stateCity.Code_statecity);

                reader = database.readerTable(command);

                if (reader == null)
                {
                    error_operation = "Não foi possivel consultar a Tabela";
                    return false;
                }

                if (reader.HasRows)
                {
                    int quantity_register = NOT_FOUND;

                    while (reader.Read())
                    {
                        quantity_register = reader.GetInt32(reader.GetOrdinal("COUNT(code_state_city)"));
                    }

                    if (quantity_register == 1)
                    {
                        bool is_deleted_stateCity = stateCityDAO.deleteStateCity(stateCity.Code_statecity);

                        if (!is_deleted_stateCity)
                        {
                            error_operation = "Não foi Possivel excluir o Endereço do Banco de Dados. ";
                            return false;
                        }
                        else return true;

                    }
                    else return false;
                }
                else
                {
                    error_operation = "Erro na Leitura dos Dados do Banco de Dados";
                    return false;
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
                reader.Close();
            }
        }
*/
    }
}
