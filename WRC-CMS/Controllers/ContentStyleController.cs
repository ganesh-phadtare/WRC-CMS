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
    public class ContentStyleController : Controller
    {
        WebApiProxy proxy = new WebApiProxy();
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
                    Dictionary<string, string> dicParams = new Dictionary<string, string>();
                    dicParams.Add("@Oid", "-1");
                    dicParams.Add("@Name", ContentStyleModelObject.Name);
                    dicParams.Add("@Descr", ContentStyleModelObject.Description);
                    if (ContentStyleModelObject.IsActive)
                        dicParams.Add("@IsActive", "1");
                    else
                        dicParams.Add("@IsActive", "0");
                    proxy.ExecuteNonQuery("SP_StaticContentsAddUp", dicParams);
                    ViewBag.Message = "Content Style added successfully";
                }

                return View();
            }
            catch
            {
                return View();
            }
        }


        public ActionResult GetAllContentsDetails()
        {
            ModelState.Clear();
            return View("GetContentsDetails", GetAllContents());
        }

        public List<ContentStyleModel> GetAllContents()
        {
            List<ContentStyleModel> ContentList = new List<ContentStyleModel>();
            ContentStyleModel style = new ContentStyleModel();
            style.Name = "My Site";
            style.Description = "Description";
            style.IsActive = true;
            ContentList.Add(style);
            return ContentList;
        }
    }
}