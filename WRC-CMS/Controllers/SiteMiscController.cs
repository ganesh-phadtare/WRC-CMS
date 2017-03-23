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
    public class SiteMiscController : BaseController
    {
        WebApiProxy proxy = new WebApiProxy();

        public async Task<JsonResult> AddUpdateRecord(SiteMiscModel ModelObject)
        {
            string Status = string.Empty;
            await Task.Run(() =>
            {
                Status = base.BaseAddUpdateRecord(ModelObject, ModelState, proxy).Result;
            }
            );
            return Json(new { status = Status });
        }        

        public async Task<ActionResult> EditSiteMisc(int SiteMISCID = 0, int SiteID = 0)
        {
            if (SiteMISCID != 0)
            {
                List<SiteMiscModel> SiteMICs = new List<SiteMiscModel>();
                await Task.Run(() =>
                {
                    SiteMICs.AddRange(BORepository.GetAllSiteMISC(proxy).Result.Where(item => item.SiteId == SiteID));
                });
                SiteMiscModelLD com = new SiteMiscModelLD();
                com.DetailView = SiteMICs.FirstOrDefault(view => view.Id == SiteMISCID);
                com.ListView = SiteMICs;
                if (SiteMICs.Count > 0)
                {
                    com.SiteName = SiteMICs[0].SiteName;
                    com.SiteID = SiteID;
                }
                ViewBag.CurrSiteID = SiteID;
                return View("GetAllSiteMisc", com);
            }
            else
            {
                return RedirectToAction("GetAllSiteMisc", new { SiteId = SiteID });
            }
        }

        public async Task<ActionResult> DeleteSiteMisc(int id, int SiteID)
        {
            try
            {
                Dictionary<string, object> dicParams = new Dictionary<string, object>();
                dicParams.Add("@Id", id);
                proxy.ExecuteNonQuery("SP_SiteMiscDel", dicParams);

                ModelState.Clear();
                List<SiteMiscModel> views = new List<SiteMiscModel>();
                await Task.Run(() =>
                {
                    views.AddRange(BORepository.GetAllSiteMISC(proxy).Result.Where(item => item.SiteId == SiteID));
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

        public async Task<ActionResult> GetAllSiteMisc(int SiteId)
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
            List<SiteMiscModel> SiteMISC = new List<SiteMiscModel>();
            await Task.Run(() =>
            {
                SiteMISC.AddRange(BORepository.GetAllSiteMISC(proxy).Result.Where(item => item.SiteId == SiteId));
            });
            SiteMiscModelLD com = new SiteMiscModelLD();
            com.DetailView = new SiteMiscModel();
            com.ListView = SiteMISC;
            await Task.Run(() =>
            {
                List<SiteModel> Sites = BORepository.GetAllSites(proxy, SiteId).Result;
                if (Sites != null && Sites.Count > 0)
                {
                    com.SiteName = Sites.First().Title;
                    com.SiteID = Sites.First().Oid;
                }
            });
            return View("GetAllSiteMisc", com);
        }

        public ActionResult DeleteRecord(int id, int SiteID)
        {
            string Status = string.Empty;
            SiteMiscModel modeldata = new SiteMiscModel();
            modeldata.Id = id;
           
            Status = base.BaseDeleteRecord(modeldata, ModelState, proxy);

            return RedirectToAction("GetAllSiteMisc", new { SiteId = SiteID });
        }    
    }
}