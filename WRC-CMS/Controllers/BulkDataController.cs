using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WRC_CMS.Models;
using WRC_CMS.Repository;

namespace WRC_CMS.Controllers
{
    public class BulkDataController : Controller
    {
        public ActionResult BulkData()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateBulkRecords(SiteModel SiteObject)
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