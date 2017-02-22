using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WRC_CMS.Communication;
using WRC_CMS.Models;
using WRC_CMS.Repository;

namespace WRC_CMS.Controllers
{
    public class AddViewController : Controller
    {
        WebApiProxy proxy = null;
        public ActionResult AddView()
        {
            proxy = new WebApiProxy();
            return View();
        }

        [HttpPost]
        public ActionResult AddView(ViewModel ViewObject, HttpPostedFileBase file)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Dictionary<string, object> dicParams = new Dictionary<string, object>();
                    dicParams.Add("@Name", ViewObject.Name);
                    dicParams.Add("@url", ViewObject.URL);
                    dicParams.Add("@Logo", ViewObject.Logo.ToString());
                    dicParams.Add("@Title", ViewObject.Title);
                    if (ViewObject.IsActive)
                        dicParams.Add("@IsActive", "1");
                    else
                        dicParams.Add("@IsActive", "0");

                    if (ViewObject.IsAuth)
                        dicParams.Add("@IsAuth", "1");
                    else
                        dicParams.Add("@IsAuth", "0");

                    if (ViewObject.IsDem)
                        dicParams.Add("@IsDem", "1");
                    else
                        dicParams.Add("@IsDem", "0");
                    proxy.ExecuteNonQuery("SP_ViewAddUp", dicParams);
                    ViewBag.Message = "View added successfully";
                }

                return View();
            }
            catch
            {
                return View();
            }
        }

        public ActionResult GetAllViewDetails()
        {
            ModelState.Clear();
            return View("GetViewDetails", GetAllViews());
        }

        public List<ViewModel> GetAllViews()
        {
            List<ViewModel> SiteList = new List<ViewModel>();
            ViewModel site = new ViewModel();
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
