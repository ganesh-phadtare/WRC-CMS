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

namespace WRC_CMS.Controllers
{
    public class AddViewController : Controller
    {
        WebApiProxy proxy = new WebApiProxy();
        public ActionResult AddView()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddView(ViewModel ViewObject, HttpPostedFileBase file)
        {
            try
            {
                if (file != null && file.ContentLength > 0)
                {
                    ViewObject.Logo = new byte[file.ContentLength];
                    file.InputStream.Read(ViewObject.Logo, 0, file.ContentLength);
                }

                if (ModelState.IsValid)
                {
                    ViewObject.SiteID = 3;
                    int ViewID = BORepository.AddView(proxy, ViewObject, true).Result;
                    if (ViewID > 0)
                        ViewBag.Message = "View added successfully.";
                    else
                        ViewBag.Message = "Problem occured while adding view, kindly contact our support team.";
                }

                return View();
            }
            catch
            {
                return View();
            }
        }


        [HttpPost]
        public ActionResult EditViewDetails(ViewModel ViewObject, HttpPostedFileBase file)
        {
            try
            {
                if (file != null && file.ContentLength > 0)
                {
                    ViewObject.Logo = new byte[file.ContentLength];
                    file.InputStream.Read(ViewObject.Logo, 0, file.ContentLength);
                }
                if (ModelState.IsValid)
                {
                    BORepository.AddView(proxy, ViewObject);
                    return RedirectToAction("GetAllViewDetails");
                }
                return View();
            }
            catch
            {
                return View();
            }
        }

        public async Task<ActionResult> EditViewDetails(int id)
        {
            List<ViewModel> views = new List<ViewModel>();
            await Task.Run(() =>
            {
                views.AddRange(BORepository.GetAllViews(proxy).Result);
            });
            if (views != null && views.Count > 0)
            {
                ViewModel objetc = views.FirstOrDefault(item => item.Oid == id);
                return View(objetc);
            }
            return View();
        }

        public ActionResult DeleteView(int id)
        {
            try
            {
                Dictionary<string, object> dicParams = new Dictionary<string, object>();
                dicParams.Add("@Oid", id);
                proxy.ExecuteNonQuery("SP_ViewDel", dicParams);
                return RedirectToAction("GetAllViewDetails");
            }
            catch
            {
                return View();
            }
        }

        public async Task<ActionResult> GetAllViewDetails()
        {
            ModelState.Clear();
            List<ViewModel> views = new List<ViewModel>();
            await Task.Run(() =>
            {
                views.AddRange(BORepository.GetAllViews(proxy).Result);
            });
            return View("GetViewDetails", views);
        }
    }
}
