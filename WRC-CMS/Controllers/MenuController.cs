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
    public class MenuController : BaseController
    {
        WebApiProxy proxy = new WebApiProxy();
        ////
        //// GET: /Menu/
        //public async Task<ActionResult> AddMenu()
        //{
        //    MenuModel MenuObject = new MenuModel();
        //    if (MenuObject != null)
        //    {
        //        await Task.Run(() =>
        //        {                    
        //            MenuObject.Site = BORepository.GetAllSites(proxy).Result;
        //            MenuObject.View = BORepository.GetAllViews(proxy).Result;
        //        });
        //    }
        //    return View(MenuObject);
        //}

        //[HttpPost]
        //public ActionResult AddMenu(MenuModel MenuObject)
        //{
        //    if (ModelState.IsValid && MenuObject != null)
        //    {
        //        Dictionary<string, object> dicParams = new Dictionary<string, object>();
        //        dicParams.Add("@Oid", -1);
        //        dicParams.Add("@SiteId", MenuObject.SelectSite);
        //        dicParams.Add("@ViewId", MenuObject.SelectView);
        //        proxy.ExecuteNonQuery("SP_MenuAddUp", dicParams);
        //        ViewBag.Message = "Menu added successfully";
        //    }
        //    return RedirectToAction("AddMenu");
        //}

        //public async Task<ActionResult> GetAllMenuDetails()
        //{
        //    ModelState.Clear();
        //    List<MenuModel> menus = new List<MenuModel>();
        //    await Task.Run(() =>
        //    {
        //        menus.AddRange(BORepository.GetAllMenu(proxy).Result);
        //    });
        //    return View("GetMenuDetails", menus);
        //}

        //[HttpPost]
        //public ActionResult EditMenuDetails(MenuModel MenuObject)
        //{
        //    if (ModelState.IsValid && MenuObject != null)
        //    {
        //        Dictionary<string, object> dicParams = new Dictionary<string, object>();
        //        dicParams.Add("@Oid", MenuObject.Oid);
        //        dicParams.Add("@SiteId", MenuObject.SelectSite);
        //        dicParams.Add("@ViewId", MenuObject.SelectView);
        //        proxy.ExecuteNonQuery("SP_MenuAddUp", dicParams);
        //    }
        //    return RedirectToAction("GetAllMenuDetails");
        //}

        //public async Task<ActionResult> EditMenuDetails(int id)
        //{
        //    List<MenuModel> menus = new List<MenuModel>();
        //    await Task.Run(() =>
        //    {
        //        menus.AddRange(BORepository.GetAllMenu(proxy).Result);
        //    });
        //    if (menus != null && menus.Count > 0)
        //    {
        //        MenuModel objetc = menus.FirstOrDefault(item => item.Oid == id);
        //        if (objetc != null)
        //        {
        //            await Task.Run(() =>
        //            {
        //                objetc.Site = BORepository.GetAllSites(proxy).Result;
        //                objetc.View = BORepository.GetAllViews(proxy).Result;
        //            });
        //        }
        //        return View(objetc);
        //    }
        //    return View();
        //}

        //public ActionResult DeleteMenu(int id)
        //{
        //    try
        //    {
        //        Dictionary<string, object> dicParams = new Dictionary<string, object>();
        //        dicParams.Add("@Oid", id);
        //        proxy.ExecuteNonQuery("SP_MenuDel", dicParams);
        //        return RedirectToAction("GetAllMenuDetails");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        public async Task<ActionResult> AddMenu(string Name, string URL, bool IsExternal, int Order, int Id, int ViewId, int SiteID)
        {
            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(URL))
                return RedirectToAction("GetAllMenu ");

            MenuModel menuobj = new MenuModel();
            if (Id > 0)
                menuobj.Id = Id;
            menuobj.Name = Name;
            menuobj.URL = URL;
            menuobj.IsExternal = IsExternal;
            menuobj.Order = Order;
            menuobj.ViewId = ViewId;
            menuobj.SiteId = SiteID;
            try
            {
                if (ModelState.IsValid)
                {
                    int MenuId = 0;

                    await Task.Run(() =>
                    {
                        if (Id == 0)
                            MenuId = BORepository.AddMenu(proxy, menuobj, true).Result;
                        else
                            MenuId = BORepository.AddMenu(proxy, menuobj, false).Result;
                    });
                    if (MenuId > 0)
                        ViewBag.Message = "Menu added successfully.";
                    else
                        ViewBag.Message = "Problem occured while adding Menu, kindly contact our support team.";

                    List<MenuModel> Menus = new List<MenuModel>();
                    await Task.Run(() =>
                    {
                        Menus.AddRange(BORepository.GetAllMenu(proxy).Result.Where(item => item.SiteId == SiteID));
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
        public async Task<ActionResult> EditMenu(int MenuID = 0, int SiteID = 0)
        {
            if (MenuID != 0)
            {
                List<MenuModel> Menus = new List<MenuModel>();
                await Task.Run(() =>
                {
                    Menus.AddRange(BORepository.GetAllMenu(proxy).Result.Where(item => item.SiteId == SiteID));
                });
                MenuModelLD com = new MenuModelLD();
                com.DetailView = Menus.FirstOrDefault(menu => menu.Id == MenuID);
                await Task.Run(() =>
                {
                    com.DetailView.View = BORepository.GetAllViews(proxy).Result.Where(item => item.SiteID == SiteID).ToList();
                });
                com.ListView = Menus;
                if (Menus.Count > 0)
                {
                    com.SiteName = Menus[0].SiteName;
                    com.SiteID = SiteID;
                }
                ViewBag.CurrSiteID = SiteID;
                return View("GetAllMenu", com);
            }
            else
            {
                return RedirectToAction("GetAllMenu", new { SiteId = SiteID });
            }
        }


        public async Task<ActionResult> DeleteMenu(int id, int SiteID)
        {
            try
            {
                Dictionary<string, object> dicParams = new Dictionary<string, object>();
                dicParams.Add("@Id", id);
                proxy.ExecuteNonQuery("SP_MenuDel", dicParams);

                ModelState.Clear();
                List<MenuModel> views = new List<MenuModel>();
                await Task.Run(() =>
                {
                    views.AddRange(BORepository.GetAllMenu(proxy).Result.Where(item => item.SiteId == SiteID));
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


        public async Task<ActionResult> GetAllMenu(int SiteId)
        {
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
            ViewBag.CurrSiteID = SiteId;
            List<MenuModel> Menu = new List<MenuModel>();
            await Task.Run(() =>
            {
                Menu.AddRange(BORepository.GetAllMenu(proxy).Result.Where(item => item.SiteId == SiteId));
            });
            MenuModelLD com = new MenuModelLD();
            com.DetailView = new MenuModel();
            await Task.Run(() =>
            {
                com.DetailView.View = BORepository.GetAllViews(proxy).Result.Where(item => item.SiteID == SiteId).ToList();
            });
            com.ListView = Menu;
            await Task.Run(() =>
            {
                List<SiteModel> Sites = BORepository.GetAllSites(proxy, SiteId).Result;
                if (Sites != null && Sites.Count > 0)
                {
                    com.SiteName = Sites.First().Title;
                    com.SiteID = Sites.First().Oid;
                }
            });
            return View("GetAllMenu", com);
        }

        public async Task<JsonResult> AddUpdateRecord(MenuModel modeldata)
        {
            Dictionary<string, object> ContentData = new Dictionary<string, object>();

            string Status = string.Empty;
            await Task.Run(() =>
            {
                Status = base.BaseAddUpdateRecord(modeldata, ModelState, proxy).Result;
            });
            return Json(new { status = Status });
        }

        public ActionResult DeleteRecord(int id, int SiteID)
        {
            string Status = string.Empty;
            MenuModel modeldata = new MenuModel();
            modeldata.Id = id;
           
            Status = base.BaseDeleteRecord(modeldata, ModelState, proxy);

            return RedirectToAction("GetAllMenu", new { SiteId = SiteID });
        }
    }
}