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
    public class SiteMiscController : Controller
    {
        WebApiProxy proxy = new WebApiProxy();

        public async Task<ActionResult> AddSiteMisc(string Key, string Value, int Oid, int SiteID)
        {
            if (string.IsNullOrEmpty(Key) || string.IsNullOrEmpty(Value))
                return RedirectToAction("GetAllSiteMisc");

            SiteMiscModel SiteMisc = new SiteMiscModel();
            if (Oid > 0)
                SiteMisc.Id = Oid;
            SiteMisc.Key = Key;
            SiteMisc.Value = Value;
            SiteMisc.SiteId = SiteID;
            try
            {
                if (ModelState.IsValid)
                {
                    int SiteDBID = 0;

                    await Task.Run(() =>
                    {
                        if (Oid == 0)
                            SiteDBID = BORepository.AddSiteMisc(proxy, SiteMisc, true).Result;
                        else
                            SiteDBID = BORepository.AddSiteMisc(proxy, SiteMisc, false).Result;
                    });
                    if (SiteDBID > 0)
                        ViewBag.Message = "Site Misc added successfully.";
                    else
                        ViewBag.Message = "Problem occured while adding Site Misc, kindly contact our support team.";

                    List<SiteMiscModel> SiteMiscs = new List<SiteMiscModel>();
                    await Task.Run(() =>
                    {
                        SiteMiscs.AddRange(BORepository.GetAllSiteMISC(proxy).Result.Where(item => item.SiteId == SiteID));
                    });

                    ActionResult MainView = null;
                    await Task.Run(() =>
                    {
                        MainView = ReturnToMainView(SiteID).Result;
                    });
                    return MainView;

                }

                return View();
            }
            catch
            {
                return View();
            }
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
            ViewBag.CurrSiteID = SiteId;
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
    }
}