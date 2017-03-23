using System;
using System.Collections.Generic;
using System.Data;
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
    public class AddViewController : BaseController
    {
        public WebApiProxy proxy = new WebApiProxy();

        public async Task<JsonResult> AddUpdateRecord(ViewModel ModelObject)
        {
            string Status = string.Empty;
            await Task.Run(() =>
            {
                Status = base.BaseAddUpdateRecord(ModelObject, ModelState, proxy).Result;
            }
            );
            return Json(new { status = Status });
        }

        public async Task<ActionResult> EditViewDetails(int ViewID = 0, int SiteID = 0)
        {
            if (ViewID != 0)
            {
                CombineModel com;
                com = await GetCombineModel(ViewID, SiteID);

                return View("ViewsLV", com);
            }
            else
                return RedirectToAction("GetAllViewDetails", new { id = SiteID });
        }

        private async Task<CombineModel> GetCombineModel(int ViewID = 0, int SiteID = 0)
        {
            CombineModel com = new CombineModel();
            List<ViewModel> views = new List<ViewModel>();
            await Task.Run(() =>
            {
                views.AddRange(BORepository.GetAllViews(proxy, SiteID).Result);
            });
            com.NewView = views.FirstOrDefault(view => view.Oid == ViewID);
            await Task.Run(() =>
           {
               var IsMenuExist = BORepository.GetAllMenu(proxy, SiteID).Result.FirstOrDefault(item => item.ViewId == ViewID);
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

            await Task.Run(() =>
            {
                com.ViewAllContents.AddRange(BORepository.GetAllContents(proxy, com.SiteID).Result.ToList());
            });

            await Task.Run(() =>
            {
                com.ViewContents.AddRange(BORepository.GetAllContents(proxy, com.SiteID, com.NewView.Oid).Result.ToList());
            });

            return com;
        }

        public async Task<ActionResult> DeleteView(int ViewID, bool IsDefault, int SiteID)
        {
            if (!IsDefault)
            {
                Dictionary<string, object> dicParams = new Dictionary<string, object>();
                dicParams.Add("@Id", ViewID);
                proxy.ExecuteNonQuery("SP_ViewDel", dicParams);
            }
            else
                ViewData["DeletionError"] = "Problem occured while deleting view.Cannot delete Default View.";

            ActionResult View = null;
            await Task.Run(() =>
            {
                View = ReturnToMainView(SiteID).Result;
            });
            return View;
        }

        public async Task<JsonResult> AddViewContent(int contentId, int viewId, int siteId)
        {
            Dictionary<string, object> dicParams = new Dictionary<string, object>();
            dicParams.Add("@Id", -1);
            dicParams.Add("@Order", 0);
            dicParams.Add("@ContentId", contentId);
            dicParams.Add("@ViewId", viewId);
            dicParams.Add("@SiteId", siteId);

            proxy.ExecuteNonQuery("SP_ContentOfViewAddUp", dicParams);

            string tabelBody = await DrawTableBody(viewId, siteId);
            return Json(new { Result = tabelBody });
        }

        private async Task<string> DrawTableBody(int viewId, int siteId)
        {
            StringBuilder tableBody = new StringBuilder();

            var combineModel = await GetCombineModel(viewId, siteId);
            if (combineModel != null)
            {
                var viewContents = combineModel.ViewContents;

                tableBody.AppendLine("<thead _ngcontent-xlf-80='' class='thead-custom-style approvals-card'>");
                tableBody.AppendLine("    <tr _ngcontent-xlf-80=''>");
                tableBody.AppendLine("       <th _ngcontent-xlf-80='' class='col-width-20' data-field='date'>Order</th>");
                tableBody.AppendLine("        <th _ngcontent-xlf-80='' class='col-width-20' data-field='comments'>Name</th>");
                tableBody.AppendLine("        <th _ngcontent-xlf-80='' class='col-width-20' data-field='comments'>Orientation</th>");
                tableBody.AppendLine("        <th _ngcontent-xlf-80='' class='col-width-20' data-field='comments'>Delete</th>");
                tableBody.AppendLine("       <th _ngcontent-xlf-80='' class='col-width-20' data-field='comments'>Move Up-Down</th>");
                tableBody.AppendLine("    </tr>");
                tableBody.AppendLine("</thead>");
                tableBody.AppendLine("<tbody _ngcontent-xlf-80='' class='leave-tbody'>");
                foreach (var item in viewContents)
                {
                    tableBody.AppendLine("<tr _ngcontent-xlf-80='' class='requiredRow'>");
                    tableBody.AppendLine("  <td _ngcontent-xlf-80='' class='col-width-20 orderNo'>");
                    tableBody.AppendLine(item.Order.ToString());
                    tableBody.AppendLine("</td>");
                    tableBody.AppendLine("<td _ngcontent-xlf-80='' class='col-width-20'>");
                    tableBody.AppendLine(item.Name);
                    tableBody.AppendLine("</td>");
                    tableBody.AppendLine("<td _ngcontent-xlf-80='' class='col-width-20'>");
                    tableBody.AppendLine(item.Orientation.ToString());
                    tableBody.AppendLine("</td>");
                    tableBody.AppendLine("<td _ngcontent-xlf-80='' class='col-width-20'>");
                    tableBody.AppendLine(string.Format("<a href='/AddView/DeleteViewContent/{0}?siteId={1}&viewId={2}' onclick='return confirm('Are sure wants to delete?');'>Delete</a>", item.Id, item.SiteID, item.ViewID));
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

        public ActionResult DeleteViewContent(int id, int siteId, int viewId)
        {
            try
            {
                Dictionary<string, object> dicParams = new Dictionary<string, object>();
                dicParams.Add("@Id", id);
                proxy.ExecuteNonQuery("SP_ViewContentsDel", dicParams);
                ModelState.Clear();
                return RedirectToAction("EditViewDetails", new { ViewID = viewId, SiteId = siteId });
            }
            catch
            {
                return View();
            }
        }

        public async Task<JsonResult> RecordMoveUpAndDown(int id, int viewId, int siteId, bool isUpOrDown)
        {
            Dictionary<string, object> dicParams = new Dictionary<string, object>();
            dicParams.Add("@Id", id);
            dicParams.Add("@ViewId", viewId);
            dicParams.Add("@SiteId", siteId);
            if (isUpOrDown)
                dicParams.Add("@MoveUp", -1);
            else
                dicParams.Add("@MoveUp", 1);

            proxy.ExecuteNonQuery("SP_ViewContentUpDown", dicParams);

            string tabelBody = await DrawTableBody(viewId, siteId);
            return Json(new { Result = tabelBody });
        }

        public async Task<ActionResult> ReturnToMainView(int id)
        {
            List<ViewModel> views = new List<ViewModel>();
            await Task.Run(() =>
            {
                views.AddRange(BORepository.GetAllViews(proxy, id).Result);
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

            await Task.Run(() =>
            {
                com.ViewAllContents.AddRange(BORepository.GetAllContents(proxy, com.SiteID, true).Result.ToList());
            });

            await Task.Run(() =>
                {
                    com.ViewContents.AddRange(BORepository.GetAllContents(proxy, com.SiteID, com.NewView.Oid).Result.ToList());
                });
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

        public ActionResult DeleteRecord(int ViewID, bool IsDefault, int SiteID)
        {
            if (!IsDefault)
            {
                string Status = string.Empty;
                ViewModel modeldata = new ViewModel();
                modeldata.Oid = ViewID;
                modeldata.SiteID = SiteID;

                Status = base.BaseDeleteRecord(modeldata, ModelState, proxy);
            }
            return RedirectToAction("GetAllViewDetails", new { id = SiteID });
        }
    }
}
