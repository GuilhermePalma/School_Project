using Database.DAO;
using SchoolProject.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace SchoolProject.Controllers
{
    public class UserController : Controller
    {


        // GET: User
        public ActionResult Index()
        {
            return View("Home");
        }

        // GET: User/Cadastro
        public ActionResult Cadastro()
        {
            // Disponibiliza uma Lista com os Estados
            User user = new User();
            ViewBag.Estados = user.listStates();

            return View("Register");
        }

        // POST: User/Cadastro
        [HttpPost]
        public ActionResult Cadastro(User user)
        {
            try
            {
                // TODO: Add insert logic here

                UserDAO userDAO = new UserDAO();
                bool insert_complet = userDAO.insertUser(user);

                if (insert_complet)
                {
                    return View("Login", user);
                }
                else
                {
                    Exception exception = new Exception("Não foi Possivel realizar" +
                        "o Cadastro. Erro: " + userDAO.error_operation);
                    return View("Error", exception);
                }

            }
            catch (Exception ex)
            {
                return View("Error", ex);
            }
        }

        public ActionResult Login()
        {
            return View("Login");
        }

        // POST: User/Login
        [HttpPost]
        public ActionResult Login(User user)
        {
            // Caso não seja infromado o CPF
            if (user.Cpf == null || user.Cpf.Length < 11)
            {
                Exception exception = new Exception("CPF não Informado ou Incorreto. " +
                    "Informe o CPF para Realizar a Alteração");
                return View("Error", exception);
            }

            try
            {
                // TODO: Add update logic here
                // Busca o BD o CPF ---> Se não Existir ---> Error

                UserDAO userDAO = new UserDAO();
                bool exist_user = userDAO.existsUser(user.Cpf);

                if (exist_user)
                {
                    User userDatabase = new User();
                    user = userDAO.selectUser(user.Cpf);

                    if(user == null)
                    {
                        Exception ex = new Exception("Não foi possivel Localizar o Usuario." +
                            "Erro: " + userDAO.error_operation);
                        return View("Error", ex);
                    }
                    else
                    {
                        return View("Login", userDatabase);
                    }

                }
                else
                {
                    ViewBag.Message = "Usuario não cadastrado no Sistema" +userDAO.error_operation;
                    return View("Login");
                }
            }
            catch (Exception ex)
            {
                return View("Error", ex);
            }
        }

        // GET: User/Detalhes?cpf={cpf}
        public ActionResult Detalhes(string cpf)
        {
            // Caso não seja infromado o CPF
            if (cpf == null || cpf.Length < 11)
            {
                Exception exception = new Exception("CPF não Informado ou Incorreto. " +
                    "Informe o CPF para Realizar a Alteração");
                return View("Error", exception);
            }

            try
            {
                // TODO: Add update logic here
                // Busca o BD o CPF ---> Se não Existir ---> Error

                User user = new User
                {
                    Name = "Robson",
                    Cpf = cpf
                };

                // Busca o CPF no Banco de Dados
                return View("Details", user);
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
            if (cpf == null || cpf.Length < 11)
            {
                Exception exception = new Exception("CPF não Informado ou Incorreto. " +
                    "Informe o CPF para Realizar a Alteração");
                return View("Error", exception);
            }

            try
            {
                // TODO: Add update logic here
                // Busca o BD o CPF ---> Se não Existir ---> Error


                User user = new User()
                {
                    Cpf = cpf
                };

                // Disponibiliza uma Lista com os Estados
                ViewBag.Estados = user.listStates();

                // Busca o CPF no Banco de Dados
                return View("Update", user);
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
            if (user == null)
            {
                Exception exception = new Exception("Usuario não Encontrado. Informe os" +
                    " Dados do Usuario para Realizar a Alteração");
                return View("Error", exception);
            } else if (user.Cpf == null || user.Cpf.Length < 11)
            {
                Exception exception = new Exception("CPF não Informado ou Incorreto. " +
                    "Informe o CPF para Realizar a Alteração");
                return View("Error", exception);
            } 

            try
            {
                // TODO: Add update logic here
                // Busca o BD o CPF ---> Se não Existir ---> Error

                // Caso realize a opereação no BD, mostra os dados do Usuario
                return View("Details", user);
            }
            catch (Exception ex)
            {
                return View("Error" + ex);
            }
        }

        // GET: User/Atualizar?cpf={cpf}
        public ActionResult Excluir(string cpf)
        {
            // Caso não seja infromado o CPF
            if (cpf == null || cpf.Length < 11)
            {
                Exception exception = new Exception("CPF não Informado ou Incorreto. " +
                    "Informe o CPF para Realizar a Alteração");
                return View("Error", exception);
            }

            try
            {
                // TODO: Add update logic here
                // Busca o BD o CPF ---> Se não Existir ---> Error

                User user = new User()
                {
                    Name = "Robson",
                    Cpf = cpf
                };
                // Busca o CPF no Banco de Dados
                return View("Delete", user);
            }
            catch (Exception ex)
            {
                return View("Error", ex);
            }
        }

        // POST: User/Excluir
        [HttpPost]
        public ActionResult Excluir(string cpf, User user)
        {
            // Caso não seja infromado o CPF
            if (cpf == null || cpf.Length < 11)
            {
                Exception exception = new Exception("CPF não Informado ou Incorreto. " +
                    "Informe o CPF para Realizar a Alteração");
                return View("Error", exception);
            }

            try
            {
                // TODO: Add update logic here
                // Busca o BD o CPF ---> Se não Existir ---> Error

                // Busca o CPF no Banco de Dados
                ViewBag.Message = "Usuario " + user.Name  + " Excluido";
                return RedirectToAction("Login", "User");
            }
            catch (Exception ex)
            {
                return View("Error", ex);
            }
        }

        public ActionResult ListUsers(List<User> userList)
        {
            return View("List", userList);
        }

    }
}
