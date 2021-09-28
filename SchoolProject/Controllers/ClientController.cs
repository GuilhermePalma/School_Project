using SchoolProject.Models;
using SchoolProject.Models.Database.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SchoolProject.Controllers
{
    public class ClientController : Controller
    {
        // GET: Cliente/Index
        [HttpGet]
        public ActionResult Index()
        {
            return View("Login");
        }

        // GET: Cliente/Login
        [HttpGet]
        public ActionResult Login()
        {
            return View("Login");
        }

        // POST: Cliente/Login
        [HttpPost]
        public ActionResult Login(string cpf)
        {
            Client client = new Client()
            {
                Cpf = string.Empty
            };

            // Valida o CPF passado
            bool cpfMask_valid = client.ValidationMaskCPF(cpf);
            bool cpf_valid = client.ValidationCPF(cpf);

            if (cpfMask_valid) client.Cpf = client.RemoveMaskCPF(cpf);
            if (cpf_valid) client.Cpf = cpf;

            if (string.IsNullOrEmpty(client.Cpf))
            {
                ViewBag.Message = "Informe um CPF Valido";
                ViewBag.Erro = client.Error_Validation;
                return View("ResultOperation");
            }

            try
            {
                // Instancia um novo Client e Busca o Usuario no Banco de Dados
                ClientDAO clientDAO = new ClientDAO();
                client = clientDAO.SelectClient(client.Cpf);

                // Usuario Encontrado no Banco de Dados
                if (client != null) return View("Login", client);

                ViewBag.Message = "Usuario não Cadastrado no Sistema";
                ViewBag.Erro = clientDAO.Error_operation;
                return View("ResultOperation");

            }
            catch (Exception ex)
            {
                return View("Error", ex);
            }
        }

        // GET: Cliente/Cadastro
        [HttpGet]
        public ActionResult Cadastro()
        {
            // Disponibiliza uma Lista com os Estados
            ViewBag.Estados = new StateCity().ListStates();

            return View("Register");
        }

        // POST: Cliente/Cadastro
        [HttpPost]
        public ActionResult Cadastro(Client client)
        {
            Address address = new Address();

            bool valid_cpf = client.ValidationMaskCPF(client.Cpf);
            bool valid_phone = client.ValidationMaskPhone(client.Telefone);

            string cpf_formmated = valid_cpf ? 
                client.RemoveMaskCPF(client.Cpf) : string.Empty;
            string phone_formatted = valid_phone ?
                client.RemoveMaskPhone(client.Telefone) : string.Empty;
            string ddd_formatted = valid_phone ? 
                client.RemoveMaskDDD(client.Telefone) : string.Empty;
            string cep_formatted = address.RemoveMaskCEP(client.Cep);

            // Caso algum dado não foi Validado Corretamente
            if (string.IsNullOrEmpty(cpf_formmated) || string.IsNullOrEmpty(phone_formatted) || 
                string.IsNullOrEmpty(ddd_formatted)) 
            {
                ViewBag.Message = "Dados Invalidos";
                ViewBag.Erro = client.Error_Validation;
                return View("ResultOperation");
            } 
            else if (string.IsNullOrEmpty(cep_formatted))
            {
                ViewBag.Message = "CEP Invalido";
                ViewBag.Erro = address.Error_Validation;
                return View("ResultOperation");
            }

            client.Ddd = ddd_formatted;
            client.Telefone = phone_formatted;
            client.Cpf = cpf_formmated;
            client.Cep = cep_formatted;

            ViewBag.Estados = new StateCity().ListStates();

            // Instancia um novo Client e Insere um Usuario no Banco de Dados
            ClientDAO clientDAO = new ClientDAO();

            // Verifica se o Usuario foi Inserido e Redireciona p/ View de Detalhes
            if (clientDAO.InsertClient(client))
            {
                return RedirectToAction("Detalhes", "Client", new { cpf = client.Cpf });
            }
            else
            {
                ViewBag.Message = "Não foi possivel Cadastrar o Usuario no Sistema";
                ViewBag.Erro = clientDAO.Error_operation;
                return View("ResultOperation");
            }
        }

        // GET: Cliente/Detalhes/cpf
        [HttpGet]
        public ActionResult Detalhes(string cpf)
        {
            Client client = new Client()
            {
                Cpf = cpf
            };

            // Caso não seja infromado o CPF
            if (!client.ValidationCPF(client.Cpf))
            {
                ViewBag.Message = "Informe um CPF Valido";
                ViewBag.Erro = client.Error_Validation;
                return View("ResultOperation");
            }

            try
            {
                // Instancia um novo Client e Busca o Usuario no Banco de Dados
                ClientDAO clientDAO = new ClientDAO();
                client = clientDAO.SelectClient(cpf);

                // Usuario Encontrado no Banco de Dados
                if (client != null) return View("Details", client);

                ViewBag.Message = "Usuario não Cadastrado no Sistema";
                ViewBag.Erro = clientDAO.Error_operation;
                return View("ResultOperation");
            }
            catch (Exception ex)
            {
                return View("Error", ex);
            }
        }


        // GET: Cliente/Atualizar/cpf
        [HttpGet]
        public ActionResult Atualizar(string cpf)
        {
            Client client = new Client();
            // Caso não seja infromado o CPF
            if (!client.ValidationCPF(cpf))
            {
                ViewBag.Message = "Informe um CPF Valido";
                ViewBag.Erro = client.Error_Validation;
                return View("ResultOperation");
            }
            else client.Cpf = cpf;

            try
            {
                ClientDAO clientDAO = new ClientDAO();

                // Obtem um Usuario no Banco de Dados. Se não consegue = null
                client = clientDAO.SelectClient(cpf);

                if (client != null)
                {
                    // Disponibiliza uma Lista com os Estados
                    ViewBag.Estados = new StateCity().ListStates();
                    return View("Update", client);
                }
                else
                {
                    ViewBag.Message = "Usuario não Cadastrado no Sistema";
                    ViewBag.Erro = clientDAO.Error_operation;
                    return View("ResultOperation");
                }
            }
            catch (Exception ex)
            {
                return View("Error", ex);
            }
        }

        // POST: Cliente/Atualizar
        [HttpPost]
        public ActionResult Atualizar(Client client)
        {
            Address address = new Address();

            bool valid_cpf = client.ValidationCPF(client.Cpf);
            bool valid_phone = client.ValidationMaskPhone(client.Telefone);

            string phone_formatted = valid_phone ?
                client.RemoveMaskPhone(client.Telefone) : string.Empty;
            string ddd_formatted = valid_phone ?
                client.RemoveMaskDDD(client.Telefone) : string.Empty;
            string cep_formatted = address.RemoveMaskCEP(client.Cep);

            // Caso algum dado não foi Validado Corretamente
            if (!valid_cpf || string.IsNullOrEmpty(phone_formatted) ||
                string.IsNullOrEmpty(ddd_formatted))
            {
                ViewBag.Message = "Dados Invalidos";
                ViewBag.Erro = client.Error_Validation;
                return View("ResultOperation");
            } 
            else if (string.IsNullOrEmpty(cep_formatted)) 
            {
                ViewBag.Message = "CEP Invalido";
                ViewBag.Erro = address.Error_Validation;
                return View("ResultOperation");
            }

            client.Ddd = ddd_formatted;
            client.Telefone = phone_formatted;
            client.Cep = cep_formatted;

            try
            {
                ClientDAO clientDAO = new ClientDAO();
                bool is_update_client = clientDAO.UpdateClient(client);

                if (is_update_client)
                    return RedirectToAction("Detalhes", "Client", new { Cpf = client.Cpf });

                ViewBag.Message = "Não foi possivel Atualizar o Usuario no Sistema";
                ViewBag.Erro = clientDAO.Error_operation;
                return View("ResultOperation");
            }
            catch (Exception ex)
            {
                return View("Error" + ex);
            }
        }

        // GET: Cliente/Excluir/cpf
        [HttpGet]
        public ActionResult Excluir(string cpf)
        {
            Client client = new Client();
            // Caso não seja infromado o CPF
            if (!client.ValidationCPF(cpf))
            {
                ViewBag.Message = "Informe um CPF Valido";
                ViewBag.Erro = client.Error_Validation;
                return View("ResultOperation");
            }
            else client.Cpf = cpf;

            try
            {
                ClientDAO clientDAO = new ClientDAO();

                // Obtem um Usuario no Banco de Dados. Se não consegue = null
                client = clientDAO.SelectClient(cpf);

                if (client != null) return View("Delete", client);

                ViewBag.Message = "Não foi possivel Atualizar o Usuario no Sistema";
                ViewBag.Erro = clientDAO.Error_operation;
                return View("ResultOperation");

            }
            catch (Exception ex)
            {
                return View("Error", ex);
            }
        }

        // POST: Cliente/Excluir
        [HttpPost]
        public ActionResult Excluir(Client client)
        {
            // Caso não seja infromado o CPF
            if (!client.ValidationCPF(client.Cpf))
            {
                ViewBag.Message = "Informe um CPF Valido";
                ViewBag.Erro = client.Error_Validation;
                return View("ResultOperation");
            }

            try
            {
                ClientDAO clientDAO = new ClientDAO();

                bool is_deleted_client = clientDAO.DeleteClient(client.Cpf);
                if (is_deleted_client)
                {
                    ViewBag.Message = "Usuario Excluido com Suceso !";
                    return View("ResultOperation");
                }

                // Caso o Usuario não Atualize ou Não Obteve um Select do Usuario
                ViewBag.Message = "Usuario não Excluido do Sistema";
                ViewBag.Erro = clientDAO.Error_operation;
                return View("ResultOperation");
            }
            catch (Exception ex)
            {
                return View("Error" + ex);
            }
        }


        [ActionName("Cadastrados")]
        public ActionResult ListClient()
        {
            try
            {
                ClientDAO clientDAO = new ClientDAO();

                // Obtem uma List<Seller> com os elementos do Banco de Dados
                IEnumerable<Client>  listClients = clientDAO.ListClients();
                if (listClients != null && listClients.Any())
                {
                    return View("ListClient", listClients);
                }

                // Caso não consiga recuperar os vendedores do banco de dados
                ViewBag.Message = "Lista de Funcionarios não Disponivel";
                ViewBag.Erro = clientDAO.Error_operation;
                return View("ResultOperation");

            }
            catch (ArgumentNullException ex)
            {
                // Caso a Lista seja Nula
                ViewBag.Message = "Lista de Funcionarios não Disponivel";
                ViewBag.Erro = "Não foi possivel obter a Lista de Vendedores";
                System.Diagnostics.Debug.WriteLine("Lista de Funcionarios Nula. Exceção: " + ex);
                return View("ResultOperation");
            }
        }

    }
}
