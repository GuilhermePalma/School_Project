using SchoolProject.Models;
using SchoolProject.Models.Database.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SchoolProject.Controllers
{
    public class SellerController : Controller
    {
        private const string INVALID_CNPJ = "CNPJ não Informado ou Incorreto. " +
            "CNPJ deve conter apenas 14 Numeros ";

        // GET: Seller
        public ActionResult Index()
        {
            return View("Login");
        }

        // POST: Seller/Login
        [HttpPost]
        public ActionResult Login(string cnpj)
        {
            // Caso não seja infromado o CNPJ
            if (!Seller.ValidationCNPJ(cnpj))
            {
                ViewBag.Message = "Informe um CNPJ Valido";
                ViewBag.Erro = INVALID_CNPJ;
                return View("ResultOperation");
            }

            try
            {
                // Instancia um novo Seller e Busca o Vendedor no Banco de Dados
                SellerDAO sellerDAO = new SellerDAO();
                Seller sellerDatabase = sellerDAO.SelectSeller(cnpj);

                // Vendedor Encontrado no Banco de Dados
                if (sellerDatabase != null) return View("Login", sellerDatabase);

                ViewBag.Message = "Vendedor não Cadastrado no Sistema";
                ViewBag.Erro = sellerDAO.Error_operation;
                return View("ResultOperation");

            }
            catch (Exception ex)
            {
                return View("Error", ex);
            }
        }

        // GET: Seller/Cadastro
        [HttpGet]
        public ActionResult Cadastro()
        {
            // Disponibiliza uma Lista com os Estados
            ViewBag.Estados = new StateCity().listStates();

            return View("Register");
        }

        // POST: Seller/Cadastro
        [HttpPost]
        public ActionResult Cadastro(Seller seller)
        {
            // Caso não seja infromado o CNPJ
            if (!Seller.ValidationCNPJ(seller.Cnpj))
            {
                ViewBag.Message = "Informe um CNPJ Valido";
                ViewBag.Erro = INVALID_CNPJ;
                return View("ResultOperation");
            }

            ViewBag.Estados = new StateCity().listStates();

            // Instancia um novo Seller e Insere um Vendedor no Banco de Dados
            SellerDAO sellerDAO = new SellerDAO();

            // Verifica se o UsuarVendedorio foi Inserido e Redireciona p/ View de Detalhes
            if (sellerDAO.InsertSeller(seller))
            {
                return RedirectToAction("Detalhes", "Seller", new { cnpj = seller.Cnpj });
            }
            else
            {
                ViewBag.Message = "Não foi possivel Cadastrar o Vendedor no Sistema";
                ViewBag.Erro = sellerDAO.Error_operation;
                return View("ResultOperation");
            }
        }

        // GET: Seller/Detalhes/{cnpj}
        [HttpGet]
        public ActionResult Detalhes(string cnpj)
        {
            // Caso não seja infromado o CNPJ
            if (!Seller.ValidationCNPJ(cnpj))
            {
                ViewBag.Message = "Informe um CNPJ Valido";
                ViewBag.Erro = INVALID_CNPJ;
                return View("ResultOperation");
            }

            try
            {
                // Instancia um novo Seller e Busca o Vendedor no Banco de Dados
                SellerDAO sellerDAO = new SellerDAO();

                Seller sellerDatabase = new Seller();
                sellerDatabase = sellerDAO.SelectSeller(cnpj);

                // Vendedor Encontrado no Banco de Dados
                if (sellerDatabase != null) return View("Details", sellerDatabase);

                ViewBag.Message = "Vendedor não Cadastrado no Sistema";
                ViewBag.Erro = sellerDAO.Error_operation;
                return View("ResultOperation");
            }
            catch (Exception ex)
            {
                return View("Error", ex);
            }
        }


        // GET: Seller/Atualizar/{cnpj}
        [HttpGet]
        public ActionResult Atualizar(string cnpj)
        {
            // Caso não seja infromado o CNPJ
            if (!Seller.ValidationCNPJ(cnpj))
            {
                ViewBag.Message = "Informe um CNPJ Valido";
                ViewBag.Erro = INVALID_CNPJ;
                return View("ResultOperation");
            }

            try
            {
                SellerDAO sellerDAO = new SellerDAO();
                // Obtem um Vendedor no Banco de Dados. Se não consegue = null
                Seller sellerDatabase = sellerDAO.SelectSeller(cnpj);

                if (sellerDatabase != null)
                {
                    // Disponibiliza uma Lista com os Estados
                    ViewBag.Estados = new StateCity().listStates();
                    return View("Update", sellerDatabase);
                }
                else
                {
                    ViewBag.Message = "Vendedor não Cadastrado no Sistema";
                    ViewBag.Erro = sellerDAO.Error_operation;
                    return View("ResultOperation");
                }
            }
            catch (Exception ex)
            {
                return View("Error", ex);
            }
        }

        // POST: Seller/Atualizar
        [HttpPost]
        public ActionResult Atualizar(Seller seller)
        {
            // Caso não seja infromado o CNPJ
            if (!Seller.ValidationCNPJ(seller.Cnpj))
            {
                ViewBag.Message = "Informe um CNPJ Valido";
                ViewBag.Erro = INVALID_CNPJ;
                return View("ResultOperation");
            }

            try
            {
                SellerDAO sellerDAO = new SellerDAO();
                bool is_update_seller = sellerDAO.UpdateSeller(seller);

                if (is_update_seller)
                    return RedirectToAction("Detalhes", "Seller", new { Cnpj = seller.Cnpj });

                ViewBag.Message = "Não foi possivel Atualizar o Vendedor no Sistema";
                ViewBag.Erro = sellerDAO.Error_operation;
                return View("ResultOperation");
            }
            catch (Exception ex)
            {
                return View("Error" + ex);
            }
        }

        // GET: Seller/Excluir/{cnpj}
        [HttpGet]
        public ActionResult Excluir(string cnpj)
        {
            // Caso não seja infromado o CNPJ
            if (!Seller.ValidationCNPJ(cnpj))
            {
                ViewBag.Message = "Informe um CNPJ Valido";
                ViewBag.Erro = INVALID_CNPJ;
                return View("ResultOperation");
            }

            try
            {
                SellerDAO sellerDAO = new SellerDAO();

                // Obtem um Vendedor no Banco de Dados. Se não consegue = null
                Seller seller = sellerDAO.SelectSeller(cnpj);

                if (seller != null) return View("Delete", seller);

                ViewBag.Message = "Não foi possivel Atualizar o Vendedor no Sistema";
                ViewBag.Erro = sellerDAO.Error_operation;
                return View("ResultOperation");

            }
            catch (Exception ex)
            {
                return View("Error", ex);
            }
        }

        // POST: Seller/Excluir
        [HttpPost]
        public ActionResult Excluir(Seller seller)
        {
            // Caso não seja infromado o CNPJ
            if (!Seller.ValidationCNPJ(seller.Cnpj))
            {
                ViewBag.Message = "Informe um CNPJ Valido";
                ViewBag.Erro = INVALID_CNPJ;
                return View("ResultOperation");
            }

            try
            {
                SellerDAO sellerDAO = new SellerDAO();

                bool is_deleted_seller = sellerDAO.DeleteSeller(seller.Cnpj);
                if (is_deleted_seller)
                {
                    ViewBag.Message = "Vendedor Excluido com Suceso !";
                    return View("ResultOperation");
                }

                // Caso o Vendedor não Atualize ou Não Obteve um Select do Vendedor
                ViewBag.Message = "Vendedor não Excluido do Sistema";
                ViewBag.Erro = sellerDAO.Error_operation;
                return View("ResultOperation");
            }
            catch (Exception ex)
            {
                return View("Error" + ex);
            }
        }

        [ActionName("Cadastrados")]
        public ActionResult ListSellers()
        {
            SellerDAO sellerDAO = new SellerDAO();
            IEnumerable<Seller> listSeller;
            try
            {
                // Obtem uma List<Seller> com os elementos do Banco de Dados
                listSeller = sellerDAO.ListSellers();
                if (listSeller != null || listSeller.Count() > 0)
                {
                    return View("ListSeller", listSeller);
                }

                // Caso não consiga recuperar os vendedores do banco de dados
                ViewBag.Message = "Lista de Funcionarios não Disponivel";
                ViewBag.Erro = sellerDAO.Error_operation;
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
