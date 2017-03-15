using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WRC_CMS.Controllers
{
    public class ErrorInfoController : Controller
    {
        //
        // GET: /ErrorInfo/
        public ActionResult Index()
        {
            return View("Error");
        }
    }
}