using System.Web.Mvc;

namespace SchoolProject.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult PageNotFound()
        {
            Response.StatusCode = 404;
            return View();
        }
    }
}