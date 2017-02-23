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
    public class AddViewController : Controller
    {
        WebApiProxy proxy = new WebApiProxy();
        public ActionResult AddView()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddView(ViewModel ViewObject, HttpPostedFileBase file)
        {
            try
            {
                if (file != null && file.ContentLength > 0)
                {
                    ViewObject.Logo = new byte[file.ContentLength];
                    file.InputStream.Read(ViewObject.Logo, 0, file.ContentLength);
                }

                if (ModelState.IsValid)
                {
                    Dictionary<string, object> dicParams = new Dictionary<string, object>();
                    dicParams.Add("@Oid", -1);
                    dicParams.Add("@Name", ViewObject.Name);
                    dicParams.Add("@url", ViewObject.URL);
                    dicParams.Add("@Logo", 1014);
                    dicParams.Add("@Title", ViewObject.Title);
                    if (ViewObject.IsActive)
                        dicParams.Add("@IsActive", "1");
                    else
                        dicParams.Add("@IsActive", "0");

                    if (ViewObject.IsAuth)
                        dicParams.Add("@IsAuth", "1");
                    else
                        dicParams.Add("@IsAuth", "0");

                    if (ViewObject.IsDem)
                        dicParams.Add("@IsDem", "1");
                    else
                        dicParams.Add("@IsDem", "0");
                    proxy.ExecuteNonQuery("SP_ViewAddUp", dicParams);
                    ViewBag.Message = "View added successfully";
                }

                return View();
            }
            catch
            {
                return View();
            }
        }


        [HttpPost]
        public ActionResult EditViewDetails(ViewModel ViewObject, HttpPostedFileBase file)
        {
            try
            {
                if (file != null && file.ContentLength > 0)
                {
                    ViewObject.Logo = new byte[file.ContentLength];
                    file.InputStream.Read(ViewObject.Logo, 0, file.ContentLength);
                }
                if (ModelState.IsValid)
                {
                    Dictionary<string, object> dicParams = new Dictionary<string, object>();
                    dicParams.Add("@Oid", ViewObject.Oid);
                    dicParams.Add("@Name", ViewObject.Name);
                    dicParams.Add("@url", ViewObject.URL);
                    dicParams.Add("@Logo", 0101);
                    dicParams.Add("@Title", ViewObject.Title);
                    if (ViewObject.IsActive)
                        dicParams.Add("@IsActive", "1");
                    else
                        dicParams.Add("@IsActive", "0");

                    if (ViewObject.IsAuth)
                        dicParams.Add("@IsAuth", "1");
                    else
                        dicParams.Add("@IsAuth", "0");

                    if (ViewObject.IsDem)
                        dicParams.Add("@IsDem", "1");
                    else
                        dicParams.Add("@IsDem", "0");

                    proxy.ExecuteNonQuery("SP_ViewAddUp", dicParams);
                    return RedirectToAction("GetAllViewDetails");
                }
                return View();
            }
            catch
            {
                return View();
            }
        }

        public async Task<ActionResult> EditViewDetails(int id)
        {
            List<ViewModel> views = new List<ViewModel>();
            await Task.Run(() =>
            {
                views.AddRange(GetAllViews().Result);
            });
            if (views != null && views.Count > 0)
            {
                ViewModel objetc = views.FirstOrDefault(item => item.Oid == id);
                return View(objetc);
            }
            return View();
        }

        public ActionResult DeleteView(int id)
        {
            try
            {
                Dictionary<string, object> dicParams = new Dictionary<string, object>();
                dicParams.Add("@Oid", id);
                proxy.ExecuteNonQuery("SP_ViewDel", dicParams);
                return RedirectToAction("GetAllViewDetails");
            }
            catch
            {
                return View();
            }
        }

        public async Task<ActionResult> GetAllViewDetails()
        {
            ModelState.Clear();
            List<ViewModel> views = new List<ViewModel>();
            await Task.Run(() =>
            {
                views.AddRange(GetAllViews().Result);
            });
            return View("GetViewDetails", views);
        }

        public async Task<List<ViewModel>> GetAllViews()
        {
            List<ViewModel> SiteList = new List<ViewModel>();
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("@Oid", -1);
            dict.Add("@LoadOnlyActive", 0);

            var dataSet = await proxy.ExecuteDataset("SP_ViewSelect", dict);
            return (from DataRow row in dataSet.Tables[0].Rows
                    select new ViewModel
                    {
                        Oid = Convert.ToInt32(row["Oid"].ToString()),
                        Name = row["Name"].ToString(),
                        Title = row["Title"].ToString(),
                        URL = row["url"].ToString(),
                        IsActive = bool.Parse(row["IsActive"].ToString()),
                        IsDem = bool.Parse(row["IsDem"].ToString()),
                        IsAuth = bool.Parse(row["IsAuth"].ToString())
                    }).ToList();
        }
    }
}
