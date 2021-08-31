using MySql.Data.MySqlClient;
using System;

namespace SchoolProject.Models.Database.DAO
{
    public class AddressDAO
    {

        MySqlDataReader reader;
        private string command;
        private string error_operation = "";

        public const string TABLE_ADDRESS = "address";
        public const string CODE = "code_address";
        public const string ADDRESS = "logradouro";

        private const int ERROR = -1;
        private const int NOT_FOUND = 0;

        // Busca se Existe um Endereço Cadastrado no Banco de Dados
        public bool existsAddress(int code)
        {
            if(code <= 0)
            {
                error_operation = "Codigo de Endereço Invalido. O codigo tem que ser" +
                    " um valor Positivo e Diferente de 0";
                return false;
            }

            try
            {
                using(Database database = new Database())
                {
                    command = String.Format("SELECT COUNT({0}) FROM {1} WHERE {0}={2}",
                    CODE, TABLE_ADDRESS, code);

                    reader = database.readerTable(command);

                    if (reader == null)
                    {
                        error_operation = "Não foi possivel consultar a Tabela";
                        return false;
                    }

                    if (reader.HasRows)
                    {
                        string formmated_count = String.Format("COUNT({0})", CODE);
                        int quantity = NOT_FOUND;

                        while (reader.Read())
                        {
                            quantity = reader.GetInt32(reader.GetOrdinal(formmated_count));
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

        // Caso o Endereço não exista = Cadastra
        public bool insertAddress(Address address)
        {
            if (address == null)
            {
                error_operation = "Usuario não Informado";
                return false;
            }

            if (existsAddress(returnCodeAddress(address)))
            {
                return false;
            }

            try
            {
                using (Database database = new Database())
                {
                    command = String.Format("INSERT INTO {0}({1}) VALUE('{2}')", 
                        TABLE_ADDRESS, ADDRESS, address.Logradouro);

                    if (database.runCommand(command) == 0)
                    {
                        error_operation = "Não foi Possivel Cadsatrar no Banco de Dados";
                        return false;
                    }
                    else return true;
                }
            }
            catch (Exception ex)
            {
                error_operation = "Não foi Cadastrar o Endereço no Banco de dados. Exceção:" + ex;
                System.Diagnostics.Debug.WriteLine(error_operation);
                return false;
            }
        }

        // Caso um Endereço exista = Exclui
        public bool deleteAddress(int code)
        {
            if (!existsAddress(code))
            {
                error_operation += " Endereço não Cadastrado no Sistema";
                return false;
            }

            try
            {
                using (Database database = new Database())
                {
                    command = String.Format("DELETE FROM {0} WHERE {1}={2}", 
                        TABLE_ADDRESS, CODE, code);

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
                error_operation = "Não foi Excluir o Endereço do Banco de dados. Exceção:" + ex;
                System.Diagnostics.Debug.WriteLine(error_operation);
                return false;
            }
        }

        // Caso o Endereço Exista, Retorna a Classe uma instanciada
        public Address selectAddress(int code)
        {
            if (!existsAddress(code))
            {
                error_operation += " Endereço não Cadastrado no Sistema";
                return null;
            }

            try
            {
                using (Database database = new Database())
                {
                    command = String.Format("SELECT * FROM address WHERE {0}={1}", CODE, code);

                    reader = database.readerTable(command);

                    if (reader == null)
                    {
                        error_operation = "Não foi possivel consultar a Tabela";
                        return null;
                    }

                    if (reader.HasRows)
                    {
                        Address address = new Address();

                        while (reader.Read())
                        {
                            address.Code_address = reader.GetInt32(reader.GetOrdinal(CODE));
                            address.Logradouro = reader.GetString(reader.GetOrdinal(ADDRESS));
                        }

                        return address;
                    }
                    else
                    {
                        error_operation = "Não foi possivel consultar a Tabela";
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                error_operation = "Não foi Excluir o Usuario no Banco de dados. Exceção:" + ex;
                System.Diagnostics.Debug.WriteLine(error_operation);
                return null;
            }
            finally
            {
                if (reader != null) reader.Close();
            }
        }

        // Atualiza o Endereço no Banco de Dados
        public bool updateAddress(int old_codeAddress, Address address)
        {
            if (address == null || address.Logradouro == null)
            {
                error_operation = "Logradouro não Informado";
                return false;
            }

            address.Code_address = old_codeAddress;

            if (!existsAddress(address.Code_address))
            {
                error_operation = "Estado/Cidade não Cadastrado no Sistema";
                return false;
            }

            try
            {
                using (Database database = new Database())
                {
                    command = String.Format("UPDATE {0} SET {1}='{2}' WHERE {3}={4}",
                        TABLE_ADDRESS, ADDRESS, address.Logradouro, CODE, address.Code_address);

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
                error_operation = "Não foi possivel Alterar o Endereço " +
                    "no Banco de dados. Exceção:" + ex;
                System.Diagnostics.Debug.WriteLine(error_operation);
                return false;
            }
        }

        // Busca o Codigo de um Endereço se for Valido
        public int returnCodeAddress(Address address)
        {
            if (address == null || address.Logradouro.Length < 5)
            {
                error_operation = "Endereço Invalido. O Preenchimento do Logradouro" +
                    " é Obrigatorio";
                return ERROR;
            }

            try
            {
                using (Database database = new Database())
                {

                    command = String.Format("SELECT {0} FROM {1} WHERE {2}='{3}'", 
                        CODE, TABLE_ADDRESS, ADDRESS, address.Logradouro);

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

                        // Retorna o Codigo ou 0 (não encontrado)
                        return code;
                    }
                    else
                    {
                        error_operation = "Dados não Encontrados no Banco de Dados";
                        return NOT_FOUND;
                    }
                }
            }
            catch (Exception ex)
            {
                error_operation = "Não foi possivel obter o Codigo do Endereço. Exceção:" + ex;
                System.Diagnostics.Debug.WriteLine(error_operation);
                return ERROR;
            }
            finally
            {
                if (reader != null) reader.Close();
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
        public bool isOnlyAddress(Address address)
        {

            if (address == null || address.Logradouro.Length < 5)
            {
                error_operation = "Logradouro Invalido. ";
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

                // Endereço existe no Banco de Dados
                using (Database database = new Database())
                {
                    command = String.Format("SELECT COUNT({0}) FROM {0} WHERE {1}={2}",
                    ClientDAO.ADDRESS, ClientDAO.TABLE_CLIENT, ClientDAO.TABLE_CLIENT, address.Code_address);

                    reader = database.readerTable(command);

                    if (reader == null)
                    {
                        error_operation = "Não foi possivel consultar a Tabela";
                        return false;
                    }

                    if (reader.HasRows)
                    {
                        String formated_count = String.Format("COUNT({0})", ClientDAO.ADDRESS);
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
                        error_operation = "Não foi Possivel Ler os Dados do Banco de Dados";
                        return false;
                    }
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
                if (reader != null) reader.Close();
            }
        }

    }
}
