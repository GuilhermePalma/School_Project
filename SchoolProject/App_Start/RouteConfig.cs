using System.Web.Mvc;
using System.Web.Routing;

namespace SchoolProject
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Products_Home",
                url: "Produtos/{action}/{id_product}",
                defaults: new { controller = "Product", action = "Index", id_product = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Seller",
                url: "Vendedor/{action}/{cnpj}",
                defaults: new { controller = "Seller", action = "Index", cnpj = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Client",
                url: "Cliente/{action}/{cpf}",
                defaults: new { controller = "Client", action = "Index", cpf = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

        }
    }
}