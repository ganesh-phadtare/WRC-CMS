using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                        Menus.AddRange(BORepository.GetAllMenu(proxy, SiteID).Result);
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
                    Menus.AddRange(BORepository.GetAllMenu(proxy, SiteID).Result);
                });
                MenuModelLD com = new MenuModelLD();
                com.DetailView = Menus.FirstOrDefault(menu => menu.Id == MenuID);
                await Task.Run(() =>
                {
                    com.DetailView.View = BORepository.GetAllViews(proxy, SiteID).Result.ToList();
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
                    views.AddRange(BORepository.GetAllMenu(proxy, SiteID).Result);
                });
                ActionResult View = null;
                View = await ReturnToMainView(SiteID);
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
            MenuModelLD com;
            ViewBag.CurrSiteID = SiteId;
            com = await GetCommonModel(SiteId);
            return View("GetAllMenu", com);
        }

        async Task<MenuModelLD> GetCommonModel(int SiteId)
        {
            ModelState.Clear();
            List<MenuModel> Menu = new List<MenuModel>();
            await Task.Run(() =>
            {
                Menu.AddRange(BORepository.GetAllMenu(proxy, SiteId).Result);
            });
            MenuModelLD com = new MenuModelLD();
            com.DetailView = new MenuModel();
            await Task.Run(() =>
            {
                com.DetailView.View = BORepository.GetAllViews(proxy, SiteId).Result.ToList();
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
            return com;
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

        public async Task<JsonResult> RecordMoveUpAndDown(int id, int siteId, bool isUpOrDown)
        {
            Dictionary<string, object> dicParams = new Dictionary<string, object>();
            dicParams.Add("@Id", id);
            dicParams.Add("@SiteId", siteId);
            if (isUpOrDown)
                dicParams.Add("@MoveUp", -1);
            else
                dicParams.Add("@MoveUp", 1);

            proxy.ExecuteNonQuery("SP_MenuUpDown", dicParams);

            string tabelBody = await DrawTableBody(siteId);
            return Json(new { Result = tabelBody });
        }

        private async Task<string> DrawTableBody(int siteId)
        {
            StringBuilder tableBody = new StringBuilder();

            var combineModel = await GetCommonModel(siteId);
            if (combineModel != null)
            {
                tableBody.AppendLine("<thead _ngcontent-xlf-80='' class='thead-custom-style approvals-card'>");
                tableBody.AppendLine("    <tr _ngcontent-xlf-80=''>");
                tableBody.AppendLine("       <th _ngcontent-xlf-80='' class='col-width-20' data-field='comments'>ViewName</th>");
                tableBody.AppendLine("        <th _ngcontent-xlf-80='' class='col-width-20' data-field='comments'>Name</th>");
                tableBody.AppendLine("        <th _ngcontent-xlf-80='' class='col-width-20' data-field='comments'>URL</th>");
                tableBody.AppendLine("        <th _ngcontent-xlf-80='' class='col-width-20' data-field='comments'>Action</th>");
                tableBody.AppendLine("       <th _ngcontent-xlf-80='' class='col-width-20' data-field='comments'>Move Up/Down</th>");
                tableBody.AppendLine("    </tr>");
                tableBody.AppendLine("</thead>");
                tableBody.AppendLine("<tbody _ngcontent-xlf-80='' class='leave-tbody'>");
                foreach (var item in combineModel.ListView)
                {
                    tableBody.AppendLine("<tr _ngcontent-xlf-80=''>");
                    tableBody.AppendLine("  <td _ngcontent-xlf-80='' class='col-width-20 orderNo'>");
                    tableBody.AppendLine(item.ViewName.ToString());
                    tableBody.AppendLine("</td>");
                    tableBody.AppendLine("<td _ngcontent-xlf-80='' class='col-width-20'>");
                    tableBody.AppendLine(item.Name);
                    tableBody.AppendLine("</td>");
                    tableBody.AppendLine("<td _ngcontent-xlf-80='' class='col-width-20'>");
                    tableBody.AppendLine(item.URL);
                    tableBody.AppendLine("</td>");
                    tableBody.AppendLine("<td _ngcontent-xlf-80='' class='col-width-20'>");
                    tableBody.AppendLine(string.Format("<a href='/Menu/EditMenu?MenuID={0}&SiteID={1}'>Edit</a> | ", item.Id, item.SiteId));
                    tableBody.AppendLine(string.Format("<a href='/Menu/DeleteRecord/{0}?siteId={1}' onclick='return confirm('Are sure wants to delete?');'>Delete</a>", item.Id, item.SiteId));
                    tableBody.AppendLine("</td>");
                    tableBody.AppendLine("<td _ngcontent-xlf-80='' class='col-width-20'>");
                    tableBody.AppendLine(string.Format("        <input type='submit' value='Λ' class='' id='btUp' onclick='OnBtnUpClick({0}, true);' {1}/> | ", item.Id, DisabledAction(item.IsUp)));
                    tableBody.AppendLine(string.Format("        <input type='submit' value='V' class='' id='btDown' onclick='OnBtnUpClick({0}, false);' {1}/>", item.Id, DisabledAction(item.IsDown)));
                    tableBody.AppendLine("  </td>");
                    tableBody.AppendLine("</tr>");
                }
                tableBody.AppendLine("</tbody>");
            }
            return tableBody.ToString();
        }

        string DisabledAction(bool isDisabled)
        {
            if (isDisabled)
                return " disabled='disabled'";
            return string.Empty;
        }
    }
}