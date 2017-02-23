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
    public class MenuController : Controller
    {
        WebApiProxy proxy = new WebApiProxy();
        //
        // GET: /Menu/
        public async Task<ActionResult> AddMenu()
        {
            MenuModel MenuObject = new MenuModel();
            if (MenuObject != null)
            {
                await Task.Run(() =>
                {
                    MenuObject.Site = BORepository.GetAllSites(proxy).Result;
                    MenuObject.View = BORepository.GetAllViews(proxy).Result;
                });
            }
            return View(MenuObject);
        }

        [HttpPost]
        public ActionResult AddMenu(MenuModel MenuObject)
        {
            if (ModelState.IsValid && MenuObject != null)
            {
                Dictionary<string, object> dicParams = new Dictionary<string, object>();
                dicParams.Add("@Oid", -1);
                dicParams.Add("@SiteId", MenuObject.SelectSite);
                dicParams.Add("@ViewId", MenuObject.SelectView);
                proxy.ExecuteNonQuery("SP_MenuAddUp", dicParams);
                ViewBag.Message = "Menu added successfully";
            }
            return RedirectToAction("AddMenu");
        }

        public async Task<ActionResult> GetAllMenuDetails()
        {
            ModelState.Clear();
            List<MenuModel> menus = new List<MenuModel>();
            await Task.Run(() =>
            {
                menus.AddRange(BORepository.GetAllMenu(proxy).Result);
            });
            return View("GetMenuDetails", menus);
        }

        [HttpPost]
        public ActionResult EditMenuDetails(MenuModel MenuObject)
        {
            if (ModelState.IsValid && MenuObject != null)
            {
                Dictionary<string, object> dicParams = new Dictionary<string, object>();
                dicParams.Add("@Oid", MenuObject.Oid);
                dicParams.Add("@SiteId", MenuObject.SelectSite);
                dicParams.Add("@ViewId", MenuObject.SelectView);
                proxy.ExecuteNonQuery("SP_MenuAddUp", dicParams);
            }
            return RedirectToAction("GetAllMenuDetails");
        }

        public async Task<ActionResult> EditMenuDetails(int id)
        {
            List<MenuModel> menus = new List<MenuModel>();
            await Task.Run(() =>
            {
                menus.AddRange(BORepository.GetAllMenu(proxy).Result);
            });
            if (menus != null && menus.Count > 0)
            {
                MenuModel objetc = menus.FirstOrDefault(item => item.Oid == id);
                if (objetc != null)
                {
                    await Task.Run(() =>
                    {
                        objetc.Site = BORepository.GetAllSites(proxy).Result;
                        objetc.View = BORepository.GetAllViews(proxy).Result;
                    });
                }
                return View(objetc);
            }
            return View();
        }

        public ActionResult DeleteMenu(int id)
        {
            try
            {
                Dictionary<string, object> dicParams = new Dictionary<string, object>();
                dicParams.Add("@Oid", id);
                proxy.ExecuteNonQuery("SP_MenuDel", dicParams);
                return RedirectToAction("GetAllMenuDetails");
            }
            catch
            {
                return View();
            }
        }
    }
}