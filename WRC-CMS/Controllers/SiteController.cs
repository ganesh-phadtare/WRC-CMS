using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
using WRC_CMS.Models;
using WRC_CMS.Repository;

namespace WRC_CMS.Controllers
{
    public class SiteController : Controller
    {
        public ActionResult AddSite()
        {
            return View();
        }
        
        [HttpPost]
        public ActionResult AddSite(SiteModel SiteObject)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    BORepository SiteRepo = new BORepository();

                    if (SiteRepo.AddRecord(SiteObject))
                    {
                        ViewBag.Message = "Site added successfully";
                    }
                }

                return View();
            }
            catch
            {
                return View();
            }
        }
    }
}
