using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WRC_CMS.Communication;
using WRC_CMS.Models;
using WRC_CMS.Repository;



using Newtonsoft.Json;
using System.IO;
namespace WRC_CMS.Controllers
{
    public class SiteController : Controller
    {
        WebApiProxy proxy = new WebApiProxy();
        public ActionResult AddSite()
        {

            return View();
        }

        public async Task<ActionResult> PreviewSite(int id)
        {
            List<SiteModel> sites = new List<SiteModel>();
            List<ViewModel> views = new List<ViewModel>();
            await Task.Run(() =>
            {
                sites.AddRange(BORepository.GetAllSites(proxy).Result);
            });
            await Task.Run(() =>
            {
                views.AddRange(BORepository.GetAllViews(proxy).Result);
            });
            if (sites != null && sites.Count > 0)
            {
                SiteModel objetc = sites.FirstOrDefault(item => item.Oid == id);
                if (views != null && views.Count > 0)
                {
                    objetc.ViewList = new List<ViewModel>();
                    objetc.ViewList.AddRange(views.Where(items => items.SiteID == objetc.Oid));
                }
                return View("PreviewSite", objetc);
            }
            return View();
        }

        public async Task<ActionResult> ShowViewContent(int SiteID, int ViewID)
        {
            List<SiteModel> sites = new List<SiteModel>();
            List<ViewModel> views = new List<ViewModel>();
            List<ContentStyleModel> contents = new List<ContentStyleModel>();
            await Task.Run(() =>
            {
                sites.AddRange(BORepository.GetAllSites(proxy).Result);
            });
            await Task.Run(() =>
            {
                views.AddRange(BORepository.GetAllViews(proxy).Result);
            });
            if (sites != null && sites.Count > 0)
            {
                SiteModel objetc = sites.FirstOrDefault(item => item.Oid == SiteID);
                if (views != null && views.Count > 0)
                {
                    objetc.ViewList = new List<ViewModel>();
                    objetc.ViewList.AddRange(views.Where(items => items.SiteID == objetc.Oid));
                    var SelectetdView = objetc.ViewList.FirstOrDefault(ite => ite.Oid == ViewID);
                    if (!ReferenceEquals(SelectetdView, null))
                    {
                        await Task.Run(() =>
                        {
                            //contents.AddRange(BORepository.GetAllContents(proxy).Result);
                        });
                        SelectetdView.Contents = new List<ContentStyleModel>();
                        SelectetdView.Contents.AddRange(contents.Where(ite => ite.ViewID == ViewID));
                        if (SelectetdView.Contents != null && SelectetdView.Contents.Count > 0)
                        {
                            StringBuilder strb = new StringBuilder();
                            foreach (var cont in SelectetdView.Contents)
                            {
                                strb.AppendLine(cont.Description);
                            }
                            ViewBag.Message = strb.ToString();
                        }
                    }
                }
                return View("PreviewSite", objetc);
            }
            return View();
        }

        //public JsonResult SiteExists(string Name)
        //{
        //    var user = Name;
        //    return user == null ?
        //        Json(true, JsonRequestBehavior.AllowGet) :
        //        Json(string.Format("{0} Already Exist.", Name),
        //            JsonRequestBehavior.AllowGet);
        //}


        //        [HttpPost]
        //        public async Task<ActionResult> AddSite(SiteModel SiteObject, HttpPostedFileBase file)
        //        {
        //            try
        //            {
        //                if (file != null && file.ContentLength > 0)
        //                {
        //                    SiteObject.Logo = new byte[file.ContentLength];
        //                    file.InputStream.Read(SiteObject.Logo, 0, file.ContentLength);
        //                }
        //                if (ModelState.IsValid)
        //                {
        //                    Dictionary<string, object> dicParams = new Dictionary<string, object>();
        //                    dicParams.Add("@Id", -1);
        //                    dicParams.Add("@Name", SiteObject.Name);
        //                    dicParams.Add("@url", SiteObject.URL);
        //                    dicParams.Add("@Logo", 0101);
        //                    dicParams.Add("@Title", SiteObject.Title);
        //                    if (SiteObject.IsActive)
        //                        dicParams.Add("@IsActive", "1");
        //                    else
        //                        dicParams.Add("@IsActive", "0");
        //                    DataSet dataSet = null;
        //                    await Task.Run(() =>
        //                    {
        //                        dataSet = proxy.ExecuteDataset("SP_SiteAddUp", dicParams).Result;
        //                    });

        //                    int SiteID = 0;
        //                    if (dataSet != null && dataSet.Tables != null && dataSet.Tables.Count > 0)
        //                    {
        //                        if (dataSet.Tables[0].Rows != null && dataSet.Tables[0].Rows.Count > 0)
        //                        {
        //                            SiteID = Convert.ToInt32(dataSet.Tables[0].Rows[0][0].ToString());

