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
    public class ContentOfViewController : Controller
    {
        WebApiProxy proxy = new WebApiProxy();
        //
        // GET: /ContentOfView/
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> GetAllContentOfView(int SiteId)
        {
            ModelState.Clear();
            List<ContentOfViewModel> ContentView = new List<ContentOfViewModel>();
            List<ViewModel> ObjViewList = new List<ViewModel>();
            List<ContentStyleModel> ObjContentList = new List<ContentStyleModel>();
            await Task.Run(() =>
            {
                ContentView.AddRange(BORepository.GetContentViews(proxy, SiteId).Result.Where(item => item.SiteId == SiteId));
                ObjViewList.AddRange(BORepository.GetAllViews(proxy).Result.Where(i => i.SiteID == SiteId));
                ObjContentList.AddRange(BORepository.GetAllContents(proxy, SiteId).Result);
            });
            CombineContentViewModel combineContentModel = new CombineContentViewModel();
            combineContentModel.ContentViewDetails = new ContentOfViewModel();
            combineContentModel.ContentViewList = ContentView;
            if (ContentView.Count > 0)
            {
                combineContentModel.SiteId = ContentView[0].SiteId;
                combineContentModel.SiteName = ContentView[0].SiteName;
            }
            combineContentModel.ViewList = ObjViewList;
            combineContentModel.ContentList = ObjContentList;
            return View("GetContentView", combineContentModel);
        }
	}
}