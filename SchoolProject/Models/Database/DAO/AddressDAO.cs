using MySql.Data.MySqlClient;
using System;

namespace SchoolProject.Models.Database.DAO
{
    public class AddressDAO
    {
        private const string NAME_FOREIGN_CODE = "code_address";
        public const string TABLE_ADDRESS = "address";
        public const string CODE = "code_address";
        public const string ADDRESS = "logradouro";
        public const string CEP = "cep";

        MySqlDataReader reader;
        private const int ERROR = -1;
        private const int NOT_FOUND = 0;

        public string Error_operation { get; set; }

        // Busca se Existe um Endereço Cadastrado no Banco de Dados
        public bool ExistsAddress(int code)
        {
            if (code <= 0)
            {
                Error_operation = "Codigo de Endereço Invalido. O codigo tem que ser" +
                    " um valor Positivo e Diferente de 0";
                return false;
            }

            string count_formatted, command;
            try
            {
                count_formatted = string.Format("COUNT({0})", CODE);
                command = string.Format("SELECT {0} FROM {1} WHERE {2}={3}",
                    count_formatted, TABLE_ADDRESS, CODE, code);
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
                    reader.Read();
                    int quantity = NOT_FOUND;
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
                    Error_operation = "Não foi possivel obter o Verificar o Endereço.";
                    System.Diagnostics.Debug.WriteLine(Error_operation + " Exceção: " + ex);
                    return false;
                }
                finally
                {
                    if (reader != null) reader.Close();
                }
            }
        }