        //                            ViewModel DefaultView = new ViewModel();
        //                            DefaultView.Name = "Home";

        //                            DefaultView.Title = "Home";
        //                            DefaultView.IsActive = true;
        //                            DefaultView.IsDefault = true;
        //                            DefaultView.Authorized = true;
        //                            DefaultView.SiteID = SiteID;
        //                            DefaultView.CreateMenu = true;
        //                            DefaultView.IsDefault = true;
        //                            int ViewID = 0;
        //                            await Task.Run(() =>
        //                            {
        //                                ViewID = BORepository.AddView(proxy, DefaultView, true).Result;
        //                            });
        //                            if (ViewID > 0)
        //                            {
        //                                ContentStyleModel DefaultContent = new ContentStyleModel();
        //                                DefaultContent.Name = "Welcome";
        //                                string welcomebody = @"<p><span style='font-size: medium;'><b><span style='text-decoration: underline;'>This is our default template.</span></b></span></p>
        //<p><strong><span style='text-decoration: underline;'>Welcome to our site.<img src='http://localhost:49791/Scripts/tinymce/plugins/emotions/img/smiley-smile.gif' alt='Smile' title='Smile' border='0' /></span></strong></p>";
        //                                DefaultContent.Description = welcomebody;

        //                                DefaultContent.IsActive = true;
        //                                DefaultContent.ViewID = ViewID;
        //                                DefaultContent.IsDefault = true;
        //                                int ContentID = 0;
        //                                await Task.Run(() =>
        //                                {
        //                                    ContentID = BORepository.AddContentStyle(proxy, DefaultContent).Result;
        //                                });
        //                            }
        //                        }
        //                    }
        //                    if (SiteID != 0)
        //                        ViewBag.Message = "Site added successfully.";
        //                    else
        //                        ViewBag.Message = "Problem occured while creating site, kindly contact our support team.";
        //                }
        //                return View();
        //            }
        //            catch
        //            {
        //                return View();
        //            }
        //        }

        [HttpPost]
        public ActionResult EditSiteDetails(SiteModel SiteObject, HttpPostedFileBase file)
        {
            try
            {
                if (file != null && file.ContentLength > 0)
                {
                    SiteObject.Logo = new byte[file.ContentLength];
                    file.InputStream.Read(SiteObject.Logo, 0, file.ContentLength);
                }
                if (ModelState.IsValid)
                {
                    Dictionary<string, object> dicParams = new Dictionary<string, object>();
                    dicParams.Add("@Id", SiteObject.Oid);
                    dicParams.Add("@Name", SiteObject.Name);
                    dicParams.Add("@url", SiteObject.URL);
                    dicParams.Add("@Logo", CommonClass.GetImage(file.InputStream));
                    dicParams.Add("@Title", SiteObject.Title);
                    if (SiteObject.IsActive)
                        dicParams.Add("@IsActive", "1");
                    else
                        dicParams.Add("@IsActive", "0");
                    proxy.ExecuteNonQuery("SP_SiteAddUp", dicParams);
                    return RedirectToAction("GetAllSitesDetails");
                }
                return View();
            }
            catch
            {
                return View();
            }
        }

        public async Task<ActionResult> EditSiteDetails(int id, int tid = 0)
        {
            ModelState.Clear();

            if (tid != 0)
            {
                List<SiteModel> sites = new List<SiteModel>();
                await Task.Run(() =>
                {
                    sites.AddRange(BORepository.GetAllSites(proxy).Result);
                });
                CombineSiteModel siteObject = new CombineSiteModel();
                siteObject.SiteView = sites.FirstOrDefault(site => site.Oid == id);
                siteObject.SiteList = sites;
                return View("AddSite1", siteObject);
            }
            else
            {
                return RedirectToAction("GetAllSite");
            }
        }

        public ActionResult DeleteSite(int id)
        {
            try
            {
                Dictionary<string, object> dicParams = new Dictionary<string, object>();
                dicParams.Add("@Id", id);
                proxy.ExecuteNonQuery("SP_SiteDel", dicParams);
                //return RedirectToAction("GetAllSitesDetails");
                return RedirectToAction("GetAllSite");
            }
            catch
            {
                return View();
            }
        }

        public async Task<ActionResult> GetAllSitesDetails()
        {
            ModelState.Clear();
            List<SiteModel> sites = new List<SiteModel>();
            await Task.Run(() =>
            {
                sites.AddRange(BORepository.GetAllSites(proxy).Result);
            });
            return View("GetSitesDetails", sites);
        }

