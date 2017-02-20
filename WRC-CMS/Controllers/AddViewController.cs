using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WRC_CMS.Models;
using WRC_CMS.Repository;

namespace WRC_CMS.Controllers
{
    public class AddViewController : Controller
    {
        public ActionResult AddView()
        {
            return View();
        }
        
        [HttpPost]
        public ActionResult AddView(ViewModel ViewObject)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    BORepository SiteRepo = new BORepository();

                    if (SiteRepo.AddRecord(ViewObject))
                    {
                        ViewBag.Message = "View added successfully";
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
