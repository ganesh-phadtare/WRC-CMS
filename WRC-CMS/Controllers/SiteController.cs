using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
using WRC_CMS.Communication;
using WRC_CMS.Models;
using WRC_CMS.Repository;

namespace WRC_CMS.Controllers
{
    public class SiteController : Controller
    {
        WebApiProxy proxy = new WebApiProxy();
        public ActionResult AddSite()
        {

            return View();
        }

        [HttpPost]
        public ActionResult AddSite(SiteModel SiteObject, HttpPostedFileBase file)
        {
            try
            {
                if (file != null && file.ContentLength > 0)
                {
                    SiteObject.Logo = new byte[file.ContentLength];
                    file.InputStream.Read(SiteObject.Logo, 0, file.ContentLength);
                }
                if (ModelState.IsValid)
                {
                    Dictionary<string, string> dicParams = new Dictionary<string, string>();
                    dicParams.Add("@Name", SiteObject.Name);
                    dicParams.Add("@url", SiteObject.URL);
                    dicParams.Add("@Logo", Convert.ToString(DBNull.Value));
                    dicParams.Add("@Title", SiteObject.Title);
                    if (SiteObject.IsActive)
                        dicParams.Add("@IsActive", "1");
                    else
                        dicParams.Add("@IsActive", "0");
                    proxy.ExecuteNonQuery("SP_SiteAddUp", dicParams);
                    ViewBag.Message = "Site added successfully";
                }
                return View();
            }
            catch
            {
                return View();
            }
        }

        public ActionResult GetAllSitesDetails()
        {
            ModelState.Clear();
            return View("GetSitesDetails", GetAllSites());
        }

        public List<SiteModel> GetAllSites()
        {
            List<SiteModel> SiteList = new List<SiteModel>();
            SiteModel site = new SiteModel();
            site.Name = "My Site";
            site.URL = "My Site";
            site.Title = "My Site";
            site.Logo = null;
            site.IsActive = true;
            SiteList.Add(site);
            return SiteList;


        }
    }
}
