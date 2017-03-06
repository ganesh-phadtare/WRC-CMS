﻿using System;
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

namespace WRC_CMS.Controllers
{
    public class ContentStyleController : Controller
    {
        WebApiProxy proxy = new WebApiProxy();
        public int PubSiteID = 0;
        #region OldCode
        //public async Task<ActionResult> AddContentStyle()
        //{
        //    ContentStyleModel StyleObject = new ContentStyleModel();
        //    if (StyleObject != null)
        //    {
        //        await Task.Run(() =>
        //        {
        //            StyleObject.View = BORepository.GetAllViews(proxy).Result;
        //        });
        //    }
        //    return View(StyleObject);
        //}

        //[HttpPost]
        //public async Task<ActionResult> AddContentStyle(ContentStyleModel ContentStyleModelObject)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            if (!string.IsNullOrEmpty(ContentStyleModelObject.SelectView))
        //                ContentStyleModelObject.ViewID = Convert.ToInt32(ContentStyleModelObject.SelectView.ToString());

        //            ContentStyleModelObject.Description = RepairLinks(ContentStyleModelObject.Description);

        //            int ContentStyleID = 0;
        //            await Task.Run(() =>
        //            {
        //                ContentStyleID = BORepository.AddContentStyle(proxy, ContentStyleModelObject).Result;
        //            });
        //            if (ContentStyleID > 0)
        //                ViewBag.Message = "Content Style added successfully.";
        //            else
        //                ViewBag.Message = "Problem occured while adding content, kindly contact our support team.";
        //            return RedirectToAction("GetAllContentsDetails");
        //        }
        //        return View();
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //public ActionResult ShowContent(ContentStyleModel ContentStyleModelObject)
        //{
        //    ViewBag.Message = ContentStyleModelObject.Description;
        //    return View("ShowContent");
        //}

        //public async Task<ActionResult> EditContentDetails(int id)
        //{
        //    //List<ContentStyleModel> contents = new List<ContentStyleModel>();
        //    //await Task.Run(() =>
        //    //{
        //    //    contents.AddRange(GetAllContents(SiteID).Result);
        //    //});
        //    //if (contents != null && contents.Count > 0)
        //    //{
        //    //    ContentStyleModel objetc = contents.FirstOrDefault(item => item.Oid == id);
        //    //    await Task.Run(() =>
        //    //    {
        //    //        objetc.View = BORepository.GetAllViews(proxy).Result;
        //    //    });
        //    //    objetc.SelectView = objetc.ViewID.ToString();
        //    //    return View(objetc);
        //    //}             
        //    //return View();
        //    ModelState.Clear();
        //    List<ContentStyleModel> contents = new List<ContentStyleModel>();
        //    await Task.Run(() =>
        //    {
        //        contents.AddRange(GetAllContents(SiteID).Result.Where(item => item.Oid == id));
        //    });

        //    CombineContentModel combineContentModel = new CombineContentModel();
        //    combineContentModel.ContentView = new ContentStyleModel();
        //    combineContentModel = contents;

        //    if (contents.Count > 0)
        //        combineContentModel.SelectView = contents[0].SelectView;

        //    return View("ContentPanel", combineContentModel);  
        //}      

        //[HttpPost]
        //public ActionResult EditContentDetails(ContentStyleModel ContentStyleModelObject)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            if (!string.IsNullOrEmpty(ContentStyleModelObject.SelectView))
        //                ContentStyleModelObject.ViewID = Convert.ToInt32(ContentStyleModelObject.SelectView.ToString());
        //            Dictionary<string, object> dicParams = new Dictionary<string, object>();
        //            dicParams.Add("@Oid", ContentStyleModelObject.Oid);
        //            dicParams.Add("@Name", ContentStyleModelObject.Name);
        //            dicParams.Add("@Descr", RepairLinks(ContentStyleModelObject.Description));
        //            dicParams.Add("@View", ContentStyleModelObject.ViewID);
        //            if (ContentStyleModelObject.IsActive)
        //                dicParams.Add("@IsActive", "1");
        //            else
        //                dicParams.Add("@IsActive", "0");
        //            proxy.ExecuteNonQuery("SP_StaticContentsAddUp", dicParams);
        //            return RedirectToAction("GetAllContentsDetails");
        //        }

        //        return View();
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}
        //string RepairLinks(string value)
        //{
        //    HtmlDocument doc = new HtmlDocument();
        //    doc.LoadHtml(value);

