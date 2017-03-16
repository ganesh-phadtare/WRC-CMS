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
            ViewBag.CurrSiteID = SiteId;
            List<SiteModel> Sites = new List<SiteModel>();
            List<ContentOfViewModel> ContentView = new List<ContentOfViewModel>();
            List<ViewModel> ObjViewList = new List<ViewModel>();
            List<ContentStyleModel> ObjContentList = new List<ContentStyleModel>();
            ContentOfViewModel obj = new ContentOfViewModel();
            await Task.Run(() =>
            {
                ContentView.AddRange(BORepository.GetContentViews(proxy, SiteId).Result.Where(item => item.SiteId == SiteId));
                ObjViewList.AddRange(BORepository.GetAllViews(proxy).Result.Where(i => i.SiteID == SiteId));
                ObjContentList.AddRange(BORepository.GetAllContents(proxy, SiteId).Result);
                Sites = BORepository.GetAllSites(proxy).Result;
            });
            CombineContentViewModel combineContentModel = new CombineContentViewModel();
            combineContentModel.ContentViewDetails = new ContentOfViewModel();
            combineContentModel.ContentViewList = ContentView;
            combineContentModel.ViewList = ObjViewList;
            combineContentModel.ContentList = ObjContentList;
            combineContentModel.ContentId = -1;
            combineContentModel.ViewId = -1;
            if (ObjContentList.Count > 0)
            {
                combineContentModel.SiteId = ObjContentList[0].SiteID;
                combineContentModel.SiteName = ObjContentList[0].SiteName;                
            }
            combineContentModel.SiteId = SiteId;
            combineContentModel.SiteName = Sites.FirstOrDefault(it => it.Oid == SiteId).Title;
            return View("GetContentView", combineContentModel);
        }

        public ActionResult DeleteContentOfView(int id, int SiteID)
        {
            try
            {
                Dictionary<string, object> dicParams = new Dictionary<string, object>();
                dicParams.Add("@Id", id);
                proxy.ExecuteNonQuery("SP_ContentOfViewDel", dicParams);

                return RedirectToAction("GetAllContentOfView", new { SiteId = SiteID });
            }
            catch
            {
                return View();
            }
        }

        public async Task<ActionResult> CreateUpdContentOfView(int ContentId, int ViewId, int SiteId, int Order, int Id = 0)
        {
            try
            {
                if (ContentId == -1 || ViewId == -1)
                    return RedirectToAction("GetAllContentOfView", new { SiteId = SiteId });

                if (ModelState.IsValid)
                {
                    int ContentID = 0;
                    if (Id == 0)
                        Id = -1;

                    Dictionary<string, object> dicParams = new Dictionary<string, object>();

                    dicParams.Add("@Id", Id);
                    dicParams.Add("@ContentId", ContentId);
                    dicParams.Add("@ViewId", ViewId);
                    dicParams.Add("@SiteId", SiteId);
                    dicParams.Add("@Order", Order);

                    //await Task.Run(() =>
                    //{
                    DataSet dataSet = await proxy.ExecuteDataset("SP_ContentOfViewAddUp", dicParams);
                    //});

                    if (dataSet != null && dataSet.Tables != null && dataSet.Tables.Count > 0)
                    {
                        if (dataSet.Tables[0].Rows != null && dataSet.Tables[0].Rows.Count > 0)
                            ContentID = Convert.ToInt32(dataSet.Tables[0].Rows[0][0].ToString());
                    }

                    if (ContentID > 0)
                        ViewBag.Message = "Content of View added successfully.";
                    else
                        ViewBag.Message = "Problem occured while adding content, kindly contact our support team.";

                    ActionResult MainView = null;
                    await Task.Run(() =>
                    {
                        MainView = GetAllContentOfView(SiteId).Result;
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

        public async Task<ActionResult> EditContentOfView(int id, int SiteID)
        {
            if (id != 0)
            {
                List<ContentOfViewModel> ContentView = new List<ContentOfViewModel>();
                List<ContentStyleModel> ObjContentList = new List<ContentStyleModel>();
                List<ViewModel> ObjViewList = new List<ViewModel>();
                await Task.Run(() =>
                {
                    ContentView.AddRange(BORepository.GetContentViews(proxy, SiteID).Result.Where(item => item.SiteId == SiteID));
                    ObjViewList.AddRange(BORepository.GetAllViews(proxy).Result.Where(i => i.SiteID == SiteID));
                    ObjContentList.AddRange(BORepository.GetAllContents(proxy, SiteID).Result);
                });
                CombineContentViewModel combineContentModel = new CombineContentViewModel();
                combineContentModel.ContentViewList = ContentView;
                combineContentModel.ViewList = ObjViewList;
                combineContentModel.ContentList = ObjContentList;
                combineContentModel.ContentViewDetails = ContentView.FirstOrDefault(item => item.Id == id);

                foreach (var item in ContentView)
                {
                    if (item.Id == id)
                    {
                        combineContentModel.ContentId = item.ContentId;
                        combineContentModel.ViewId = item.ViewId;
                    }
                }

                if (ContentView.Count > 0)
                {
                    combineContentModel.SiteName = ContentView[0].SiteName;
                    combineContentModel.SiteId = SiteID;
                }

                ViewBag.CurrSiteID = SiteID;
                combineContentModel.SiteId = SiteID;
                ViewBag.DepartmentId = new SelectList(combineContentModel.ContentList, "ContentId", "ContentName", combineContentModel.ContentId);

                return View("GetContentView", combineContentModel);
            }
            else
            {
                return RedirectToAction("GetAllContentOfView", new { SiteId = SiteID });
            }
        }

    }
}