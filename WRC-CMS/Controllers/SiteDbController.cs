using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WRC_CMS.Communication;
using WRC_CMS.Models;
using WRC_CMS.Repository;

namespace WRC_CMS.Controllers
{
    public class SiteDbController : BaseController
    {
        WebApiProxy proxy = new WebApiProxy();

        public ActionResult Index()
        {
            return View();
        }

        public async Task<JsonResult> AddUpdateRecord(SiteDbModel ModelObject)
        {
            string Status = string.Empty;
            await Task.Run(() =>
                            {
                                Status = base.BaseAddUpdateRecord(ModelObject, ModelState, proxy).Result;
                            }
            );
            return Json(new { status = Status });
        }

        public async Task<ActionResult> EditSiteDB(int SiteDBID = 0, int SiteID = 0)
        {
            if (SiteDBID != 0)
            {
                List<SiteDbModel> SiteDbs = new List<SiteDbModel>();
                await Task.Run(() =>
                {
                    SiteDbs.AddRange(BORepository.GetAllSiteDb(proxy, SiteID).Result);
                });
                SiteDbModelLD com = new SiteDbModelLD();
                com.DetailView = SiteDbs.FirstOrDefault(view => view.Id == SiteDBID);
                com.ListView = SiteDbs;
                if (SiteDbs.Count > 0)
                {
                    com.SiteName = SiteDbs[0].SiteName;
                    com.SiteID = SiteID;
                }
                ViewBag.CurrSiteID = SiteID;
                return View("GetAllSiteDb", com);
            }
            else
            {
                return RedirectToAction("GetAllSiteDb", new { SiteId = SiteID });
            }
        }


        public async Task<ActionResult> DeleteSiteDB(int id, int SiteID)
        {
            try
            {
                Dictionary<string, object> dicParams = new Dictionary<string, object>();
                dicParams.Add("@Id", id);
                proxy.ExecuteNonQuery("SP_SiteDBDel", dicParams);

                ModelState.Clear();
                List<SiteDbModel> views = new List<SiteDbModel>();
                await Task.Run(() =>
                {
                    views.AddRange(BORepository.GetAllSiteDb(proxy, SiteID).Result);
                });
                ActionResult View = null;
                await Task.Run(() =>
                {
                    View = ReturnToMainView(SiteID).Result;
                });
                return View;

            }
            catch
            {
                return View();
            }
        }

        public async Task<ActionResult> GetAllSiteDb(int SiteId)
        {
            ActionResult View = null;
            await Task.Run(() =>
            {
                View = ReturnToMainView(SiteId).Result;
            });
            return View;
        }

        public async Task<ActionResult> ReturnToMainView(int SiteId)
        {
            ModelState.Clear();
            ViewBag.CurrSiteID = SiteId;
            List<SiteDbModel> SiteDBS = new List<SiteDbModel>();
            await Task.Run(() =>
            {
                SiteDBS.AddRange(BORepository.GetAllSiteDb(proxy, SiteId).Result);
            });
            SiteDbModelLD com = new SiteDbModelLD();
            com.DetailView = new SiteDbModel();
            com.ListView = SiteDBS;
            await Task.Run(() =>
           {
               List<SiteModel> Sites = BORepository.GetAllSites(proxy, SiteId).Result;
               if (Sites != null && Sites.Count > 0)
               {
                   com.SiteName = Sites.First().Title;
                   com.SiteID = Sites.First().Oid;
               }
           });
            return View("GetAllSiteDb", com);
        }

        public ActionResult DeleteRecord(int id, int SiteID)
        {
            string Status = string.Empty;
            SiteDbModel modeldata = new SiteDbModel();
            modeldata.Id = id;

            Status = base.BaseDeleteRecord(modeldata, ModelState, proxy);

            return RedirectToAction("GetAllSiteDb", new { SiteId = SiteID });
        }
    }
}