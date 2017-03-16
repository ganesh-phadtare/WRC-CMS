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

namespace WRC_CMS.Controllers
{
    public class AddViewController : Controller
    {
        WebApiProxy proxy = new WebApiProxy();
        public async Task<ActionResult> AddView()
        {
            ViewModel ViewObject = new ViewModel();
            if (ViewObject != null)
            {
                await Task.Run(() =>
                {
                    ViewObject.Site = BORepository.GetAllSites(proxy).Result;
                });
            }
            return View(ViewObject);
        }

        public async Task<ActionResult> Test(string Name, string Title, string IsActive, string IsDefault, string Authorized, string CreateMenu, object Logo, int Oid, string Orientation, int SiteID)
        {
            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Title))
                return RedirectToAction("GetAllViewDetails");

            ViewModel ViewObject = new ViewModel();
            if (Oid > 0)
                ViewObject.Oid = Oid;
            ViewObject.Name = Name.ToString();
            ViewObject.Title = Title.ToString();
            ViewObject.IsActive = Convert.ToBoolean(IsActive);
            ViewObject.IsDefault = Convert.ToBoolean(IsDefault);
            ViewObject.Authorized = Convert.ToBoolean(Authorized);
            ViewObject.CreateMenu = Convert.ToBoolean(CreateMenu);
            ViewObject.SiteID = SiteID;
            ViewObject.Orientation = Orientation;
            try
            {
                //if (file != null && file.ContentLength > 0)
                //{
                //    ViewObject.Logo = new byte[file.ContentLength];
                //    file.InputStream.Read(ViewObject.Logo, 0, file.ContentLength);
                //}

                if (ModelState.IsValid)
                {
                    int ViewID = 0;

                    await Task.Run(() =>
                    {
                        if (Oid == 0)
                            ViewID = BORepository.AddView(proxy, ViewObject, true).Result;
                        else
                            ViewID = BORepository.AddView(proxy, ViewObject, false).Result;
                    });
                    if (ViewID > 0)
                        ViewBag.Message = "View added successfully.";
                    else
                        ViewBag.Message = "Problem occured while adding view, kindly contact our support team.";

                    List<ViewModel> views = new List<ViewModel>();
                    await Task.Run(() =>
                    {
                        views.AddRange(BORepository.GetAllViews(proxy).Result.Where(item => item.SiteID == SiteID));
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

        public async Task<ActionResult> EditViewDetails(int ViewID = 0, int SiteID = 0)
        {
            if (ViewID != 0)
            {
                List<ViewModel> views = new List<ViewModel>();
                await Task.Run(() =>
                {
                    views.AddRange(BORepository.GetAllViews(proxy).Result.Where(item => item.SiteID == SiteID));
                });
                CombineModel com = new CombineModel();
                com.NewView = views.FirstOrDefault(view => view.Oid == ViewID);
                await Task.Run(() =>
               {
                   var IsMenuExist = BORepository.GetAllMenu(proxy).Result.FirstOrDefault(item => item.SiteId == SiteID && item.ViewId == ViewID);
                   if (!ReferenceEquals(IsMenuExist, null))
                       com.NewView.CreateMenu = true;
               });
                com.views = views;
                if (views.Count > 0)
                {
                    com.SiteName = views[0].SelectSite;
                    com.SiteID = views[0].SiteID;
                }
                ViewBag.CurrSiteID = SiteID;
                return View("ViewsLV", com);
            }
            else
            {
                return RedirectToAction("GetAllViewDetails", new { id = SiteID });
            }
        }

        public async Task<ActionResult> DeleteView(int id, int SiteID)
        {
            try
            {
                List<ViewModel> views = new List<ViewModel>();
                await Task.Run(() =>
                {
                    views.AddRange(BORepository.GetAllViews(proxy).Result.Where(item => item.SiteID == SiteID));
                });
                if (views != null && views.Count > 0)
                {
                    if (!ReferenceEquals(views.FirstOrDefault(it => it.Oid == id), null))
                    {
                        ViewModel viewtodelete = views.FirstOrDefault(it => it.Oid == id);
                        if (viewtodelete != null && !viewtodelete.IsDefault)
                        {
                            Dictionary<string, object> dicParams = new Dictionary<string, object>();
                            dicParams.Add("@Id", id);
                            proxy.ExecuteNonQuery("SP_ViewDel", dicParams);
                        }
                        else
                        {
                            ViewData["DeletionError"] = "Problem occured while deleting view.Cannot delete Default View.";
                        }
                    }
                }

                ModelState.Clear();
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

        public async Task<ActionResult> ReturnToMainView(int id)
        {
            ModelState.Clear();
            List<ViewModel> views = new List<ViewModel>();
            await Task.Run(() =>
            {
                views.AddRange(BORepository.GetAllViews(proxy).Result.Where(item => item.SiteID == id));
            });
            CombineModel com = new CombineModel();
            com.NewView = new ViewModel();
            com.views = views;
            if (views.Count > 0)
            {
                com.SiteName = views[0].SelectSite;
                com.SiteID = views[0].SiteID;
            }
            ViewBag.CurrSiteID = id;
            return View("ViewsLV", com);
        }

        public async Task<ActionResult> GetAllViewDetails(int id)
        {
            ActionResult View = null;
            await Task.Run(() =>
             {
                 View = ReturnToMainView(id).Result;
             });
            return View;
        }
    }
}
