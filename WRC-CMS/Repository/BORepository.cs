using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using WRC_CMS.Communication;
using WRC_CMS.Models;

namespace WRC_CMS.Repository
{
    public class BORepository
    {
        public bool AddRecord(object obj)
        {
            return true;
        }

        public static async Task<List<SiteModel>> GetAllSites(WebApiProxy proxy)
        {
            List<SiteModel> SiteList = new List<SiteModel>();
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("@Oid", -1);
            dict.Add("@LoadOnlyActive", 0);

            var dataSet = await proxy.ExecuteDataset("SP_SiteSelect", dict);
            return (from DataRow row in dataSet.Tables[0].Rows
                    select new SiteModel
                    {
                        Oid = Convert.ToInt32(row["Oid"].ToString()),
                        Name = row["Name"].ToString(),
                        Title = row["Title"].ToString(),
                        URL = row["url"].ToString(),
                        IsActive = bool.Parse(row["IsActive"].ToString())
                    }).ToList();
        }

        public static async Task<List<ViewModel>> GetAllViews(WebApiProxy proxy)
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

        public static async Task<List<MenuModel>> GetAllMenu(WebApiProxy proxy)
        {
            List<ViewModel> Views = GetAllViews(proxy).Result;
            List<SiteModel> Sites = GetAllSites(proxy).Result;

            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("@Oid", -1);

            var dataSet = await proxy.ExecuteDataset("SP_MenuSelect", dict);
            return (from DataRow row in dataSet.Tables[0].Rows
                    select new MenuModel
                    {
                        Oid = Convert.ToInt32(row["Oid"].ToString()),
                        SelectSite = Sites.FirstOrDefault(it => it.Oid == Convert.ToInt32(row["SiteId"].ToString())).Name,
                        SelectView = Views.FirstOrDefault(it => it.Oid == Convert.ToInt32(row["ViewId"].ToString())).Name,
                    }).ToList();
        }
    }
}