        // Busca o Codigo de um Endereço se for Valido
        public int ReturnCodeAddress(Address address)
        {
            if (address == null)
            {
                Error_operation = "Endereço Invalido. O Preenchimento do Logradouro" +
                    " é Obrigatorio";
                return ERROR;
            } else if (!address.ValidationLogradouro(address.Logradouro))
            {
                Error_operation = address.Error_Validation;
                return ERROR;
            }

            string command;
            try
            {
                command = string.Format("SELECT {0} FROM {1} WHERE {2}='{3}'",
                        CODE, TABLE_ADDRESS, ADDRESS, address.Logradouro);
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

                    // Retorna o Codigo ou 0 (não encontrado)
                    return code;
                }
                catch (IndexOutOfRangeException ex)
                {
                    Error_operation = "Não foi possivel Obter os Dados.";
                    System.Diagnostics.Debug.WriteLine(Error_operation + " Exceção: " + ex);
                    return ERROR;
                }
                catch (Exception ex)
                {
                    Error_operation = "Não foi possivel obter o Codigo do Endereço.";
                    System.Diagnostics.Debug.WriteLine(Error_operation + " Exceção: " + ex);
                    return ERROR;
                }
                finally
                {
                    if (reader != null) reader.Close();
                }
            }
        }

        // Caso o Endereço não exista = Cadastra
        public bool InsertAddress(Address address)
        {
            if (ExistsAddress(ReturnCodeAddress(address))) return false;

            string command;
            try
            {
                command = string.Format("INSERT INTO {0}({1},{2}) VALUE('{3}', '{4}')",
                    TABLE_ADDRESS, ADDRESS, CEP, address.Logradouro, address.Cep);
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

        // Caso o Endereço Exista, Retorna a Classe uma instanciada
        public Address SelectAddress(int code)
        {
            if (!ExistsAddress(code)) return null;

            string command;
            try
            {
                command = string.Format("SELECT * FROM address WHERE {0}={1}", CODE, code);
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

                    Address address = new Address()
                    {
                        Code_address = reader.GetInt32(reader.GetOrdinal(CODE)),
                        Logradouro = reader.GetString(reader.GetOrdinal(ADDRESS)),
                        Cep = reader.GetString(reader.GetOrdinal(CEP))
                    };

                    return address;
                }
                catch(IndexOutOfRangeException ex)
                {
                    Error_operation = "Não foi possivel Obter os Dados.";
                    System.Diagnostics.Debug.WriteLine(Error_operation + " Exceção: " + ex);
                    return null;
                }
                catch (Exception ex)
                {
                    Error_operation = "Não foi possivel Excluir o Endereço.";
                    System.Diagnostics.Debug.WriteLine(Error_operation + " Exceção: " + ex);
                    return null;
                }
                finally
                {
                    if (reader != null) reader.Close();
                }
            }
        }

        // Obtem o Codigo do Endereço. Caso não exista, tenta Inserir
        public int CodeAddressValid(Address address)
        {
            if (address == null)
            {
                Error_operation = "Logradouro não Informado";
                return ERROR;
            }

            // Obtem o Codigo Address
            int code_address = ReturnCodeAddress(address);

            if (code_address == ERROR)
            {
                return ERROR;
            }
            else if (code_address == NOT_FOUND)
            {
                // Caso não exista no Banco de Dados ---> Insere
                if (!InsertAddress(address)) return ERROR;

                // Obtem o codigo do Registro Inserido
                code_address = ReturnCodeAddress(address);
            }

            // Retorna um Erro ou o Codigo Valido
            return code_address <= 0 ? ERROR : code_address;
        }

        // Verifica se o Usuario excluido é o Unico com aquele Endereço
        public bool IsOnlyAddress(Address address)
        {

            if (address == null)
            {
                Error_operation = "Logradouro Invalido.";
                return false;
            }

            address.Code_address = ReturnCodeAddress(address);
            if (address.Code_address <= 0)
            {
                Error_operation = "Codigo do Logradouro não Encontrado. ";
                return false;
            }

            string[] tables_search = new string[2];
            tables_search[0] = ClientDAO.TABLE_CLIENT;
            tables_search[1] = SellerDAO.TABLE_SELLER;

            // Variavel que controlará se é o unico registro
            int isOnlyStateCity = 0;

            // Laço de Repetição com as operações de busca de registro nas 2 tabelas
            foreach (string table in tables_search)
            {
                string command, count_formmated;
                try
                {
                    count_formmated = string.Format("COUNT({0})", NAME_FOREIGN_CODE);
                    command = string.Format("SELECT {0} FROM {1} WHERE {2}={3}",
                    count_formmated, table, NAME_FOREIGN_CODE, address.Code_address);
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

                // Endereço existe no Banco de Dados
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
                        quantity_register = reader.GetInt32(reader.GetOrdinal(count_formmated));

                        // Retorna True (Se for o Unico)
                        if (quantity_register <= 1)
                        {
                            // 1 ou nenhum usuario com aquele codigo
                            isOnlyStateCity++;
                        }
                        else if (isOnlyStateCity > 0)
                        {
                            // +1 usuario com aquele codigo em uma tabela, mas na outra não
                            isOnlyStateCity--;
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
                        Error_operation = "Não foi possivel obter o Verificar do Endereço.";
                        System.Diagnostics.Debug.WriteLine(Error_operation + " Exceção: " + ex);
                        return false;
                    }
                    finally
                    {
                        if (reader != null) reader.Close();
                    }
                }
            }

            return isOnlyStateCity == 2;
        }

        // Exclui o Logradouro se o Usuario for o Unico usando
        public bool DeleteOnlyAddress(Address address)
        {
            if (address == null)
            {
                Error_operation = "Dados Invalidos. Insira o Logradouro Corretamente.";
                return false;
            }

            // Verificar se o Usuario é o unico usando aquele endereço 
            if (IsOnlyAddress(address))
            {
                address.Code_address = ReturnCodeAddress(address);

                if (address.Code_address < 1)
                {
                    Error_operation = "Logradouro não Encontrados no Banco de Dados";
                    return false;
                }

                string command;
                try
                {
                    command = string.Format("DELETE FROM {0} WHERE {1}={2}",
                    TABLE_ADDRESS, CODE, address.Code_address);
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
                        Error_operation = "Não Foi Possivel Excluir o Logradouro. "
                            + database.Error_operation;
                        return false;
                    }
                    else return true;
                }
            }
            else
            {
                // Não Exlcui o usuario pois não é o unico usando o Registro
                Error_operation = string.Empty;
                return false;
            }
        }

        // Atualiza o Logradouro se o Usuario for o Unico usando
        public bool UpdateOnlyAddress(Address oldAddress, Address newAddress)
        {
            if (oldAddress == null || newAddress == null)
            {
                Error_operation = "Logradouro não Informados.";
                return false;
            }

            bool equalsAddress = oldAddress.Equals(newAddress);
            if (equalsAddress)
            {
                return true;
            } else if(!equalsAddress && !string.IsNullOrEmpty(oldAddress.Error_Validation))
            {
                Error_operation = oldAddress.Error_Validation;
                return false;
            }

            int code_newAddress = ReturnCodeAddress(newAddress);

            if (code_newAddress == ERROR)
            {
                return false;
            }
            else if (code_newAddress > 0)
            {
                // Registro novo já existe no Banco de Dados e vê se o Velho somente ele usava
                DeleteOnlyAddress(oldAddress);
                return true;
            }
            else if (IsOnlyAddress(oldAddress))
            {
                // Novo Logradouro não Existe no Banco de Dados
                // Caso Seja o Unico com Aquele Registro ---> Atualiza

                string command;
                try
                {
                    command = string.Format("UPDATE {0} SET {1}='{2}', {3}='{4}' WHERE {5}={6}",
                        TABLE_ADDRESS, ADDRESS, newAddress.Logradouro, CEP, newAddress.Cep,
                        CODE, oldAddress.Code_address);
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
                        Error_operation = "Não Foi Possivel Atualizar o Logradouro. "
                            + database.Error_operation;
                        return false;
                    }
                    else return true;
                }
            }
            else
            {
                // Novo registro não Existe no Banco de Dados e o Registro Velho
                // é Usado por +1 Usuario ---> Insere novo Registro
                return InsertAddress(newAddress);
            }
        }

    }
}