        public async Task<ActionResult> GetAllSite()
        {
            ModelState.Clear();
            List<SiteModel> sites = new List<SiteModel>();
            await Task.Run(() =>
            {
                sites.AddRange(BORepository.GetAllSites(proxy).Result);
            });

            CombineSiteModel siteObject = new CombineSiteModel();
            siteObject.SiteList = sites;
            siteObject.SiteView = new SiteModel();
            return View("AddSite1", siteObject);
        }

        public async Task<ActionResult> CreateSite(int Oid, string Name, string URL, string Title, string IsActive, object Logo)
        {
            if (Oid == 0)
                Oid = -1;

            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(URL) || string.IsNullOrEmpty(Title))
                return RedirectToAction("GetAllSite");
            try
            {
                if (ModelState.IsValid)
                {
                    Dictionary<string, object> dicParams = new Dictionary<string, object>();
                    dicParams.Add("@Id", Oid);
                    dicParams.Add("@Name", Name);
                    dicParams.Add("@url", URL);
                    dicParams.Add("@Logo", new ComplexDataModel(typeof(Byte[]), CommonClass.GetImage(Convert.ToString(Logo))));
                    dicParams.Add("@Title", Title);
                    dicParams.Add("@IsActive", Convert.ToBoolean(IsActive));

                    DataSet dataSet = null;
                    await Task.Run(() =>
                    {
                        dataSet = proxy.ExecuteDataset("SP_SiteAddUp", dicParams).Result;
                    });

                    int SiteID = 0;
                    if (Oid == -1)
                    {
                        if (dataSet != null && dataSet.Tables != null && dataSet.Tables.Count > 0)
                        {
                            if (dataSet.Tables[0].Rows != null && dataSet.Tables[0].Rows.Count > 0)
                            {
                                SiteID = Convert.ToInt32(dataSet.Tables[0].Rows[0][0].ToString());

                                ViewModel DefaultView = new ViewModel();
                                DefaultView.Name = "Home";
                                DefaultView.Title = "Home";
                                DefaultView.Logo = (byte[])CommonClass.GetImage(Server.MapPath(@"..\images\V2ViewLogo.png"));
                                DefaultView.Orientation = "0";
                                DefaultView.IsActive = true;
                                DefaultView.Authorized = true;
                                DefaultView.IsDefault = true;
                                DefaultView.SiteID = SiteID;
                                int ViewID = 0;
                                await Task.Run(() =>
                                {
                                    ViewID = BORepository.AddView(proxy, DefaultView, true).Result;
                                });

                                if (ViewID > 0)
                                {
                                    ContentStyleModel DefaultContent = new ContentStyleModel();
                                    Dictionary<string, object> ContentData = new Dictionary<string, object>();

                                    DefaultContent.Name = "Home";
                                    string welcomebody = @"<p><span style='font-size: medium;'><b><span style='text-decoration: underline;'>This is our default template.</span></b></span></p>
<p><strong><span style='text-decoration: underline;'>Welcome to our site.<img src='http://localhost:49791/Scripts/tinymce/plugins/emotions/img/smiley-smile.gif' alt='Smile' title='Smile' border='0' /></span></strong></p>";

                                    ContentData.Add("sd", welcomebody);
                                    ContentData.Add("st", -1);
                                    ContentData.Add("v", ViewID);

                                    DefaultContent.Type = 0;
                                    DefaultContent.Orientation = "0";
                                    DefaultContent.Data = JsonConvert.SerializeObject(ContentData);
                                    DefaultContent.Description = "Welcome";
                                    //DefaultContent.Order = 1;
                                    DefaultContent.IsActive = true;
                                    DefaultContent.SiteID = SiteID;
                                    int ContentID = 0;

                                    await Task.Run(() =>
                                    {
                                        ContentID = BORepository.AddContentStyle(proxy, DefaultContent).Result;
                                    });
                                }
                            }
                        }
                    }
                    if (SiteID != 0)
                        ViewBag.Message = "Site added successfully.";
                    else
                        ViewBag.Message = "Problem occured while creating site, kindly contact our support team.";

                    List<SiteModel> sites = new List<SiteModel>();
                    await Task.Run(() =>
                    {
                        sites.AddRange(BORepository.GetAllSites(proxy).Result);
                    });

                    CombineSiteModel siteObject = new CombineSiteModel();
                    siteObject.SiteList = sites;
                    siteObject.SiteView = new SiteModel();
                    return View("AddSite1", siteObject);
                }

                return View();
            }
            catch
            {
                return View();
            }
        }

        public ActionResult SiteDBDetail()
        {
            return View("SiteDBDetails");
        }
    }
}
