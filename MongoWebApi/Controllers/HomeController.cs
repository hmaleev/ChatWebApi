using System;
using System.Linq;
using System.Web.Mvc;

namespace MongoWebApi.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}