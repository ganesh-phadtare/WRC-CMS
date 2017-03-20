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
using HtmlAgilityPack;
using Newtonsoft.Json;

namespace WRC_CMS.Controllers
{
    public class ContentStyleController : BaseController
    {
        WebApiProxy proxy = new WebApiProxy();
        public int PubSiteID = 0;

        public ActionResult DeleteContent(int id, bool IsDefault, int Siteid)
        {
            try
            {
                if (!IsDefault)
                {
                    Dictionary<string, object> dicParams = new Dictionary<string, object>();
                    dicParams.Add("@Id", id);
                    proxy.ExecuteNonQuery("SP_ContentsDel", dicParams);
                }
                else
                {
                    ViewBag.Message = "Site must have atleast one content.";
                }
                return RedirectToAction("GetContentPage", new { SiteId = Siteid });
            }
            catch
            {
                return View();
            }
        }

        public async Task<List<ContentStyleModel>> GetAllContents(int SiteId, int ContentId)
        {
            if (ContentId == 0)
                ContentId = -1;

            List<ContentStyleModel> ContentList = new List<ContentStyleModel>();
            List<SiteModel> Sites = BORepository.GetAllSites(proxy).Result;
            Dictionary<string, object> dict = new Dictionary<string, object>();

            dict.Add("@Id", ContentId);
            dict.Add("@LoadOnlyActive", 0);
            dict.Add("@SiteId", SiteId);

            var dataSet = await proxy.ExecuteDataset("SP_ContentsSelect", dict);
            return (from DataRow row in dataSet.Tables[0].Rows
                    select new ContentStyleModel
                    {
                        Id = Convert.ToInt32(row["Id"].ToString()),
                        Name = row["Name"].ToString(),
                        Description = row["Description"].ToString(),
                        IsActive = bool.Parse(row["IsActive"].ToString()),
                        SiteID = Convert.ToInt32(row["SiteId"].ToString()),
                        Type = Convert.ToInt32(row["Type"].ToString()),
                        Orientation = row["Orientation"].ToString(),
                        Data = (row["Data"].ToString() == "") ? "" : JsonConvert.DeserializeObject(row["Data"].ToString()).ToString(),
                        SiteName = Sites.FirstOrDefault(sit => sit.Oid == SiteId).Name,
                    }).ToList();
        }

        public async Task<ActionResult> GetContentPage(int SiteId = 0)
        {
            //SiteID = SiteId;
            ViewBag.Site = SiteId;
            ModelState.Clear();
            List<ContentStyleModel> contents = new List<ContentStyleModel>();
            List<ViewModel> ObjViewList = new List<ViewModel>();
            CombineContentModel combineContentModel = new CombineContentModel();
            List<SiteModel> Sites = new List<SiteModel>();

            await Task.Run(() =>
            {
                contents.AddRange(GetAllContents(SiteId, 0).Result);
                ObjViewList.AddRange(BORepository.GetAllViews(proxy).Result.Where(i => i.SiteID == SiteId));
                Sites = BORepository.GetAllSites(proxy).Result;
            });

            combineContentModel.ContentView = new ContentStyleModel();
            combineContentModel.ContentView.Type = -1;
            combineContentModel.ContentView.SearchType = -1;
            combineContentModel.ContentList = contents;
            combineContentModel.ViewList = ObjViewList;
            combineContentModel.ContentView.Orientation = "1";

            if (contents.Count > 0)
                combineContentModel.SiteName = contents[0].SiteName;

            combineContentModel.SiteID = SiteId;
            combineContentModel.SiteName = Sites.FirstOrDefault(it => it.Oid == SiteId).Title;
            PubSiteID = SiteId;
            return View("ContentPanel", combineContentModel);
        }