        //    int i = doc.DocumentNode.SelectNodes("//a[@href]").Count;
        //    foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//a[@href]"))
        //    {
        //        HtmlAttribute att = link.Attributes["href"];
        //        att.Value = FixLink(att);
        //    }

        //    System.IO.StringWriter srw = new System.IO.StringWriter();
        //    doc.Save(srw as System.IO.TextWriter);

        //    if (srw != null)
        //        return srw.ToString();

        //    return value;
        //}

        //public string FixLink(HtmlAttribute att)
        //{
        //    if (!string.IsNullOrEmpty(att.Value) && !att.Value.StartsWith(AppKeys.DefaultRespondURL))
        //    {
        //        return string.Concat(AppKeys.DefaultRespondURL, att.Value);
        //    }
        //    return att.Value;
        //}
        #endregion

        public ActionResult DeleteContent(int id, bool IsDefault)
        {
            try
            {
                if (!IsDefault)
                {
                    Dictionary<string, object> dicParams = new Dictionary<string, object>();
                    dicParams.Add("@Oid", id);
                    proxy.ExecuteNonQuery("SP_StaticContentsDel", dicParams);
                }
                else
                {
                    ViewBag.Message = "Site must have atleast one content.";
                }
                return RedirectToAction("GetContentPage");
            }
            catch
            {
                return View();
            }
        }


        //public async Task<ActionResult> GetAllContentsDetails()
        //{
        //    ModelState.Clear();
        //    List<ContentStyleModel> views = new List<ContentStyleModel>();
        //    await Task.Run(() =>
        //    {
        //        views.AddRange(GetAllContents(0).Result);
        //    });
        //    return View("GetContentsDetails", views);
        //}

