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
    public class DashboardController : Controller
    {
        WebApiProxy proxy = new WebApiProxy();
        //
        // GET: /Dashboard/
        public async Task<ActionResult> Index()
        {
            ModelState.Clear();
            List<SiteModel> sites = new List<SiteModel>();
            await Task.Run(() =>
            {
                sites.AddRange(BORepository.GetAllSites(proxy).Result);
            });
            return View("Index", sites);
        }

        public ActionResult Home()
        {
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Index1(string viewObject)
        {
            ModelState.Clear();
            List<SiteModel> sites = new List<SiteModel>();
            await Task.Run(() =>
            {
                sites.AddRange(BORepository.GetSearchSite(proxy, viewObject).Result);
            });
            return View("Index", sites);
        }
    }
}