        public async Task<ActionResult> CreateUpdContent(string Name, int CType, string Orientation, string Data, string Description, string IsActive, int Siteid, string ViewList, string SearchType = "", int Id = 0)
        {
            try
            {
                if (string.IsNullOrEmpty(Name) || CType == -1 || (CType == 1 && (Convert.ToInt32((Orientation == "" ? "0" : Orientation)) <= 0 || Convert.ToInt32((Orientation == "" ? "0" : Orientation)) >= 5)))
                    return RedirectToAction("GetContentPage", new { SiteId = Siteid });

                if (ModelState.IsValid)
                {
                    int ContentID = 0;
                    if (Id == 0)
                        Id = -1;
                    Dictionary<string, object> ContentData = new Dictionary<string, object>();
                    Dictionary<string, object> dicParams = new Dictionary<string, object>();

                    if (CType == 0)
                        ContentData.Add("sd", Data);
                    else if (CType == 1)
                        ContentData.Add("v", ViewList.ToString().Substring(0, ViewList.Length - 1));
                    else if (CType == 2)
                        ContentData.Add("st", SearchType.ToString().Substring(0, SearchType.Length - 1));

                    dicParams.Add("@Id", Id);
                    dicParams.Add("@Name", Name);
                    dicParams.Add("@Type", CType);
                    dicParams.Add("@Orientation", Orientation);
                    dicParams.Add("@Data", JsonConvert.SerializeObject(ContentData));
                    dicParams.Add("@Description", Description);
                    dicParams.Add("@IsActive", Convert.ToBoolean(IsActive));
                    dicParams.Add("@Siteid", Siteid);
                    //await Task.Run(() =>
                    //{
                    DataSet dataSet = await proxy.ExecuteDataset("SP_ContentsAddUp", dicParams);
                    //});

                    if (dataSet != null && dataSet.Tables != null && dataSet.Tables.Count > 0)
                    {
                        if (dataSet.Tables[0].Rows != null && dataSet.Tables[0].Rows.Count > 0)
                            ContentID = Convert.ToInt32(dataSet.Tables[0].Rows[0][0].ToString());
                    }

                    if (ContentID > 0)
                        ViewBag.Message = "Content Style added successfully.";
                    else
                        ViewBag.Message = "Problem occured while adding content, kindly contact our support team.";

                    ActionResult MainView = null;
                    await Task.Run(() =>
                    {
                        MainView = GetContentPage(Siteid).Result;
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

        public async Task<ActionResult> GetEdiContentPage(int Contentid = 0, int SiteId = 0)
        {
            ModelState.Clear();
            if (Contentid != 0)
            {
                // int temp = ViewBag.Site;
                List<ContentStyleModel> contents = new List<ContentStyleModel>();
                List<ViewModel> ObjViewList = new List<ViewModel>();
                CombineContentModel combineContentModel = new CombineContentModel();
                ContentStyleModel objContentstyle = new ContentStyleModel();
                Dictionary<string, object> dictData = new Dictionary<string, object>();
                //List<int> STyList =new List<int> ();

                await Task.Run(() =>
                {
                    contents.AddRange(GetAllContents(SiteId, 0).Result);
                    ObjViewList.AddRange(BORepository.GetAllViews(proxy).Result.Where(i => i.SiteID == SiteId));
                });

                if (contents.Count > 0)
                {
                    combineContentModel.SiteName = contents[0].SiteName;
                    combineContentModel.SiteID = contents[0].SiteID;
                }

                combineContentModel.SiteID = SiteId;
                PubSiteID = SiteId;
                combineContentModel.ContentView = contents.FirstOrDefault(item => item.Id == Contentid);
                combineContentModel.ContentList = contents;
                combineContentModel.ViewList = ObjViewList;
                //ViewBag.Site = PubSiteID;
                foreach (var item in contents)
                {
                    if (item.Id == Contentid)
                    {
                        combineContentModel.ContentView.Orientation = item.Orientation;
                        combineContentModel.ContentView.Type = item.Type;
                        dictData = JsonConvert.DeserializeObject<Dictionary<string, object>>(item.Data.ToString());
                        foreach (var _item in dictData)
                        {
                            if (_item.Key == "sd")
                                combineContentModel.ContentView.Data = _item.Value.ToString();
                            else if (_item.Key == "st")
                                combineContentModel.ContentView.STyList = new List<int>(Array.ConvertAll(_item.Value.ToString().Split(','), int.Parse));
                            else if (_item.Key == "v")
                                combineContentModel.ContentView.VTyList = new List<int>(Array.ConvertAll((string.IsNullOrEmpty(_item.Value.ToString()) ? -1 : _item.Value).ToString().Split(','), int.Parse));
                            else
                            { }
                        }
                    }
                }

                return View("ContentPanel", combineContentModel);
            }
            else
            {
                return RedirectToAction("GetContentPage", new { SiteId = SiteId });
            }
        }

        public async Task<JsonResult> AddUpdateRecord(ContentStyleModel modeldata)
        {
            Dictionary<string, object> ContentData = new Dictionary<string, object>();

            if (modeldata.Type == 0)
                ContentData.Add("sd", modeldata.Data);
            else if (modeldata.Type == 1)
                ContentData.Add("v", modeldata.views.ToString().Substring(0, modeldata.views.Length - 1));
            else if (modeldata.Type == 2)
                ContentData.Add("st", modeldata.SearchType.ToString().Substring(0, modeldata.searchty.Length - 1));

            modeldata.Data = JsonConvert.SerializeObject(ContentData);

            string Status = string.Empty;
            await Task.Run(() =>
            {
                Status = base.BaseAddUpdateRecord(modeldata, ModelState, proxy).Result;
            }
            );
            return Json(new { status = Status });
        }
    }
}
