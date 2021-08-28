using SchoolProject.Models;
using SchoolProject.Models.Database.DAO;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace SchoolProject.Controllers
{
    public class UserController : Controller
    { 

        // GET: User/Cadastro
        public ActionResult Cadastro()
        {
            // Disponibiliza uma Lista com os Estados
            ViewBag.Estados = new StateCity().listStates();

            return View("Register");
        }

        // POST: User/Cadastro
        [HttpPost]
        public ActionResult Cadastro(User user)
        {
            try
            {
                ViewBag.Estados = new StateCity().listStates();

                // Caso não seja infromado o CPF
                if (user.Cpf == null || user.Cpf.Length != 11)
                {
                    Exception exception = new Exception("CPF não Informado ou Incorreto. " +
                        "Informe o CPF para Realizar a Alteração");
                    return View("Error", exception);
                }

                // Instancia um novo User e Insere um Usuario no Banco de Dados
                UserDAO userDAO = new UserDAO();
                bool is_insert_user = userDAO.insertUser(user);

                // Verifica se o Usuario foi Inserido
                if (is_insert_user)
                {
                    // Redireciona para a Action Login --> Menu de Metodos CRUD
                    return RedirectToAction("Detalhes","User", new { cpf = user.Cpf });
                }
                else
                {
                    ViewBag.Message = "Não foi possivel Cadastrar o Usuario no Sistema";
                    ViewBag.Erro = userDAO.error_operation;
                    return View("Register");
                }
            }
            catch (Exception ex)
            {
                return View("Error", ex);
            }
        }

        // GET: User/Login
        public ActionResult Login()
        { 
            return View("Login");
        }   

        // POST: User/Login
        [HttpPost]
        public ActionResult Login(string cpf)
        {
            // Caso não seja infromado o CPF
            if (cpf == null || cpf.Length != 11)
            {
                Exception exception = new Exception("CPF não Informado ou Incorreto. " +
                    "Informe o CPF para Realizar a Alteração");
                return View("Error", exception);
            }

            try
            {
                // Instancia um novo User e Busca o Usuario no Banco de Dados
                UserDAO userDAO = new UserDAO();
                User userDatabase = userDAO.selectUser(cpf);

                if(userDatabase == null)
                {
                    ViewBag.Message = "Usuario não cadastrado no Sistema";
                    ViewBag.Erro = userDAO.error_operation;
                    return View("Login");
                }
                else return View("Login", userDatabase);
            }
            catch (Exception ex)
            {
                return View("Error", ex);
            }
        }

        // GET: User/Detalhes?cpf={cpf}
        [HttpGet]
        public ActionResult Detalhes(string cpf)
        {
            // Caso não seja infromado o CPF
            if (cpf == null || cpf.Length != 11)
            {
                Exception exception = new Exception("CPF não Informado ou Incorreto. " +
                    "Informe o CPF para Realizar a Alteração");
                return View("Error", exception);
            }

            try
            {
                // Instancia um novo User e Busca o Usuario no Banco de Dados
                UserDAO userDAO = new UserDAO();
                User userDatabase = new User();
                userDatabase = userDAO.selectUser(cpf);

                if (userDatabase == null)
                {
                    ViewBag.Message = "Usuario não cadastrado no Sistema";
                    ViewBag.Erro = userDAO.error_operation;
                    return View("Login");
                }
                else return View("Details", userDatabase);
            }
            catch (Exception ex)
            {
                return View("Error", ex);
            }
        }


        // GET: User/Atualizar?cpf={cpf}
        public ActionResult Atualizar(string cpf)
        {
            // Caso não seja infromado o CPF
            if (cpf == null || cpf.Length != 11)
            {
                Exception exception = new Exception("CPF não Informado ou Incorreto. " +
                    "Informe o CPF para Realizar a Alteração");
                return View("Error", exception);
            }

            try
            {
                UserDAO userDAO = new UserDAO();

                // Obtem um Usuario no Banco de Dados. Se não consegue = null
                User user = userDAO.selectUser(cpf);

                if (user != null)
                {
                    // Disponibiliza uma Lista com os Estados
                    ViewBag.Estados = new StateCity().listStates();
                    return View("Update", user);
                }
                else
                {
                    ViewBag.Message = "Usuario não cadastrado no Sistema.";
                    ViewBag.Erro = userDAO.error_operation;
                    return View("Update");
                }
            }
            catch (Exception ex)
            {
                return View("Error", ex);
            }
        }

        // POST: User/Atualizar
        [HttpPost]
        public ActionResult Atualizar(User user)
        {
            // Caso não seja infromado o CPF
            if (user == null || user.Cpf.Length != 11)
            {
                Exception exception = new Exception("Usuario Invalido. Informe os" +
                    " Dados do Usuario corretamente para Realizar a Alteração");
                return View("Error", exception);
            }

            try
            {
                UserDAO userDAO = new UserDAO();
                bool is_update_user = userDAO.updateUser(user);

                if (is_update_user) return RedirectToAction("Detalhes", "User", new { Cpf = user.Cpf });

                // Caso o Usuario não Atualize ou Não Obteve um Select do Usuario
                ViewBag.Message = "Usuario não Atualizado no Sistema";
                ViewBag.Erro = userDAO.error_operation;
                return View("Details");
            }
            catch (Exception ex)
            {
                return View("Error" + ex);
            }
        }

        // GET: User/Excluir?cpf={cpf}
        public ActionResult Excluir(string cpf)
        {
            // Caso não seja infromado o CPF
            if (cpf == null || cpf.Length != 11)
            {
                Exception exception = new Exception("CPF não Informado ou Incorreto. " +
                    "Informe o CPF para Realizar a Exclusão do Usuario");
                return View("Error", exception);
            }

            try
            {
                UserDAO userDAO = new UserDAO();

                // Obtem um Usuario no Banco de Dados. Se não consegue = null
                User user = userDAO.selectUser(cpf);

                if (user != null)
                {
                    return View("Delete", user);
                }
                else
                {
                    ViewBag.Message = "Usuario não cadastrado no Sistema";
                    ViewBag.Erro = userDAO.error_operation;
                    return View("Delete");
                }

            }
            catch (Exception ex)
            {
                return View("Error", ex);
            }
        }

        // POST: User/Excluir
        [HttpPost]
        public ActionResult Excluir(User user)
        {
            // Caso não seja infromado o CPF
            if (user == null || user.Cpf.Length != 11)
            {
                Exception exception = new Exception("Usuario Invalido. Informe os" +
                    " Dados do Usuario corretamente para Realizar a Alteração");
                return View("Error", exception);
            }

            try
            {
                UserDAO userDAO = new UserDAO();

                bool is_deleted_user = userDAO.deleteUser(user.Cpf);
                if (is_deleted_user) return RedirectToAction("Login", "User");

                // Caso o Usuario não Atualize ou Não Obteve um Select do Usuario
                ViewBag.Message = "Usuario não Excluido do Sistema";
                ViewBag.Erro = userDAO.error_operation;
                return View("Details");
            }
            catch (Exception ex)
            {
                return View("Error" + ex);
            }
        }

        public ActionResult ListUsers(List<User> userList)
        {
            return View("List", userList);
        }

    }
}
