using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolProject.Models.Database.DAO
{
    public class ProductDAO
    {
        public const string TABLE_PRODUCTS = "products";
        public const string ID_PRODUCT = "id_product";
        public const string CNPJ_SELLER = "cnpj_seller";
        public const string NAME_PRODUCT = "name";
        public const string STATE_CITY_PRODUCT = "code_state_city";
        public const string ADDRESS_PRODUCT = "code_address";
        public const string QUANTITY = "quantity";
        public const string DESCRIPTION = "description";
        public const string PRICE = "price";

        private const int ERROR = -1;
        private const int NOT_FOUND = 0;

        public string Error_operation { get; set; }

        // Verifica se um Vendedor possui aquele  Produto no Banco de Dados
        public bool ExistsProduct(string name, string cnpj_seller)
        {
            Seller seller = new Seller();
            if (!seller.ValidationCNPJ(cnpj_seller))
            {
                Error_operation = seller.Error_Validation;
                return false;
            } else if (string.IsNullOrEmpty(name))
            {
                Error_operation = "Nome do Produto não Informado";
                return false;
            }

            using (Database database = new Database())
            {
                if (!database.IsAvalibleDatabase)
                {
                    Error_operation = database.Error_operation;
                    return false;
                }

                string count_formatted = database.FormattedSQL("COUNT({0})", new string[] { NAME_PRODUCT });
                string format = "SELECT {0} FROM {1} WHERE {2}='{3}' AND {4}='{5}'";
                string[] parameters = new string[]  
                {
                    count_formatted, TABLE_PRODUCTS, CNPJ_SELLER, cnpj_seller, NAME_PRODUCT, name
                };

                string command = string.IsNullOrEmpty(count_formatted) ? string.Empty :
                    database.FormattedSQL(format, parameters);
                if (string.IsNullOrEmpty(command))
                { 
                    Error_operation = database.Error_operation;
                    return false;
                }

                MySqlDataReader dataReader = database.ReaderTable(command);
                if (dataReader == null)
                {
                    Error_operation = database.Error_operation;
                    return false;
                }

                try
                {
                    int quantity = NOT_FOUND;
                    dataReader.Read();
                    quantity = dataReader.GetInt32(dataReader.GetOrdinal(count_formatted));

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
                    if (dataReader != null) dataReader.Close();
                }
            }
        }

        // Insere um Usuario (Com dados Normalizados) se for Validado e não Existir
        public bool InsertProduct(Product product, string cnpj_seller)
        {
            if (product == null)
            {
                Error_operation = "Produto não Informado";
                return false;
            }
            else if (ExistsProduct(product.Name, cnpj_seller))
            {
                Error_operation = "Produto já Cadastrado no Sistema";
                return false;
            }

            // Intancia das Classes de Endereço
            StateCity stateCity = new StateCity()
            {
                Estado = product.Estado,
                Cidade = product.Cidade
            };

            Address address = new Address()
            {
                Logradouro = product.Logradouro,
                Cep = product.Cep
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

            using (Database database = new Database())
            {
                if (!database.IsAvalibleDatabase)
                {
                    Error_operation = database.Error_operation;
                    return false;
                }

                string format = "INSERT INTO {0}({1},{2},{3},{4},{5},{6},{7}) " +
                    "VALUE('{8}', '{9}', {10}, '{11}', {12}, {13}, {14})";
                string[] parameters = new string[]
                {
                    TABLE_PRODUCTS, CNPJ_SELLER, NAME_PRODUCT, QUANTITY, DESCRIPTION, PRICE,
                    STATE_CITY_PRODUCT, ADDRESS_PRODUCT, cnpj_seller, product.Name,
                    product.Quantity.ToString(), product.Description, product.Price.ToString(),
                    code_state_city.ToString(), code_address.ToString()
                };

                string command = database.FormattedSQL(format, parameters);
                if (string.IsNullOrEmpty(command))
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
        public bool DeleteProduct(string name_product, string cnpj_seller)
        {
            Product product = new Product();
            product.Name = name_product;
            product.Cnpj_Seller = cnpj_seller;
            product.Id_product = ReturnIdProduct(product);

            if (product.Id_product < 1) return false;
            else product = SelectProduct(product.Id_product);

            if (product == null) return false;

            // Verificar se o Usuario é o unico usando aquele endereço ---> Exclui
            AddressDAO addressDAO = new AddressDAO();
            StateCityDAO stateCityDAO = new StateCityDAO();
            Address address = new Address()
            {
                Logradouro = product.Logradouro,
                Cep = product.Cep
            };

            StateCity stateCity = new StateCity()
            {
                Estado = product.Estado,
                Cidade = product.Cidade,
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

            using (Database database = new Database())
            {
                if (!database.IsAvalibleDatabase)
                {
                    Error_operation = database.Error_operation;
                    return false;
                }

                string format = "DELETE FROM {0} WHERE {1}='{2}'";
                string command = database.FormattedSQL(format, new string[] {
                    TABLE_PRODUCTS, ID_PRODUCT, product.Id_product.ToString() });

                if (string.IsNullOrEmpty(command))
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
        public bool UpdateProduct(Product product, string seller_cnpj)
        {
            if (product == null)
            {
                Error_operation = "Produto não Informado";
                return false;
            }

            // Obtem a Cidade/Estado/Endereço antes de Atualizar
            Product oldProduct = SelectProduct(product.Id_product);
            if (oldProduct == null) return false;

            StateCityDAO stateCityDAO = new StateCityDAO();
            AddressDAO addressDAO = new AddressDAO();

            // Usado para obter o Antigo Codigo do Estado/Cidade
            StateCity oldStateCity = new StateCity()
            {
                Estado = oldProduct.Estado,
                Cidade = oldProduct.Cidade
            };
            StateCity newStateCity = new StateCity()
            {
                Estado = product.Estado,
                Cidade = product.Cidade
            };
            Address oldAddress = new Address()
            {
                Logradouro = oldProduct.Logradouro,
                Cep = oldProduct.Cep
            };
            Address newAddress = new Address()
            {
                Logradouro = product.Logradouro,
                Cep = product.Cep
            };

            // Verificar se o Usuario é o unico usando aquele Estado/Cidade/Logradouro
            if (!stateCityDAO.UpdateOnlyStateCity(oldStateCity, newStateCity))
            {
                Error_operation = stateCityDAO.Error_operation;
                return false;
            }
            else if (!addressDAO.UpdateOnlyAddress(oldAddress, newAddress))
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

            // Codigos ja obtidos (Inseridos ou Atualizados ou Obtidos) --> Atualiza o Usuario
            using (Database database = new Database())
            {
                if (!database.IsAvalibleDatabase)
                {
                    Error_operation = database.Error_operation;
                    return false;
                }

                string format = "UPDATE {0} SET {1}='{2}', {3}={4}, {5}={6}, " +
                    "{7}={8}, {9}='{10}', {11}={12} WHERE {13}={14}";
                string[] parameters = new string[]
                {
                    TABLE_PRODUCTS, NAME_PRODUCT, product.Name, STATE_CITY_PRODUCT,
                    newStateCity.Code_stateCity.ToString(), ADDRESS_PRODUCT,
                    newAddress.Code_address.ToString(), QUANTITY, product.Quantity.ToString(),
                    DESCRIPTION, product.Description, PRICE, product.Price.ToString(),
                    ID_PRODUCT, product.Id_product.ToString()
                };

                string command = database.FormattedSQL(format, parameters);
                if (string.IsNullOrEmpty(command))
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
        public Product SelectProduct(int id_product)
        {
            if(id_product < 1)
            {
                Error_operation = "Codigo do Produto Invalido.";
                return null;
            }

            int code_state_city = 0, code_address = 0;
            Product productDatabase;

            // Usuario existe no Banco de Dados
            using (Database database = new Database())
            {
                if (!database.IsAvalibleDatabase)
                {
                    Error_operation = database.Error_operation;
                    return null;
                }

                string format = "SELECT * FROM {0} WHERE {1}='{2}'";

                string command = database.FormattedSQL(format,
                    new string[] { TABLE_PRODUCTS, ID_PRODUCT, id_product.ToString() });
                if (string.IsNullOrEmpty(command))
                {
                    Error_operation = database.Error_operation;
                    return null;
                }

                MySqlDataReader dataReader = database.ReaderTable(command);
                if (dataReader == null)
                {
                    Error_operation = database.Error_operation;
                    return null;
                }
                try
                {
                    dataReader.Read();
                    productDatabase = new Product()
                    {
                        Name = dataReader.GetString(dataReader.GetOrdinal(NAME_PRODUCT)),
                        Quantity = dataReader.GetInt32(dataReader.GetOrdinal(QUANTITY)),
                        Price = dataReader.GetString(dataReader.GetOrdinal(PRICE)),
                        Description = dataReader.GetString(dataReader.GetOrdinal(DESCRIPTION)),
                        Id_product = dataReader.GetInt32(dataReader.GetOrdinal(ID_PRODUCT)),
                        Cnpj_Seller = dataReader.GetString(dataReader.GetOrdinal(CNPJ_SELLER))
                    };
                    
                    code_state_city = dataReader.GetInt32(dataReader.GetOrdinal(STATE_CITY_PRODUCT));
                    code_address = dataReader.GetInt32(dataReader.GetOrdinal(ADDRESS_PRODUCT));
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
                    if (dataReader != null) dataReader.Close();
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
                productDatabase.Cidade = stateCity.Cidade;
                productDatabase.Estado = stateCity.Estado;
                productDatabase.Logradouro = addressClass.Logradouro;
                productDatabase.Cep = addressClass.Cep;
            }

            return productDatabase;
        }

        // Retorna o ID de um Produto a partir do Nome do produto + CNPJ do Vendedor
        public int ReturnIdProduct(Product product)
        {
            if (product == null)
            {
                Error_operation = "Informações do Produto Não Informado.";
                return ERROR;
            }
            else if (string.IsNullOrEmpty(product.Name))
            {
                Error_operation = "Nome do Produto Não Informado.";
                return ERROR;
            }
            else if (string.IsNullOrEmpty(product.Cnpj_Seller))
            {
                Error_operation = "CNPJ do Vendedor Não Informado.";
                return ERROR;
            }

            using (Database database = new Database())
            {
                if (!database.IsAvalibleDatabase)
                {
                    Error_operation = database.Error_operation;
                    return ERROR;
                }

                string format = "SELECT {0} FROM {1} WHERE {2}='{3}' AND {4}='{5}'";
                string[] parameters = new string[]
                {
                    ID_PRODUCT, TABLE_PRODUCTS, NAME_PRODUCT, product.Name, CNPJ_SELLER, product.Cnpj_Seller
                };

                string command = database.FormattedSQL(format, parameters);
                if (string.IsNullOrEmpty(command))
                {
                    Error_operation = database.Error_operation;
                    return ERROR;
                }

                MySqlDataReader dataReader = database.ReaderTable(command);
                if (dataReader == null)
                {
                    Error_operation = database.Error_operation;
                    return NOT_FOUND;
                }

                try
                {
                    dataReader.Read();
                    return dataReader.GetInt32(dataReader.GetOrdinal(ID_PRODUCT));
                }
                catch (Exception ex)
                {
                    Error_operation = "Não foi Possivel Sel ecionar o Vendedor no Banco de dados.";
                    System.Diagnostics.Debug.WriteLine(Error_operation + " Exceção: " + ex);
                    return ERROR;
                }
                finally
                {
                    if (dataReader != null) dataReader.Close();
                }
            }
        }

        // Lista os Usuarios Cadastrados no Banco de Dados
        public List<Product> ListProducts()
        {
            // Vendedor existe no Banco de Dados
            using (Database database = new Database())
            {
                if (!database.IsAvalibleDatabase)
                {
                    Error_operation = database.Error_operation;
                    return null;
                }

                string format = "SELECT * FROM {0}";

                string command = database.FormattedSQL(format, new string[] { TABLE_PRODUCTS });
                if (string.IsNullOrEmpty(command))
                {
                    Error_operation = database.Error_operation;
                    return null;
                }

                MySqlDataReader dataReader = database.ReaderTable(command);
                if (dataReader == null)
                {
                    Error_operation = database.Error_operation;
                    return null;
                }

                try
                {
                    List<Product> listClients = new List<Product>();
                    int code_state_city = NOT_FOUND, code_address = NOT_FOUND;

                    while (dataReader.Read())
                    {
                        Product productDatabase = new Product()
                        {
                            Name = dataReader.GetString(dataReader.GetOrdinal(NAME_PRODUCT)),
                            Quantity = dataReader.GetInt32(dataReader.GetOrdinal(QUANTITY)),
                            Price = dataReader.GetString(dataReader.GetOrdinal(PRICE)),
                            Description = dataReader.GetString(dataReader.GetOrdinal(DESCRIPTION)),
                            Id_product = dataReader.GetInt32(dataReader.GetOrdinal(ID_PRODUCT)),
                            Cnpj_Seller = dataReader.GetString(dataReader.GetOrdinal(CNPJ_SELLER))
                        };

                        code_state_city = dataReader.GetInt32(dataReader.GetOrdinal(STATE_CITY_PRODUCT));
                        code_address = dataReader.GetInt32(dataReader.GetOrdinal(ADDRESS_PRODUCT));

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
                            productDatabase.Cidade = stateCity.Cidade;
                            productDatabase.Estado = stateCity.Estado;
                            productDatabase.Logradouro = addressClass.Logradouro;
                            productDatabase.Cep = addressClass.Cep;
                        }

                        listClients.Add(productDatabase);
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
                    Error_operation = "Não foi Possivel Selecionar o Produto no Banco de dados.";
                    System.Diagnostics.Debug.WriteLine(Error_operation + " Exceção: " + ex);
                    return null;
                }
                finally
                {
                    if (dataReader != null) dataReader.Close();
                }
            }
        }

        // Lista os Usuarios Cadastrados no Banco de Dados
        public List<Product> ListProductsSeller(string cnpj_seller)
        {
            // Vendedor existe no Banco de Dados
            using (Database database = new Database())
            {
                if (!database.IsAvalibleDatabase)
                {
                    Error_operation = database.Error_operation;
                    return null;
                }

                string format = "SELECT * FROM {0} WHERE {1}='{2}'";

                string command = database.FormattedSQL(format, 
                    new string[] { TABLE_PRODUCTS, CNPJ_SELLER, cnpj_seller });
                if (string.IsNullOrEmpty(command))
                {
                    Error_operation = database.Error_operation;
                    return null;
                }


                MySqlDataReader dataReader = database.ReaderTable(command);
                if (dataReader == null)
                {
                    Error_operation = "Vendedor não Possui Produtos Cadastrados. \n" 
                        + database.Error_operation;
                    return null;
                }

                try
                {
                    List<Product> listClients = new List<Product>();
                    while (dataReader.Read())
                    {
                        // Re-atribui o Valor padrão em cada Produto
                        string seller_cnpj = string.Empty;
                        int code_state_city = NOT_FOUND, code_address = NOT_FOUND;

                        Product productDatabase = new Product()
                        {
                            Name = dataReader.GetString(dataReader.GetOrdinal(NAME_PRODUCT)),
                            Quantity = dataReader.GetInt32(dataReader.GetOrdinal(QUANTITY)),
                            Price = dataReader.GetString(dataReader.GetOrdinal(PRICE)),
                            Description = dataReader.GetString(dataReader.GetOrdinal(DESCRIPTION)),
                            Id_product = dataReader.GetInt32(dataReader.GetOrdinal(ID_PRODUCT)),
                            Cnpj_Seller = dataReader.GetString(dataReader.GetOrdinal(CNPJ_SELLER))
                        };

                        seller_cnpj = dataReader.GetString(dataReader.GetOrdinal(CNPJ_SELLER));
                        code_state_city = dataReader.GetInt32(dataReader.GetOrdinal(STATE_CITY_PRODUCT));
                        code_address = dataReader.GetInt32(dataReader.GetOrdinal(ADDRESS_PRODUCT));

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
                            productDatabase.Cidade = stateCity.Cidade;
                            productDatabase.Estado = stateCity.Estado;
                            productDatabase.Logradouro = addressClass.Logradouro;
                            productDatabase.Cep = addressClass.Cep;
                        }

                        listClients.Add(productDatabase);
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
                    if (dataReader != null) dataReader.Close();
                }
            }
        }


    }
}