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
        public async Task<ActionResult> AddView()
        {
            ViewModel ViewObject = new ViewModel();
            if (ViewObject != null)
            {
                await Task.Run(() =>
                {
                    ViewObject.Site = BORepository.GetAllSites(proxy).Result;
                });
            }
            return View(ViewObject);
        }

        public async Task<ActionResult> Test(string Name, string URL, string Title, string IsActive, string IsDem, string IsAuth, string CreateMenu, object Logo)
        {
            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(URL) || string.IsNullOrEmpty(Title))
                return RedirectToAction("GetAllViewDetails");
            ViewModel ViewObject = new ViewModel();
            ViewObject.Name = Name.ToString();
            ViewObject.URL = URL.ToString();
            ViewObject.Title = Title.ToString();
            ViewObject.IsActive = Convert.ToBoolean(IsActive);
            ViewObject.IsDem = Convert.ToBoolean(IsDem);
            ViewObject.IsAuth = Convert.ToBoolean(IsAuth);
            ViewObject.CreateMenu = Convert.ToBoolean(CreateMenu);
            ViewObject.SiteID = 2078;
            try
            {
                //if (file != null && file.ContentLength > 0)
                //{
                //    ViewObject.Logo = new byte[file.ContentLength];
                //    file.InputStream.Read(ViewObject.Logo, 0, file.ContentLength);
                //}

                if (ModelState.IsValid)
                {
                    if (!string.IsNullOrEmpty(ViewObject.SelectSite))
                        ViewObject.SiteID = Convert.ToInt32(ViewObject.SelectSite.ToString());
                    int ViewID = 0;
                    await Task.Run(() =>
                    {
                        ViewID = BORepository.AddView(proxy, ViewObject, true).Result;
                    });
                    if (ViewID > 0)
                        ViewBag.Message = "View added successfully.";
                    else
                        ViewBag.Message = "Problem occured while adding view, kindly contact our support team.";
                    return RedirectToAction("GetAllViewDetails");
                }

                return View();
            }
            catch
            {
                return View();
            }
        }



        [HttpPost]
        public async Task<ActionResult> AddView(ViewModel ViewObject, HttpPostedFileBase file)
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
                    if (!string.IsNullOrEmpty(ViewObject.SelectSite))
                        ViewObject.SiteID = Convert.ToInt32(ViewObject.SelectSite.ToString());
                    int ViewID = 0;
                    await Task.Run(() =>
                           {
                               ViewID = BORepository.AddView(proxy, ViewObject, true).Result;
                           });
                    if (ViewID > 0)
                        ViewBag.Message = "View added successfully.";
                    else
                        ViewBag.Message = "Problem occured while adding view, kindly contact our support team.";
                    return RedirectToAction("GetAllViewDetails");
                }

                return View();
            }
            catch
            {
                return View();
            }
        }


        [HttpPost]
        public async Task<ActionResult> EditViewDetails(ViewModel ViewObject, HttpPostedFileBase file)
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
                    if (!string.IsNullOrEmpty(ViewObject.SelectSite))
                        ViewObject.SiteID = Convert.ToInt32(ViewObject.SelectSite.ToString());
                    int ViewID = 0;
                    await Task.Run(() =>
                    {
                        ViewID = BORepository.AddView(proxy, ViewObject).Result;
                    });
                    if (ViewID > 0)
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
                if (objetc != null)
                {
                    await Task.Run(() =>
                    {
                        objetc.Site = BORepository.GetAllSites(proxy).Result;
                    });
                    objetc.SelectSite = objetc.SiteID.ToString();
                }
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
            CombineModel com = new CombineModel();
            com.NewView = new ViewModel();
            com.views = views;
            return View("ViewsLV", com);
        }
    }
}
