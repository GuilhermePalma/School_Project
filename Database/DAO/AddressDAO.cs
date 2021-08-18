using MySql.Data.MySqlClient;
using OpreationsDatabase.DLLs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.DAO
{
    public class AddressDAO
    {
        private Database database;
        private string command;
        private string error_operation = "";

        private const string CODE = "code_address";
        private const string ADDRESS = "logradouro";

        private const int ERROR = -1;
        private const int NOT_FOUND = 0;

        public bool existsAddress(int code)
        {
            command = String.Format("SELECT COUNT({0}) FROM address WHERE {0}={1}",
               CODE, code);

            try
            {
                database = new Database();

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
        }

        public bool insertAddress(Address address)
        {
            try
            {
                database = new Database();

                command = String.Format("INSERT INTO address({0}) VALUE('{1}')", ADDRESS, address);
               
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

        public bool deleteAddress(int code)
        {
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

        public Address selectAddress(int code)
        {
            try
            {
                if (existsAddress(code))
                { 
                    database = new Database();

                    command = String.Format("SELECT * FROM address WHERE {0}={1}", CODE, code);

                    MySqlDataReader reader = database.readerTable(command);

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
                }

                // Caso HasRows = false ou não existe no Banco de Dados
                error_operation = "Dados não Encontrados no Banco de Dados";
                return null;
            }
            catch (Exception ex)
            {
                error_operation = "Não foi Excluir o Usuario no Banco de dados. Exceção:" + ex;
                System.Diagnostics.Debug.WriteLine(error_operation);
                return null;
            }
        }

        public int returnCodeAddress(Address address)
        {
            try
            {
                database = new Database();

                command = String.Format("SELECT {0} FROM address " +
                    "WHERE {1}='{2}'", CODE, ADDRESS, address.Logradouro);

                MySqlDataReader reader = database.readerTable(command);

                if (reader != null)
                {
                    if (reader.HasRows)
                    {
                        // Retorna o Codigo
                        return reader.GetInt32(reader.GetOrdinal(CODE));
                    }
                    else
                    {
                        error_operation = "Dados não Encontrados no Banco de Dados";
                        return NOT_FOUND;
                    }
                }
                else
                {
                    error_operation = "Não foi Possivel Acessar o Banco de Dados";
                    return ERROR;
                }
            }
            catch (Exception ex)
            {
                error_operation = "Não foi possivel obter o Codigo do Endereço. Exceção:" + ex;
                System.Diagnostics.Debug.WriteLine(error_operation);
                return ERROR;
            }
        }


    }
}
