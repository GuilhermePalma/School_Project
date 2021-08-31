using SchoolProject.Models;
using SchoolProject.Models.Database.DAO;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace SchoolProject.Controllers
{
    public class ClientController : Controller
    {

        // GET: Client/Index
        [HttpGet]
        public ActionResult Index()
        {
            return View("Login");
        }

        // POST: Client/Login
        [HttpPost]
        public ActionResult Login(string cpf)
        {
            // Caso não seja infromado o CPF
            if (!new Client().validationCPF(cpf))
            {
                ViewBag.Message = "Informe um CPF Valido";
                ViewBag.Erro = "CPF não Informado ou Incorreto. " +
                    "Informe o CPF para Realizar a Operação";
                return View("ResultOperation");
            }

            try
            {
                // Instancia um novo Client e Busca o Usuario no Banco de Dados
                ClientDAO clientDAO = new ClientDAO();
                Client clientDatabase = clientDAO.selectClient(cpf);

                // Usuario Encontrado no Banco de Dados
                if (clientDatabase != null) return View("Login", clientDatabase);

                ViewBag.Message = "Usuario não Cadastrado no Sistema";
                ViewBag.Erro = clientDAO.error_operation;
                return View("ResultOperation");

            }
            catch (Exception ex)
            {
                return View("Error", ex);
            }
        }

        // GET: Client/Cadastro
        [HttpGet]
        public ActionResult Cadastro()
        {
            // Disponibiliza uma Lista com os Estados
            ViewBag.Estados = new StateCity().listStates();

            return View("Register");
        }

        // POST: Client/Cadastro
        [HttpPost]
        public ActionResult Cadastro(Client client)
        {
            try
            {
                // Caso não seja infromado o CPF
                if (!client.validationCPF(client.Cpf))
                {
                    ViewBag.Message = "Informe um CPF Valido";
                    ViewBag.Erro = "CPF não Informado ou Incorreto. " +
                        "Informe o CPF para Realizar a Operação";
                    return View("ResultOperation");
                }

                ViewBag.Estados = new StateCity().listStates();

                // Instancia um novo Client e Insere um Usuario no Banco de Dados
                ClientDAO clientDAO = new ClientDAO();

                if (clientDAO.existsClient(client.Cpf))
                {
                    ViewBag.Message = "Não foi Possivel Cadastrar o Usuario. " +
                        "Usuario já existente";
                    return View("Register");
                }

                bool is_insert_client = clientDAO.insertClient(client);

                // Verifica se o Usuario foi Inserido e Redireciona p/ View de Detalhes
                if (is_insert_client) 
                    return RedirectToAction("Detalhes","Client", new { cpf = client.Cpf });
                
                ViewBag.Message = "Não foi possivel Cadastrar o Usuario no Sistema";
                ViewBag.Erro = clientDAO.error_operation;
                return View("ResultOperation");
            }
            catch (Exception ex)
            {
                return View("Error", ex);
            }
        }

        // GET: Client/Detalhes?cpf={cpf}
        [HttpGet]
        public ActionResult Detalhes(string cpf)
        {
            // Caso não seja infromado o CPF
            if (!new Client().validationCPF(cpf))
            {
                ViewBag.Message = "Informe um CPF Valido";
                ViewBag.Erro = "CPF não Informado ou Incorreto. " +
                    "Informe o CPF para Realizar a Operação";
                return View("ResultOperation");
            }

            try
            {
                // Instancia um novo Client e Busca o Usuario no Banco de Dados
                ClientDAO clientDAO = new ClientDAO();
                Client clientDatabase = new Client();
                clientDatabase = clientDAO.selectClient(cpf);


                // Usuario Encontrado no Banco de Dados
                if (clientDatabase != null) return View("Details", clientDatabase);

                ViewBag.Message = "Usuario não Cadastrado no Sistema";
                ViewBag.Erro = clientDAO.error_operation;
                return View("ResultOperation");
            }
            catch (Exception ex)
            {
                return View("Error", ex);
            }
        }


        // GET: Client/Atualizar?cpf={cpf}
        [HttpGet]
        public ActionResult Atualizar(string cpf)
        {
            // Caso não seja infromado o CPF
            if (!new Client().validationCPF(cpf))
            {
                ViewBag.Message = "Informe um CPF Valido";
                ViewBag.Erro = "CPF não Informado ou Incorreto. " +
                    "Informe o CPF para Realizar a Operação";
                return View("ResultOperation");
            }

            try
            {
                ClientDAO clientDAO = new ClientDAO();

                // Obtem um Usuario no Banco de Dados. Se não consegue = null
                Client client = clientDAO.selectClient(cpf);

                if (client != null)
                {
                    // Disponibiliza uma Lista com os Estados
                    ViewBag.Estados = new StateCity().listStates();
                    return View("Update", client);
                }
                else
                {
                    ViewBag.Message = "Usuario não Cadastrado no Sistema";
                    ViewBag.Erro = clientDAO.error_operation;
                    return View("ResultOperation");
                }
            }
            catch (Exception ex)
            {
                return View("Error", ex);
            }
        }

        // POST: Client/Atualizar
        [HttpPost]
        public ActionResult Atualizar(Client client)
        {
            // Caso não seja infromado o CPF
            if (!client.validationCPF(client.Cpf))
            {
                ViewBag.Message = "Informe um CPF Valido";
                ViewBag.Erro = "CPF não Informado ou Incorreto. " +
                    "Informe o CPF para Realizar a Operação";
                return View("ResultOperation");
            }

            try
            {
                ClientDAO clientDAO = new ClientDAO();
                bool is_update_client = clientDAO.updateClient(client);

                if (is_update_client) 
                    return RedirectToAction("Detalhes", "Client", new { Cpf = client.Cpf });

                ViewBag.Message = "Não foi possivel Atualizar o Usuario no Sistema";
                ViewBag.Erro = clientDAO.error_operation;
                return View("ResultOperation");
            }
            catch (Exception ex)
            {
                return View("Error" + ex);
            }
        }

        // GET: Client/Excluir?cpf={cpf}
        [HttpGet]
        public ActionResult Excluir(string cpf)
        {
            // Caso não seja infromado o CPF
            if (!new Client().validationCPF(cpf))
            {
                ViewBag.Message = "Informe um CPF Valido";
                ViewBag.Erro = "CPF não Informado ou Incorreto. " +
                    "Informe o CPF para Realizar a Operação";
                return View("ResultOperation");
            }

            try
            {
                ClientDAO clientDAO = new ClientDAO();

                // Obtem um Usuario no Banco de Dados. Se não consegue = null
                Client client = clientDAO.selectClient(cpf);

                if (client != null) return View("Delete", client);

                ViewBag.Message = "Não foi possivel Atualizar o Usuario no Sistema";
                ViewBag.Erro = clientDAO.error_operation;
                return View("ResultOperation");

            }
            catch (Exception ex)
            {
                return View("Error", ex);
            }
        }

        // POST: Client/Excluir
        [HttpPost]
        public ActionResult Excluir(Client client)
        {
            // Caso não seja infromado o CPF
            if (!new Client().validationCPF(client.Cpf))
            {
                ViewBag.Message = "Informe um CPF Valido";
                ViewBag.Erro = "CPF não Informado ou Incorreto. " +
                    "Informe o CPF para Realizar a Operação";
                return View("ResultOperation");
            }

            try
            {
                ClientDAO clientDAO = new ClientDAO();

                bool is_deleted_client = clientDAO.deleteClient(client.Cpf);
                if (is_deleted_client)
                {
                    ViewBag.Message = "Usuario Excluido com Suceso !";
                    return View("ResultOperation");
                }

                // Caso o Usuario não Atualize ou Não Obteve um Select do Usuario
                ViewBag.Message = "Usuario não Excluido do Sistema";
                ViewBag.Erro = clientDAO.error_operation;
                return View("ResultOperation");
            }
            catch (Exception ex)
            {
                return View("Error" + ex);
            }
        }

        public ActionResult ListUsers(List<Client> userList)
        {
            return View("List", userList);
        }

    }
}
