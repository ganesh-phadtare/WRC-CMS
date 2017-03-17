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
    public class AddViewController : BaseController
    {
        public WebApiProxy proxy = new WebApiProxy();

        public async Task<JsonResult> AddUpdateRecord(ViewModel ModelObject)
        {
            string Status = string.Empty;
            await Task.Run(() =>
                {
                    Status = base.BaseAddUpdateRecord(ModelObject, ModelState, proxy).Result;
                }
            );
            return Json(new { status = Status });
        }

        public async Task<ActionResult> EditViewDetails(int ViewID = 0, int SiteID = 0)
        {
            if (ViewID != 0)
            {
                List<ViewModel> views = new List<ViewModel>();
                await Task.Run(() =>
                {
                    views.AddRange(BORepository.GetAllViews(proxy).Result.Where(item => item.SiteID == SiteID));
                });
                CombineModel com = new CombineModel();
                com.NewView = views.FirstOrDefault(view => view.Oid == ViewID);
                await Task.Run(() =>
               {
                   var IsMenuExist = BORepository.GetAllMenu(proxy).Result.FirstOrDefault(item => item.SiteId == SiteID && item.ViewId == ViewID);
                   if (!ReferenceEquals(IsMenuExist, null))
                       com.NewView.CreateMenu = true;
               });
                com.views = views;
                if (views.Count > 0)
                {
                    com.SiteName = views[0].SelectSite;
                    com.SiteID = views[0].SiteID;
                }
                ViewBag.CurrSiteID = SiteID;

                await Task.Run(() =>
                {
                    com.NewView.ViewAllContents.AddRange(BORepository.GetAllContents(proxy, com.SiteID).Result.ToList());
                });

                await Task.Run(() =>
                {
                    com.NewView.ViewContents.AddRange(BORepository.GetAllContents(proxy, com.SiteID, com.NewView.Oid).Result.ToList());
                });

                return View("ViewsLV", com);
            }
            else
                return RedirectToAction("GetAllViewDetails", new { id = SiteID });
        }

        public async Task<ActionResult> DeleteView(int ViewID, bool IsDefault, int SiteID)
        {
            if (!IsDefault)
            {
                Dictionary<string, object> dicParams = new Dictionary<string, object>();
                dicParams.Add("@Id", ViewID);
                proxy.ExecuteNonQuery("SP_ViewDel", dicParams);
            }
            else
                ViewData["DeletionError"] = "Problem occured while deleting view.Cannot delete Default View.";

            ActionResult View = null;
            await Task.Run(() =>
            {
                View = ReturnToMainView(SiteID).Result;
            });
            return View;
        }

        public ActionResult AddViewContent(int contentId, int siteId)
        {
            try
            {
                return RedirectToAction("GetAllViewDetails", new { id = siteId });
            }
            catch
            {
                return View();
            }
        }

        public async Task<ActionResult> DeleteViewContent(int id, int SiteID)
        {
            try
            {
                Dictionary<string, object> dicParams = new Dictionary<string, object>();
                dicParams.Add("@Id", id);
                proxy.ExecuteNonQuery("SP_ViewContentsDel", dicParams);

                ModelState.Clear();
                List<ViewModel> views = new List<ViewModel>();
                await Task.Run(() =>
                {
                    views.AddRange(BORepository.GetAllViews(proxy).Result.Where(item => item.SiteID == SiteID));
                });

                return RedirectToAction("GetAllViewDetails", new { id = SiteID });

            }
            catch
            {
                return View();
            }
        }

        public async Task<ActionResult> ReturnToMainView(int id)
        {
            List<ViewModel> views = new List<ViewModel>();
            await Task.Run(() =>
            {
                views.AddRange(BORepository.GetAllViews(proxy).Result.Where(item => item.SiteID == id));
            });
            CombineModel com = new CombineModel();
            com.NewView = new ViewModel();
            com.views = views;
            if (views.Count > 0)
            {
                com.SiteName = views[0].SelectSite;
                com.SiteID = views[0].SiteID;
            }
            ViewBag.CurrSiteID = id;

            await Task.Run(() =>
            {
                com.NewView.ViewAllContents.AddRange(BORepository.GetAllContents(proxy, com.SiteID, true).Result.ToList());
            });

            await Task.Run(() =>
                {
                    com.NewView.ViewContents.AddRange(BORepository.GetAllContents(proxy, com.SiteID, com.NewView.Oid).Result.ToList());
                });
            return View("ViewsLV", com);
        }

        public async Task<ActionResult> GetAllViewDetails(int id)
        {
            ActionResult View = null;
            await Task.Run(() =>
             {
                 View = ReturnToMainView(id).Result;
             });
            return View;
        }
    }
}
