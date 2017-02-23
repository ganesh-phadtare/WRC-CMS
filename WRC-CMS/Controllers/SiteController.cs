using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
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

        //public JsonResult SiteExists(string Name)
        //{
        //    var user = Name;
        //    return user == null ?
        //        Json(true, JsonRequestBehavior.AllowGet) :
        //        Json(string.Format("{0} Already Exist.", Name),
        //            JsonRequestBehavior.AllowGet);
        //}


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
                    Dictionary<string, object> dicParams = new Dictionary<string, object>();
                    dicParams.Add("@Oid", -1);
                    dicParams.Add("@Name", SiteObject.Name);
                    dicParams.Add("@url", SiteObject.URL);
                    dicParams.Add("@Logo", 0101);
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

        [HttpPost]
        public ActionResult EditSiteDetails(SiteModel SiteObject, HttpPostedFileBase file)
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
                    Dictionary<string, object> dicParams = new Dictionary<string, object>();
                    dicParams.Add("@Oid", SiteObject.Oid);
                    dicParams.Add("@Name", SiteObject.Name);
                    dicParams.Add("@url", SiteObject.URL);
                    dicParams.Add("@Logo", 0101);
                    dicParams.Add("@Title", SiteObject.Title);
                    if (SiteObject.IsActive)
                        dicParams.Add("@IsActive", "1");
                    else
                        dicParams.Add("@IsActive", "0");
                    proxy.ExecuteNonQuery("SP_SiteAddUp", dicParams);
                    return RedirectToAction("GetAllSitesDetails");
                }
                return View();
            }
            catch
            {
                return View();
            }
        }

        public async Task<ActionResult> EditSiteDetails(int id)
        {
            List<SiteModel> sites = new List<SiteModel>();
            await Task.Run(() =>
            {
                sites.AddRange(GetAllSites().Result);
            });
            if (sites != null && sites.Count > 0)
            {
                SiteModel objetc = sites.FirstOrDefault(item => item.Oid == id);
                return View(objetc);
            }
            return View();
        }

        public ActionResult DeleteSite(int id)
        {
            try
            {
                Dictionary<string, object> dicParams = new Dictionary<string, object>();
                dicParams.Add("@Oid", id);
                proxy.ExecuteNonQuery("SP_SiteDel", dicParams);
                return RedirectToAction("GetAllSitesDetails");
            }
            catch
            {
                return View();
            }
        }

        public async Task<ActionResult> GetAllSitesDetails()
        {
            ModelState.Clear();
            List<SiteModel> sites = new List<SiteModel>();
            await Task.Run(() =>
            {
                sites.AddRange(GetAllSites().Result);
            });
            return View("GetSitesDetails", sites);
        }

        public async Task<List<SiteModel>> GetAllSites()
        {
            List<SiteModel> SiteList = new List<SiteModel>();
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("@Oid", -1);
            dict.Add("@LoadOnlyActive", 0);

            var dataSet = await proxy.ExecuteDataset("SP_SiteSelect", dict);
            return (from DataRow row in dataSet.Tables[0].Rows
                    select new SiteModel
                    {
                        Oid = Convert.ToInt32(row["Oid"].ToString()),
                        Name = row["Name"].ToString(),
                        Title = row["Title"].ToString(),
                        URL = row["url"].ToString(),
                        IsActive = bool.Parse(row["IsActive"].ToString())
                    }).ToList();
        }
    }
}
