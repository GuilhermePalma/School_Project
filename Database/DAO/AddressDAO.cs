using MySql.Data.MySqlClient;
using SchoolProject.Models;
using System;

namespace Database.DAO
{
    public class AddressDAO
    {
        private Database database;
        MySqlDataReader reader;
        private string command;
        private string error_operation = "";

        private const string CODE = "code_address";
        private const string ADDRESS = "logradouro";

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
                database = new Database();

                command = String.Format("SELECT COUNT({0}) FROM address WHERE {0}={1}",
                    CODE, code);

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
                        quantity = reader.GetInt32(reader.GetOrdinal("COUNT(code_address)"));
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
                System.Diagnostics.Debug.WriteLine("Dados: ");
                return false;
            }

            try
            {
                database = new Database();

                command = String.Format("INSERT INTO address({0}) VALUE('{1}')", ADDRESS, address.Logradouro);

                if (database.runCommand(command) == 0)
                {
                    error_operation = "Não foi Possivel Cadsatrar no Banco de Dados";
                    return false;
                }
                else return true;
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
                database = new Database();

                command = String.Format("DELETE FROM address WHERE {0}={1}", CODE, code);

                if (database.runCommand(command) == 0)
                {
                    error_operation = "Não foi Possivel Excluir do Banco de Dados";
                    return false;
                } else return true;
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
                database = new Database();

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
                database = new Database();

                command = String.Format("SELECT {0} FROM address " +
                    "WHERE {1}='{2}'", CODE, ADDRESS, address.Logradouro);

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

        // Busca se Existe um Endereço Cadastrado no Banco de Dados
        public bool onlyAddress(int code)
        {
            if (code <= 0)
            {
                error_operation = "Codigo de Endereço Invalido. O codigo tem que ser" +
                    " um valor Positivo e Diferente de 0";
                return false;
            }

            try
            {
                database = new Database();

                // Obtem a quantidade de Usuarios com aquele codigo de Endereço
                command = String.Format("SELECT COUNT({0}) FROM user WHERE {0}={1}",
                    UserDAO.ADDRESS, code);

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
                        quantity = reader.GetInt32(reader.GetOrdinal("COUNT(code_address)"));
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
                if (reader != null) reader.Close();
            }
        }


    }
}
