using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TaBetaltMedSwish.Controllers
{
    public class ProduktionController : Controller
    {
        // GET: Produktion
        public ActionResult Index()
        {
            Helpers.VisitorLogHelper.Log();

            return View();
        }
    }
}