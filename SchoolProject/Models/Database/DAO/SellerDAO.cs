using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace SchoolProject.Models.Database.DAO
{
    public class SellerDAO
    {
        public const string TABLE_SELLER = "seller";
        public const string CNPJ = "cnpj";
        public const string NAME = "name";
        public const string STATE_CITY = "code_state_city";
        public const string ADDRESS = "code_address";
        public const string NUMBER = "residential_number";
        public const string COMPLEMENT = "complement";
        public const string PHONE = "phone";
        public const string DDD = "ddd";

        private MySqlDataReader reader;
        private const int ERROR = -1;
        private const int NOT_FOUND = 0;

        public string Error_operation { get; set; }

        // Verifica se um Vendedor existe no Banco de Dados
        public bool ExistsSeller(string cnpj)
        {
            Seller seller = new Seller();
            if (!seller.ValidationCNPJ(cnpj))
            {
                Error_operation = seller.Error_Validation;
                return false;
            }

            string count_formatted, command;
            try
            {
                count_formatted = string.Format("COUNT({0})", CNPJ);
                command = string.Format("SELECT {0} FROM {1} WHERE {2}='{3}'",
                        count_formatted, TABLE_SELLER, CNPJ, cnpj);
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

                try
                {
                    if(reader != null)
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
                        else if (quantity == 1) return true;
                    }

                    // Caso o reader == null ou quantity < 1
                    Error_operation = "Registro não Encontrado no Banco de Dados";
                    return false;

                }
                catch (IndexOutOfRangeException ex)
                {
                    Error_operation = "Não foi possivel Obter os Dados.";
                    System.Diagnostics.Debug.WriteLine(Error_operation + " Exceção: " + ex);
                    return false;
                }
                catch (Exception ex)
                {
                    Error_operation = "Não foi possivel obter o Verificar o Vendedor.";
                    System.Diagnostics.Debug.WriteLine(Error_operation + " Exceção: " + ex);
                    return false;
                }
                finally
                {
                    if (reader != null) reader.Close();
                }
            }
        }

        // Insere um Vendedor (Com dados Normalizados) se for Validado e não Existir
        public bool InsertSeller(Seller seller)
        {
            if (seller == null)
            {
                Error_operation = "Vendedor não Informado";
                return false;
            }
            else if (ExistsSeller(seller.Cnpj))
            {
                Error_operation = "Vendedor já Cadastrado no Sistema";
                return false;
            }

            // Intancia das Classes de Endereço
            StateCity stateCity = new StateCity()
            {
                Estado = seller.Estado,
                Cidade = seller.Cidade
            };

            Address address = new Address()
            {
                Logradouro = seller.Logradouro,
                Cep = seller.Cep
            };

            Phone phone = new Phone()
            {
                Telefone = seller.Telefone,
                Ddd = seller.Ddd
            };

            StateCityDAO stateCityDAO = new StateCityDAO();
            AddressDAO addressDAO = new AddressDAO();

            // Metodos responsaveis por Buscar/Inserir (Se não Existir) o Estado/Cidade/Endereço
            int code_state_city = stateCityDAO.CodeStateCityValid(stateCity);
            int code_address = addressDAO.CodeAddressValid(address);

            if (code_state_city == ERROR || code_state_city == NOT_FOUND) 
            {
                Error_operation = stateCityDAO.Error_operation;
                return false; 
            }
            else if (code_address == ERROR || code_address == NOT_FOUND) 
            {
                Error_operation = addressDAO.Error_operation;
                return false;
            }
            else if (!phone.ValidationPhone(phone.Telefone))
            {
                Error_operation = phone.Error_Validation;
                return false;
            }

            string command;
            try
            {
                command = string.Format("INSERT INTO {0}({1},{2},{3},{4},{5},{6},{7},{8}) " +
                    "VALUE('{9}', '{10}', {11}, {12}, {13}, '{14}', '{15}', '{16}')",
                    TABLE_SELLER, CNPJ, NAME, STATE_CITY, ADDRESS, NUMBER, COMPLEMENT, 
                    PHONE, DDD, seller.Cnpj, seller.Name, code_state_city, code_address, 
                    seller.Numero, seller.Complemento, phone.Telefone, phone.Ddd);
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

        // Exlcui o Vendedor se Existir
        public bool DeleteSeller(string cnpj)
        {
            Seller seller = SelectSeller(cnpj);
            if (seller == null) return false;

            // Verificar se o Usuario é o unico usando aquele endereço ---> Exclui
            AddressDAO addressDAO = new AddressDAO();
            StateCityDAO stateCityDAO = new StateCityDAO();
            Address address = new Address()
            {
                Logradouro = seller.Logradouro, 
                Cep = seller.Cep
            };

            StateCity stateCity = new StateCity()
            {
                Estado = seller.Estado,
                Cidade = seller.Cidade,
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
                    TABLE_SELLER, CNPJ, cnpj);
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

        // Atualiza (Com dados Normalizados) um Vendedor caso Exista
        public bool UpdateSeller(Seller seller)
        {
            if (seller == null)
            {
                Error_operation = "Vendedor não Informado";
                return false;
            }
            else if (!ExistsSeller(seller.Cnpj)) return false;

            // Obtem a Cidade/Estado/Endereço antes de Atualizar
            Seller oldSeller = SelectSeller(seller.Cnpj);

            if (oldSeller == null) return false;

            StateCityDAO stateCityDAO = new StateCityDAO();
            AddressDAO addressDAO = new AddressDAO();


            // Usado para obter o Antigo Codigo do Estado/Cidade
            StateCity oldStateCity = new StateCity()
            {
                Estado = oldSeller.Estado,
                Cidade = oldSeller.Cidade
            };
            StateCity newStateCity = new StateCity()
            {
                Estado = seller.Estado,
                Cidade = seller.Cidade
            };
            Address oldAddress = new Address()
            {
                Logradouro = oldSeller.Logradouro,
                Cep = oldSeller.Cep
            };
            Address newAddress = new Address()
            {
                Logradouro = seller.Logradouro,
                Cep = seller.Cep
            };

            Phone phone = new Phone()
            {
                Telefone = seller.Telefone,
                Ddd = seller.Ddd
            };

            // Verificar se o Usuario é o unico usando aquele Estado/Cidade/Logradouro
            if (!stateCityDAO.UpdateOnlyStateCity(oldStateCity, newStateCity))
            {
                Error_operation = stateCityDAO.Error_operation;
                return false;
            }
            else if(!addressDAO.UpdateOnlyAddress(oldAddress, newAddress))
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
                } else if (code_address < 1)
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
                    TABLE_SELLER, NAME, seller.Name, STATE_CITY, newStateCity.Code_stateCity, 
                    ADDRESS, newAddress.Code_address, NUMBER, seller.Numero, COMPLEMENT,
                    seller.Complemento, PHONE, phone.Telefone, DDD, phone.Ddd, CNPJ, seller.Cnpj);
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

            // Codigos ja obtidos (Inseridos ou Atualizados ou Obtidos) --> Atualiza o Vendedor
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

        // Retorna um User se o Vendedor existir e obter seus Dados
        public Seller SelectSeller(string cnpj)
        {
            if (!ExistsSeller(cnpj)) return null;

            string command;
            try
            {
                command = string.Format("SELECT * FROM {0} WHERE {1}='{2}'",
                    TABLE_SELLER, CNPJ, cnpj);
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
            Seller sellerDatabase;
            Phone phone;

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
                    reader.Read();

                    sellerDatabase = new Seller()
                    {
                        Cnpj = reader.GetString(reader.GetOrdinal(CNPJ)),
                        Name = reader.GetString(reader.GetOrdinal(NAME)),
                        Numero = reader.GetInt32(reader.GetOrdinal(NUMBER)),
                    };

                    if (!reader.IsDBNull(reader.GetOrdinal(COMPLEMENT)))
                    {
                        sellerDatabase.Complemento = reader.GetString(reader.GetOrdinal(COMPLEMENT));
                    }

                    phone = new Phone()
                    {
                        Telefone = reader.GetString(reader.GetOrdinal(PHONE)),
                        Ddd = reader.GetString(reader.GetOrdinal(DDD))
                    };

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
                    Error_operation = "Não foi Possivel Selecionar o Vendedor no Banco de dados.";
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
            else if(phone == null)
            {
                Error_operation = "Não foi Possivel obter o Telefone";
            }
            else
            {
                sellerDatabase.Cidade = stateCity.Cidade;
                sellerDatabase.Estado = stateCity.Estado;
                sellerDatabase.Logradouro = addressClass.Logradouro;
                sellerDatabase.Cep = addressClass.Cep;
                sellerDatabase.Telefone = phone.Telefone;
                sellerDatabase.Ddd= phone.Ddd;
            }
            return sellerDatabase;
        }

        public List<Seller> ListSellers()
        {
            string command;
            try
            {
                command = string.Format("SELECT * FROM {0}", TABLE_SELLER);
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
                    List<Seller> listSeller = new List<Seller>();

                    while (reader.Read())
                    {
                        Seller sellerDatabase = new Seller()
                        {
                            Cnpj = reader.GetString(reader.GetOrdinal(CNPJ)),
                            Name = reader.GetString(reader.GetOrdinal(NAME)),
                            Numero = reader.GetInt32(reader.GetOrdinal(NUMBER))
                        };

                        if (!reader.IsDBNull(reader.GetOrdinal(COMPLEMENT)))
                        {
                            sellerDatabase.Complemento = reader.GetString(reader.GetOrdinal(COMPLEMENT));
                        }

                        code_state_city = reader.GetInt32(reader.GetOrdinal(STATE_CITY));
                        code_address = reader.GetInt32(reader.GetOrdinal(ADDRESS));

                        Phone phone = new Phone()
                        {
                            Telefone = reader.GetString(reader.GetOrdinal(PHONE)),
                            Ddd = reader.GetString(reader.GetOrdinal(DDD))
                        };

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
                        else if (phone == null)
                        {
                            Error_operation = "Não foi Possivel obter o Telefone";
                        }
                        else
                        {
                            sellerDatabase.Cidade = stateCity.Cidade;
                            sellerDatabase.Estado = stateCity.Estado;
                            sellerDatabase.Logradouro = addressClass.Logradouro;
                            sellerDatabase.Cep = addressClass.Cep;
                            sellerDatabase.Telefone = phone.Telefone;
                            sellerDatabase.Ddd = phone.Ddd;
                        }
                        listSeller.Add(sellerDatabase);
                    }

                    return listSeller;
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