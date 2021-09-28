using SchoolProject.Models;
using SchoolProject.Models.Database.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SchoolProject.Controllers
{
    public class ProductController : Controller
    {

        public const string ControllerProduct = "Product";

        // GET: Produtos
        [HttpGet]
        public ActionResult Index()
        {
            return RedirectToAction("Catalogo");
        }

        // GET: Produtos/Cadastro/{cnpj}
        [HttpGet]
        public ActionResult Cadastro(string cnpj)
        {
            Seller seller = new Seller();
            if (!seller.ValidationCNPJ(cnpj))
            {
                ViewBag.Message = "CNPJ Invalido.";
                ViewBag.Erro = seller.Error_Validation;
                return View("ResultOperation");
            }

            SellerDAO sellerDAO = new SellerDAO();
            if (sellerDAO.ExistsSeller(cnpj))
            {
                // Disponibiliza uma Lista com os Estados
                ViewBag.Estados = new StateCity().ListStates();
                Product product = new Product()
                {
                    Cnpj_Seller = cnpj
                };

                return View("Register", product);
            }
            else
            {
                ViewBag.Message = "Vendedor não Encontrado.";
                ViewBag.Erro = "O CNPJ Informado não existe no Banco de Dados";
                return View("ResultOperation");
            }
        }

        // POST: Produtos/Cadastro
        [HttpPost]
        public ActionResult Cadastro(Product product)
        {
            Seller seller = new Seller();
            if (!seller.ValidationCNPJ(product.Cnpj_Seller))
            {
                ViewBag.Message = "CNPJ Invalido.";
                ViewBag.Erro = seller.Error_Validation;
                return View("ResultOperation");
            }

            ViewBag.Estados = new StateCity().ListStates();

            // Instancia um novo Seller e Insere um Vendedor no Banco de Dados
            ProductDAO productDAO = new ProductDAO();

            // Verifica se o UsuarVendedorio foi Inserido e Redireciona p/ View de Detalhes
            if (productDAO.InsertProduct(product, product.Cnpj_Seller))
            {
                product.Id_product = productDAO.ReturnIdProduct(product);
                if(product.Id_product > 0)
                {
                    return RedirectToAction("Detalhes", ControllerProduct, new { id_product = product.Id_product });
                }
            }
            
            ViewBag.Message = "Não foi possivel Cadastrar o Produto no Sistema";
            ViewBag.Erro = productDAO.Error_operation;
            return View("ResultOperation");
        }

        // GET: Produtos/Detalhes/{id_product}
        [HttpGet]
        public ActionResult Detalhes(int id_product)
        {
            // Instancia um novo Seller e Busca o Vendedor no Banco de Dados
            ProductDAO productDAO = new ProductDAO();
            Product product = productDAO.SelectProduct(id_product);

            // Vendedor Encontrado no Banco de Dados
            if (product != null) return View("Details", product);

            ViewBag.Message = "Produto não Cadastrado no Sistema";
            ViewBag.Erro = productDAO.Error_operation;
            return View("ResultOperation");
        }

        // GET: Produtos/Atualizar/{id_product}
        [HttpGet]
        public ActionResult Atualizar(int id_product)
        {
            ProductDAO productDAO = new ProductDAO();
            // Obtem um Vendedor no Banco de Dados. Se não consegue = null
            Product product = productDAO.SelectProduct(id_product);

            if (product != null)
            {
                // Disponibiliza uma Lista com os Estados
                ViewBag.Estados = new StateCity().ListStates();
                return View("Update", product);
            }
            else
            {
                ViewBag.Message = "Produto não Cadastrado no Sistema";
                ViewBag.Erro = productDAO.Error_operation;
                return View("ResultOperation");
            }
        }

        // POST: Produtos/Atualizar
        [HttpPost]
        public ActionResult Atualizar(Product product)
        {
            ProductDAO productDAO = new ProductDAO();

            bool is_update_seller = productDAO.UpdateProduct(product, product.Cnpj_Seller);

            if (is_update_seller)
                return RedirectToAction("Detalhes", ControllerProduct, new { id_product = product.Id_product});

            ViewBag.Message = "Não foi possivel Atualizar o Produto no Sistema";
            ViewBag.Erro = productDAO.Error_operation;
            return View("ResultOperation");
        }

        // GET: Produtos/Excluir/{id_product}
        [HttpGet]
        public ActionResult Excluir(int id_product)
        {
            ProductDAO productDAO = new ProductDAO();

            // Obtem um Vendedor no Banco de Dados. Se não consegue = null
            Product product = productDAO.SelectProduct(id_product);
            if (product != null)
            {
                Seller seller = new Seller();
                return View("Delete", product);
            }

            ViewBag.Message = "Não foi possivel Atualizar o Produto no Sistema";
            ViewBag.Erro = productDAO.Error_operation;
            return View("ResultOperation");
        }

        // POST: Produtos/Excluir
        [HttpPost]
        public ActionResult Excluir(Product product)
        {
            Seller seller = new Seller();
            if (!seller.ValidationCNPJ(product.Cnpj_Seller))
            {
                ViewBag.Message = "CNPJ Invalido.";
                ViewBag.Erro = seller.Error_Validation;
                return View("ResultOperation");
            }

            try
            {
                ProductDAO productDAO = new ProductDAO();

                bool is_deleted_seller = productDAO.DeleteProduct(product.Name, product.Cnpj_Seller);
                if (is_deleted_seller)
                {
                    ViewBag.Message = "Produto Excluido com Suceso !";
                    return View("ResultOperation");
                }

                // Caso o Vendedor não Atualize ou Não Obteve um Select do Vendedor
                ViewBag.Message = "Produto não Excluido do Sistema";
                ViewBag.Erro = productDAO.Error_operation;
                return View("ResultOperation");
            }
            catch (Exception ex)
            {
                return View("Error" + ex);
            }
        }

        // GET: Produtos/Catalogo
        [HttpGet]
        [ActionName("Catalogo")]
        public ActionResult ListProducts()
        {
            try
            {
                ProductDAO productDAO = new ProductDAO();

                // Obtem uma List<Seller> com os elementos do Banco de Dados
                IEnumerable<Product>  listProducts = productDAO.ListProducts();
                if (listProducts != null && listProducts.Any()) 
                {
                    return View("ListProducts", listProducts);
                }

                // Caso não consiga recuperar os vendedores do banco de dados
                ViewBag.Message = "Lista de Produtos não Disponivel";
                ViewBag.Erro = productDAO.Error_operation;
                return View("ResultOperation");

            }
            catch (ArgumentNullException ex)
            {
                // Caso a Lista seja Nula
                ViewBag.Message = "Lista de Produtos não Disponivel";
                ViewBag.Erro = "Não foi possivel obter a Lista de Vendedores";
                System.Diagnostics.Debug.WriteLine("Lista de Funcionarios Nula. Exceção: " + ex);
                return View("ResultOperation");
            }
        }

        // GET: Produtos/ProdutosCadastrados/{cnpj}
        [ActionName("ProdutosCadastrados")]
        [HttpGet]
        public ActionResult ListProductsSeller(string cnpj)
        {
            try
            {
                ProductDAO productDAO = new ProductDAO();

                // Obtem uma List<Seller> com os elementos do Banco de Dados
                IEnumerable<Product>  listProducts = productDAO.ListProductsSeller(cnpj);
                if (listProducts != null && listProducts.Any())
                {
                    ViewBag.SellerCNPJ = cnpj;
                    return View("ListProducts", listProducts);
                }

                // Caso não consiga recuperar os vendedores do banco de dados
                ViewBag.Message = "Lista de Produtos não Disponivel";
                ViewBag.Erro = productDAO.Error_operation;
                return View("ResultOperation");
            }
            catch (ArgumentNullException ex)
            {
                // Caso a Lista seja Nula
                ViewBag.Message = "Lista de Produtos não Disponivel";
                ViewBag.Erro = "Não foi possivel obter a Lista de Produtos";
                System.Diagnostics.Debug.WriteLine("Lista de Produtos Nula. Exceção: " + ex);
                return View("ResultOperation");
            }
        }


    }
}
