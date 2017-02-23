using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WRC_CMS.Models;

namespace WRC_CMS.Controllers
{
    public class MenuController : Controller
    {
        //
        // GET: /Menu/
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddMenu(SiteModel objSiteModel)
        {
            return View();
        }
	}
}