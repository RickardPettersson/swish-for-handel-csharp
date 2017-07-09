using System.Web.Mvc;

namespace TaBetaltMedSwish.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            Helpers.VisitorLogHelper.Log();

            return View();
        }

        public ActionResult About()
        {
            Helpers.VisitorLogHelper.Log();

            return View();
        }

        public ActionResult Contact()
        {
            Helpers.VisitorLogHelper.Log();

            return View();
        }
    }
}