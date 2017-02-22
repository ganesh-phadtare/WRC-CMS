using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WRC_CMS.Communication;
using WRC_CMS.Models;
using WRC_CMS.Repository;
using System.Linq;

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
                    Dictionary<string, object> dicParams = new Dictionary<string, object>();
                    dicParams.Add("@Oid", "-1");
                    dicParams.Add("@Name", ContentStyleModelObject.Name);
                    dicParams.Add("@Descr", ContentStyleModelObject.Description);
                    if (ContentStyleModelObject.IsActive)
                        dicParams.Add("@IsActive", "1");
                    else
                        dicParams.Add("@IsActive", "0");
                    proxy.ExecuteNonQuery("SP_StaticContentsAddUp", dicParams);
                    ViewBag.Message = "Content Style added successfully";
                    BORepository SiteRepo = new BORepository();                    
                    if (SiteRepo.AddRecord(ContentStyleModelObject))
                    {
                        //ViewBag.Message = "Content Style added successfully";
                        ViewBag.Message = ContentStyleModelObject.Description;
                    }
                }

                return View();
            }
            catch
            {
                return View();
            }
        }

        public ActionResult ShowContent(ContentStyleModel ContentStyleModelObject)
        {
            ViewBag.Message = ContentStyleModelObject.Description;
            return View("ShowContent");
        }

        public ActionResult GetAllContentsDetails()
        {
            ModelState.Clear();
            return View("GetContentsDetails", GetAllContents());
        }

        public List<ContentStyleModel> GetAllContents()
        {
            //List<ContentStyleModel> ContentList = new List<ContentStyleModel>();
            //int i = 10000;
            //while (i > 0)
            //{
            //    ContentStyleModel style = new ContentStyleModel();
            //    style.Name = "My Site";
            //    style.Description = "<p><strong>this is</strong> <a href='https://www.google.com' title='g'>google</a> <em>site</em></p>";
            //    style.IsActive = true;
            //    ContentList.Add(style);
            //    i--;
            //}
            //return ContentList;
        }  
    }
}
