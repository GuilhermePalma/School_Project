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
        // GET: Vendedor
        [HttpGet]
        public ActionResult Index()
        {
            return View("Login");
        }

        // GET: Vendedor/Login
        [HttpGet]
        public ActionResult Login()
        {
            return View("Login");
        }

        // POST: Vendedor/Login
        [HttpPost]
        public ActionResult Login(string cnpj)
        { 
            Seller seller = new Seller()
            {
                Cnpj= string.Empty
            };

            // Valida o CNPJ passado
            bool cnpjMask_valid = seller.ValidationMaskCNPJ(cnpj);
            bool cnpj_valid = seller.ValidationCNPJ(cnpj);

            if (cnpjMask_valid) seller.Cnpj = seller.RemoveMaskCNPJ(cnpj);
            if (cnpj_valid) seller.Cnpj = cnpj;

            if (string.IsNullOrEmpty(seller.Cnpj))
            {
                ViewBag.Message = "Informe um CNPJ Valido";
                ViewBag.Erro = seller.Error_Validation;
                return View("ResultOperation");
            }

            try
            {
                // Instancia um novo Seller e Busca o Vendedor no Banco de Dados
                SellerDAO sellerDAO = new SellerDAO();
                seller = sellerDAO.SelectSeller(seller.Cnpj);

                // Vendedor Encontrado no Banco de Dados
                if (seller != null) return View("Login", seller);

                ViewBag.Message = "Vendedor não Cadastrado no Sistema";
                ViewBag.Erro = sellerDAO.Error_operation;
                return View("ResultOperation");

            }
            catch (Exception ex)
            {
                return View("Error", ex);
            }
        }

        // GET: Vendedor/Cadastro
        [HttpGet]
        public ActionResult Cadastro()
        {
            // Disponibiliza uma Lista com os Estados
            ViewBag.Estados = new StateCity().ListStates();

            return View("Register");
        }

        // POST: Vendedor/Cadastro
        [HttpPost]
        public ActionResult Cadastro(Seller seller)
        {
            Phone phone = new Phone();
            bool valid_cpf = seller.ValidationMaskCNPJ(seller.Cnpj);
            bool valid_phone = phone.ValidationMaskPhone(seller.Telefone);

            string cnpj_formmated = valid_cpf ?
                seller.RemoveMaskCNPJ(seller.Cnpj) : string.Empty;
            string phone_formatted = valid_phone ?
                phone.RemoveMaskPhone(seller.Telefone) : string.Empty;
            string ddd_formatted = valid_phone ?
                phone.RemoveMaskDDD(seller.Telefone) : string.Empty;

            // Caso algum dado não foi Validado Corretamente
            if (string.IsNullOrEmpty(cnpj_formmated) || string.IsNullOrEmpty(phone_formatted) ||
                string.IsNullOrEmpty(ddd_formatted))
            {
                ViewBag.Message = "Dados Invalidos";
                ViewBag.Erro = valid_cpf ? phone.Error_Validation :
                    seller.Error_Validation;
                return View("ResultOperation");
            }

            seller.Ddd = ddd_formatted;
            seller.Telefone = phone_formatted;
            seller.Cnpj = cnpj_formmated;

            ViewBag.Estados = new StateCity().ListStates();

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

        // GET: Vendedor/Detalhes/{cnpj}
        [HttpGet]
        public ActionResult Detalhes(string cnpj)
        {
            // Valida CNPJ Passado
            Seller seller = new Seller();
            if (!seller.ValidationCNPJ(cnpj))
            {
                ViewBag.Message = "Informe um CNPJ Valido";
                ViewBag.Erro = seller.Error_Validation;
                return View("ResultOperation");
            }
            else seller.Cnpj = cnpj;

            try
            {
                // Instancia um novo Seller e Busca o Vendedor no Banco de Dados
                SellerDAO sellerDAO = new SellerDAO();
                seller = sellerDAO.SelectSeller(seller.Cnpj);

                // Vendedor Encontrado no Banco de Dados
                if (seller != null) return View("Details", seller);

                ViewBag.Message = "Vendedor não Cadastrado no Sistema";
                ViewBag.Erro = sellerDAO.Error_operation;
                return View("ResultOperation");
            }
            catch (Exception ex)
            {
                return View("Error", ex);
            }
        }


        // GET: Vendedor/Atualizar/{cnpj}
        [HttpGet]
        public ActionResult Atualizar(string cnpj)
        {
            // Valida CNPJ Passado
            Seller seller = new Seller();
            if (!seller.ValidationCNPJ(cnpj))
            {
                ViewBag.Message = "Informe um CNPJ Valido";
                ViewBag.Erro = seller.Error_Validation;
                return View("ResultOperation");
            }
            else seller.Cnpj = cnpj;

            try
            {
                SellerDAO sellerDAO = new SellerDAO();
                // Obtem um Vendedor no Banco de Dados. Se não consegue = null
                seller = sellerDAO.SelectSeller(seller.Cnpj);

                if (seller != null)
                {
                    // Disponibiliza uma Lista com os Estados
                    ViewBag.Estados = new StateCity().ListStates();
                    return View("Update", seller);
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

        // POST: Vendedor/Atualizar
        [HttpPost]
        public ActionResult Atualizar(Seller seller)
        {
            Phone phone = new Phone();
            bool valid_cpf = seller.ValidationCNPJ(seller.Cnpj);
            bool valid_phone = phone.ValidationMaskPhone(seller.Telefone);

            string phone_formatted = valid_phone ?
                phone.RemoveMaskPhone(seller.Telefone) : string.Empty;
            string ddd_formatted = valid_phone ?
                phone.RemoveMaskDDD(seller.Telefone) : string.Empty;

            // Caso algum dado não foi Validado Corretamente
            if (string.IsNullOrEmpty(phone_formatted) || 
                string.IsNullOrEmpty(ddd_formatted))
            {
                ViewBag.Message = "Dados Invalidos";
                ViewBag.Erro = valid_cpf ? phone.Error_Validation :
                    seller.Error_Validation;
                return View("ResultOperation");
            }

            seller.Ddd = ddd_formatted;
            seller.Telefone = phone_formatted;

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

        // GET: Vendedor/Excluir/{cnpj}
        [HttpGet]
        public ActionResult Excluir(string cnpj)
        {
            // Valida CNPJ Passado
            Seller seller = new Seller();
            if (!seller.ValidationCNPJ(cnpj))
            {
                ViewBag.Message = "Informe um CNPJ Valido";
                ViewBag.Erro = seller.Error_Validation;
                return View("ResultOperation");
            }
            else seller.Cnpj = cnpj;

            try
            {
                SellerDAO sellerDAO = new SellerDAO();

                // Obtem um Vendedor no Banco de Dados. Se não consegue = null
                seller = sellerDAO.SelectSeller(seller.Cnpj);

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

        // POST: Vendedor/Excluir
        [HttpPost]
        public ActionResult Excluir(Seller seller)
        {
            // Valida CNPJ Passado
            if (!seller.ValidationCNPJ(seller.Cnpj))
            {
                ViewBag.Message = "Informe um CNPJ Valido";
                ViewBag.Erro = seller.Error_Validation;
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

        // GET: Vendedor/Cadastrados
        [ActionName("Cadastrados")]
        public ActionResult ListSellers()
        {
            try
            {
                SellerDAO sellerDAO = new SellerDAO();

                // Obtem uma List<Seller> com os elementos do Banco de Dados
                IEnumerable<Seller>  listSeller = sellerDAO.ListSellers();
                if (listSeller != null && listSeller.Any())
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
