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
        private const string INVALID_CPF = "CPF não Informado ou Incorreto. " +
                    "CPF deve conter apenas 11 Numeros ";

        // GET: Cliente/Index
        [HttpGet]
        public ActionResult Index()
        {
            return View("Login");
        }

        // POST: Cliente/Login
        [HttpPost]
        public ActionResult Login(string cpf)
        {
            // Caso não seja infromado o CPF
            if (!Client.ValidationCPF(cpf))
            {
                ViewBag.Message = "Informe um CPF Valido";
                ViewBag.Erro = INVALID_CPF;
                return View("ResultOperation");
            }

            try
            {
                // Instancia um novo Client e Busca o Usuario no Banco de Dados
                ClientDAO clientDAO = new ClientDAO();
                Client clientDatabase = clientDAO.SelectClient(cpf);

                // Usuario Encontrado no Banco de Dados
                if (clientDatabase != null) return View("Login", clientDatabase);

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
            ViewBag.Estados = new StateCity().listStates();

            return View("Register");
        }

        // POST: Cliente/Cadastro
        [HttpPost]
        public ActionResult Cadastro(Client client)
        {
            // Caso não seja infromado o CPF
            if (!Client.ValidationCPF(client.Cpf))
            {
                ViewBag.Message = "Informe um CPF Valido";
                ViewBag.Erro = INVALID_CPF;
                return View("ResultOperation");
            }

            ViewBag.Estados = new StateCity().listStates();

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
            // Caso não seja infromado o CPF
            if (!Client.ValidationCPF(cpf))
            {
                ViewBag.Message = "Informe um CPF Valido";
                ViewBag.Erro = INVALID_CPF;
                return View("ResultOperation");
            }

            try
            {
                // Instancia um novo Client e Busca o Usuario no Banco de Dados
                ClientDAO clientDAO = new ClientDAO();

                Client clientDatabase = new Client();
                clientDatabase = clientDAO.SelectClient(cpf);

                // Usuario Encontrado no Banco de Dados
                if (clientDatabase != null) return View("Details", clientDatabase);

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
            // Caso não seja infromado o CPF
            if (!Client.ValidationCPF(cpf))
            {
                ViewBag.Message = "Informe um CPF Valido";
                ViewBag.Erro = INVALID_CPF;
                return View("ResultOperation");
            }

            try
            {
                ClientDAO clientDAO = new ClientDAO();

                // Obtem um Usuario no Banco de Dados. Se não consegue = null
                Client client = clientDAO.SelectClient(cpf);

                if (client != null)
                {
                    // Disponibiliza uma Lista com os Estados
                    ViewBag.Estados = new StateCity().listStates();
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
            // Caso não seja infromado o CPF
            if (!Client.ValidationCPF(client.Cpf))
            {
                ViewBag.Message = "Informe um CPF Valido";
                ViewBag.Erro = INVALID_CPF;
                return View("ResultOperation");
            }

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
            // Caso não seja infromado o CPF
            if (!Client.ValidationCPF(cpf))
            {
                ViewBag.Message = "Informe um CPF Valido";
                ViewBag.Erro = INVALID_CPF;
                return View("ResultOperation");
            }

            try
            {
                ClientDAO clientDAO = new ClientDAO();

                // Obtem um Usuario no Banco de Dados. Se não consegue = null
                Client client = clientDAO.SelectClient(cpf);

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
            if (!Client.ValidationCPF(client.Cpf))
            {
                ViewBag.Message = "Informe um CPF Valido";
                ViewBag.Erro = INVALID_CPF;
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
            ClientDAO clientDAO = new ClientDAO();
            IEnumerable<Client> listClients;
            try
            {
                // Obtem uma List<Seller> com os elementos do Banco de Dados
                listClients = clientDAO.ListClients();
                if (listClients != null || listClients.Count() > 0)
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
