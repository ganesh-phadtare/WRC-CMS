using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WRC_CMS.Models;
using WRC_CMS.Repository;

namespace WRC_CMS.Controllers
{
    public class ContentStyleController : Controller
    {
        public ActionResult AddContentStyle()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddContentStyle(ContentStyleModel ContentStyleModelObject)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    BORepository SiteRepo = new BORepository();

                    if (SiteRepo.AddRecord(ContentStyleModelObject))
                    {
                        ViewBag.Message = "Content Style added successfully";
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