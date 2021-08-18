using MySql.Data.MySqlClient;
using OpreationsDatabase.DLLs;
using SchoolProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.DAO
{
    public class UserDAO
    {

        private Database database;
        private string command;
        public string error_operation = "";

        private const string CPF = "cpf";
        private const string NAME = "name";
        private const string STATE_CITY = "code_state_city";
        private const string ADDRESS = "code_address";
        private const string NUMBER = "residential_number";
        private const string COMPLEMENT = "complement";

        private const int ERROR = -1;
        private const int NOT_FOUND = 0;


        public bool existsUser(string cpf_user)
        {
            try
            {
                database = new Database();

                command = String.Format("SELECT COUNT({0}) FROM user WHERE {0}='{1}'",
                    CPF, cpf_user);

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
        }

        public bool insertUser(User user)
        {

            if (existsUser(user.Cpf))
            {
                error_operation = "Usuario já foi Cadastrar no Sistema";
                return false;
            }
            else if (user == null)
            {
                error_operation = "Usuario não Informado";
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

                if (!checkStateCity(stateCity) || !checkAddress(address)) return false;

                int code_state_city = getCodeStateCity(stateCity);
                int code_address = getCodeAdress(address);

                database = new Database();

                command = String.Format("INSERT INTO user({0},{1},{2},{3},{4},{5}) VALUE(" +
                    "'{6}','{7}',{8},{9},{10},'{11}')", CPF, NAME, STATE_CITY, ADDRESS, NUMBER,
                    COMPLEMENT, user.Cpf, user.Name, code_state_city, code_address, 
                    user.Numero, user.Complemento);

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

        public bool deleteUser(string cpf)
        {
            try
            {
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

        public bool updateUser(User user)
        {

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

                if (!checkStateCity(stateCity) || !checkAddress(address)) return false;

                int code_state_city = getCodeStateCity(stateCity);
                int code_address = getCodeAdress(address);

                database = new Database();

                command = String.Format("UPDATE user SET {0}='{1}',{2}={3}, {4}={5}, {6}={7}, {8}='{9}'," +
                    " WHERE {10}='{11}'", NAME, user.Name, STATE_CITY, code_state_city, ADDRESS,
                    code_address, NUMBER, user.Numero, COMPLEMENT, user.Complemento, CPF, user.Cpf);

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

        public User selectUser(string cpf)
        {
            try
            {
                if (existsUser(cpf))
                {
                    database = new Database();

                    command = String.Format("SELECT * FROM user WHERE {0}='{1}'", CPF, cpf);

                    MySqlDataReader reader = database.readerTable(command);

                    if(reader == null)
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
                            user.Complemento = reader.GetString(reader.GetOrdinal(COMPLEMENT));

                            code_state_city = reader.GetInt32(reader.GetOrdinal(STATE_CITY));
                            code_address = reader.GetInt32(reader.GetOrdinal(ADDRESS));
                        }

                        StateCityDAO dao_state_city = new StateCityDAO();
                        StateCity class_state_city = new StateCity();
                        class_state_city = dao_state_city.selectStateCity(code_state_city);
                        user.Cidade = class_state_city.Cidade;
                        user.Estado = class_state_city.Estado;

                        AddressDAO addressDAO = new AddressDAO();
                        Address addressClass = addressDAO.selectAddress(code_address);
                        user.Logradouro = addressClass.Logradouro;

                        return user;
                    }
                }

                error_operation = "usuario não Localizadp no Banco de Dados";
                return null;

            }
            catch (Exception ex)
            {
                error_operation = "Não foi possivel Encontrar o Usuario no Banco de dados. Exceção:" + ex;
                System.Diagnostics.Debug.WriteLine(error_operation);
                return null;
            }
        }


        public bool checkStateCity(StateCity stateCity)
        {
            int code_state_city = getCodeStateCity(stateCity);

            if (code_state_city == ERROR)
            {
                // stateCity == null ou houve algum erro na Consulta (catch)
                error_operation = "Estado e/ou Cidade não Informados";
                return false;
            }
            else if (code_state_city == NOT_FOUND)
            {
                // Não existe no Banco de Dados

                StateCityDAO stateCityDAO = new StateCityDAO();
                bool has_insert = stateCityDAO.insertStateCity(stateCity);

                if (has_insert == false)
                {
                    error_operation = "Não foi possivel Inserir o Endereço no Banco de dados.";
                    return false;
                }
                else
                {
                    // Realiza uma busca para ver se a Inserção deu Certo
                    code_state_city = getCodeStateCity(stateCity);
                    return code_state_city != NOT_FOUND || code_state_city != ERROR;
                }
            }
            else
            {
                // Existe no Banco de Dados
                return true;
            }
        }

        public int getCodeStateCity(StateCity stateCity)
        {
            try
            {
                if (stateCity == null) return ERROR;

                StateCityDAO stateCityDAO = new StateCityDAO();
                return stateCityDAO.returnCodeStateCity(stateCity);
            }
            catch (Exception ex)
            {
                error_operation = "Não foi possivel Encontrar o Estado e Cidade no Banco de dados. Exceção:" + ex;
                Console.WriteLine(error_operation);
                return ERROR;
            }
        }


        public bool checkAddress(Address address)
        {
            int code_address = getCodeAdress(address);

            if (code_address == ERROR)
            {
                // address == null ou houve algum erro na Consulta (catch)
                error_operation = "Logradouro não Informados";
                return false;
            }
            else if (code_address == NOT_FOUND)
            {
                AddressDAO addressDAO = new AddressDAO();
                bool has_insert = addressDAO.insertAddress(address);

                if (has_insert == false)
                {
                    error_operation = "Não foi possivel Inserir o Endereço no Banco de dados.";
                    return false;
                }
                else
                {
                    // Verifica se a Inserção no BD foi bem Realizada
                    code_address = getCodeAdress(address);
                    return code_address != NOT_FOUND || code_address != ERROR;
                }
            }
            else
            {
                return true;
            }
        }
        
        public int getCodeAdress(Address address)
        {
            try
            {
                if (address == null)
                {
                    return ERROR;
                }

                AddressDAO addressDAO = new AddressDAO();
                return addressDAO.returnCodeAddress(address);
            }
            catch (Exception ex)
            {
                error_operation = "Não foi possivel Encontrar o Estado e Cidade no Banco de dados. Exceção:" + ex;
                Console.WriteLine(error_operation);
                return ERROR;
            }
        }


    }
}