        public async Task<List<ContentStyleModel>> GetAllContents(int SiteId = 0, int ViewId = 0)
        {

            List<ContentStyleModel> ContentList = new List<ContentStyleModel>();
            List<ViewModel> Views = BORepository.GetAllViews(proxy).Result;
            //List<SiteModel> Sites = BORepository.GetAllSites(proxy).Result;
            Dictionary<string, object> dict = new Dictionary<string, object>();

            dict.Add("@Oid", -1);
            dict.Add("@LoadOnlyActive", 0);
            dict.Add("@SiteId", SiteId);
            dict.Add("@ViewId", ViewId);

            var dataSet = await proxy.ExecuteDataset("SP_StaticContentsSelect", dict);
            //ViewBag.ViewList = dataSet.Tables[1].ToString();
            return (from DataRow row in dataSet.Tables[0].Rows
                    select new ContentStyleModel
                    {
                        Oid = Convert.ToInt32(row["Oid"].ToString()),
                        Name = row["Name"].ToString(),
                        Description = row["Descr"].ToString(),
                        IsActive = bool.Parse(row["IsActive"].ToString()),
                        ViewID = Convert.ToInt32(row["Views"].ToString()),
                        //SelectView = Sites.FirstOrDefault(sit => sit.Oid == Views.FirstOrDefault(it => it.Oid == Convert.ToInt32(row["Views"].ToString())).SiteID).Name + " - " + Views.FirstOrDefault(it => it.Oid == Convert.ToInt32(row["Views"].ToString())).Name,
                        SelectView = Views.FirstOrDefault(VId => VId.Oid == Convert.ToInt32(row["Views"].ToString())).Title,
                        //SelectView = Views.FirstOrDefault(VId => VId.Oid == (ViewId==0?Convert.ToInt32(row["Views"].ToString()):ViewId) ).Title,
                        SiteID = Convert.ToInt32(row["Site"].ToString()),
                        SiteName = row["SiteName"].ToString(),
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

            await Task.Run(() =>
            {
                contents.AddRange(GetAllContents(SiteId, 0).Result);
                ObjViewList.AddRange(BORepository.GetAllViews(proxy).Result.Where(i => i.SiteID == SiteId));
            });

            combineContentModel.ContentView = new ContentStyleModel();
            combineContentModel.ContentList = contents;
            combineContentModel.ViewList = ObjViewList;

            if (contents.Count > 0)
            {
                combineContentModel.SelectView = contents[0].SelectView; //View Name
                combineContentModel.SiteName = contents[0].SiteName;
                combineContentModel.SiteID = contents[0].SiteID;
            }

            return View("ContentPanel", combineContentModel);
        }

        public async Task<ActionResult> CreateUpdContent(string Name, string Description, int ViewID, string IsActive)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    int ContentID = 0;
                    Dictionary<string, object> dicParams = new Dictionary<string, object>();
                    dicParams.Add("@Oid", "-1");
                    dicParams.Add("@Name", Name);
                    dicParams.Add("@Descr", Description);
                    dicParams.Add("@View", ViewID);
                    dicParams.Add("@IsActive", IsActive);

                    //DataSet dataSet = null;
                    //await Task.Run(() =>
                    //{
                    DataSet dataSet = await proxy.ExecuteDataset("SP_StaticContentsAddUp", dicParams);
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

                    ModelState.Clear();
                    List<ContentStyleModel> contents = new List<ContentStyleModel>();
                    await Task.Run(() =>
                    {
                        contents.AddRange(GetAllContents(0).Result);
                    });
                    CombineContentModel combineContentModel = new CombineContentModel();
                    combineContentModel.ContentView = new ContentStyleModel();
                    combineContentModel.ContentList = contents;

                    if (contents.Count > 0)
                    {
                        combineContentModel.SelectView = contents[0].SelectView; //View Name
                        combineContentModel.SiteName = contents[0].SiteName;
                    }


                    return View("ContentPanel", combineContentModel);
                }
                return View();
            }
            catch
            {
                return View();
            }
        }

        public async Task<ActionResult> GetEdiContentPage(int Contentid = 0, int EViewId = 0, int tid = 0)
        {
            ModelState.Clear();
            if (tid != 0)
            {
                // int temp = ViewBag.Site;
                List<ContentStyleModel> contents = new List<ContentStyleModel>();
                List<ViewModel> ObjViewList = new List<ViewModel>();
                CombineContentModel combineContentModel = new CombineContentModel();

                await Task.Run(() =>
                {
                    contents.AddRange(GetAllContents(0, EViewId).Result);
                    //contents.AddRange(GetSelectedContent(Contentid).Result.Where(item => item.Oid == Contentid));                    
                });

                foreach (var item in contents)
                {
                    if (Convert.ToInt32(item.Oid.ToString()) == Contentid)
                        combineContentModel.SelectView = item.SelectView;
                }

                if (contents.Count > 0)
                {
                    //combineContentModel.SelectView = contents[Contentid].SelectView;  //View Name
                    combineContentModel.SiteName = contents[0].SiteName;
                    combineContentModel.SiteID = contents[0].SiteID;
                    PubSiteID = (from cont in contents
                                 where cont.Oid == Contentid
                                 select cont.SiteID).First();
                }
                await Task.Run(() =>
                {
                    ObjViewList.AddRange(BORepository.GetAllViews(proxy).Result.Where(i => i.SiteID == PubSiteID));
                });
                combineContentModel.ContentView = contents.FirstOrDefault(item => item.Oid == Contentid);
                combineContentModel.ContentList = contents;
                combineContentModel.ViewList = ObjViewList;


                return View("ContentPanel", combineContentModel);
            }
            else
            {
                return RedirectToAction("CreateUpdContent");
            }
        }

        //public async Task<List<ContentStyleModel>> GetSelectedContent(int Contentid)
        //{

        //    List<ContentStyleModel> ContentList = new List<ContentStyleModel>();
        //    List<ViewModel> Views = BORepository.GetAllViews(proxy).Result;            
        //    Dictionary<string, object> dict = new Dictionary<string, object>();
        //    dict.Add("@ContentId", Contentid);

        //    var dataSet = await proxy.ExecuteDataset("SP_GetSelectedContent", dict);            
        //    return (from DataRow row in dataSet.Tables[0].Rows
        //            select new ContentStyleModel
        //            {
        //                Oid = Convert.ToInt32(row["Oid"].ToString()),
        //                Name = row["Name"].ToString(),
        //                Description = row["Descr"].ToString(),
        //                IsActive = bool.Parse(row["IsActive"].ToString()),
        //                ViewID = Convert.ToInt32(row["Views"].ToString()),
        //                //SelectView = Sites.FirstOrDefault(sit => sit.Oid == Views.FirstOrDefault(it => it.Oid == Convert.ToInt32(row["Views"].ToString())).SiteID).Name + " - " + Views.FirstOrDefault(it => it.Oid == Convert.ToInt32(row["Views"].ToString())).Name,
        //                SelectView = Views.FirstOrDefault(VId => VId.Oid == Convert.ToInt32(row["Views"].ToString())).Title,
        //            }).ToList();
        //}
    }
}
