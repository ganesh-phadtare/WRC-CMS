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
using System.Linq;

namespace WRC_CMS.Controllers
{
    public class ContentStyleController : Controller
    {
        WebApiProxy proxy = new WebApiProxy();
        public ActionResult AddContentStyle()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddContentStyle(ContentStyleModel ContentStyleModelObject)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Dictionary<string, object> dicParams = new Dictionary<string, object>();
                    dicParams.Add("@Oid", "-1");
                    dicParams.Add("@Name", ContentStyleModelObject.Name);
                    dicParams.Add("@Descr", ContentStyleModelObject.Description);
                    if (ContentStyleModelObject.IsActive)
                        dicParams.Add("@IsActive", "1");
                    else
                        dicParams.Add("@IsActive", "0");
                    proxy.ExecuteNonQuery("SP_StaticContentsAddUp", dicParams);
                    ViewBag.Message = "Content Style added successfully";
                    BORepository SiteRepo = new BORepository();
                    if (SiteRepo.AddRecord(ContentStyleModelObject))
                    {
                        //ViewBag.Message = "Content Style added successfully";
                        ViewBag.Message = ContentStyleModelObject.Description;
                    }
                }

                return View();
            }
            catch
            {
                return View();
            }
        }

        public ActionResult ShowContent(ContentStyleModel ContentStyleModelObject)
        {
            ViewBag.Message = ContentStyleModelObject.Description;
            return View("ShowContent");
        }

        public async Task<ActionResult> EditContentDetails(int id)
        {
            List<ContentStyleModel> contents = new List<ContentStyleModel>();
            await Task.Run(() =>
            {
                contents.AddRange(GetAllContents().Result);
            });
            if (contents != null && contents.Count > 0)
            {
                ContentStyleModel objetc = contents.FirstOrDefault(item => item.Oid == id);
                return View(objetc);
            }
            return View();
        }

        [HttpPost]
        public ActionResult EditContentDetails(ContentStyleModel ContentStyleModelObject)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Dictionary<string, object> dicParams = new Dictionary<string, object>();
                    dicParams.Add("@Oid", ContentStyleModelObject.Oid);
                    dicParams.Add("@Name", ContentStyleModelObject.Name);
                    dicParams.Add("@Descr", ContentStyleModelObject.Description);
                    if (ContentStyleModelObject.IsActive)
                        dicParams.Add("@IsActive", "1");
                    else
                        dicParams.Add("@IsActive", "0");
                    proxy.ExecuteNonQuery("SP_StaticContentsAddUp", dicParams);
                    return RedirectToAction("GetAllContentsDetails");
                }

                return View();
            }
            catch
            {
                return View();
            }
        }

        public ActionResult DeleteView(int id)
        {
            try
            {
                Dictionary<string, object> dicParams = new Dictionary<string, object>();
                dicParams.Add("@Oid", id);
                proxy.ExecuteNonQuery("SP_StaticContentsDel", dicParams);
                return RedirectToAction("GetAllContentsDetails");
            }
            catch
            {
                return View();
            }
        }


        public async Task<ActionResult> GetAllContentsDetails()
        {
            ModelState.Clear();
            List<ContentStyleModel> views = new List<ContentStyleModel>();
            await Task.Run(() =>
            {
                views.AddRange(GetAllContents().Result);
            });
            return View("GetContentsDetails", views);
        }

        public async Task<List<ContentStyleModel>> GetAllContents()
        {
            List<ContentStyleModel> ContentList = new List<ContentStyleModel>();

            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("@Oid", -1);
            dict.Add("@LoadOnlyActive", 0);

            var dataSet = await proxy.ExecuteDataset("SP_StaticContentsSelect", dict);

            return (from DataRow row in dataSet.Tables[0].Rows
                    select new ContentStyleModel
                    {
                        Oid = Convert.ToInt32(row["Oid"].ToString()),
                        Name = row["Name"].ToString(),
                        Description = row["Descr"].ToString(),
                        IsActive = bool.Parse(row["IsActive"].ToString()),
                    }).ToList();
        }
    }
}
