using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WRC_CMS.Communication;
using WRC_CMS.Models;
using WRC_CMS.Repository;

namespace WRC_CMS.Controllers
{
    public class BulkDataController : Controller
    {
        WebApiProxy proxy = new WebApiProxy();
        public ActionResult BulkData()
        {
            return View();
        }

        [HttpPost]
        public ActionResult BulkData(BulkDataModel bulkData)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (bulkData != null)
                    {
                        //Dictionary<string, object> dict = new Dictionary<string, object>();
                        //for (int i = 0; i < bulkData.NoOfRecords; i++)
                        //{
                        //    dict.Add("@Oid", -1);
                        //    dict.Add("@Name", MockData.Product.ProductName());
                        //    dict.Add("@url", MockData.Internet.DomainName());
                        //    dict.Add("@Logo", 01010101010100000);
                        //    dict.Add("@Title", "");
                        //    dict.Add("@IsActive", 1);

                        //    proxy.ExecuteNonQuery("SP_SiteAddUp", dict);
                        //    dict.Clear();
                        //}

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