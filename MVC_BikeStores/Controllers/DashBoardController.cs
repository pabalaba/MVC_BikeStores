using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC_BikeStores.Controllers
{
    public class DashBoardController : Controller
    {
        // GET: DashBoard
        public ActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "Administrators")]
        public ActionResult Administrators()
        {
            return View();
        }
        [Authorize(Roles = "Employees")]
        public ActionResult Employees()
        {
            return View();
        }
        [Authorize(Roles = "Customers")]
        public ActionResult Customers()
        {
            return View();
        }

    }
}