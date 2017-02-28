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
            if (!ReferenceEquals(dataSet, null) && dataSet.Tables.Count > 0)
            {
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
            return SiteList;
        }

        public static async Task<int> AddView(WebApiProxy proxy, ViewModel ViewObject, bool IsNewObject = false)
        {
            Dictionary<string, object> dicParams = new Dictionary<string, object>();
            if (IsNewObject)
                dicParams.Add("@Oid", -1);
            else
                dicParams.Add("@Oid", ViewObject.Oid);
            dicParams.Add("@SiteId", ViewObject.SiteID);
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
                dicParams.Add("@IsDefault", "1");
            else
                dicParams.Add("@IsDefault", "0");

            if (ViewObject.CreateMenu)
                dicParams.Add("@IsMenu", "1");
            else
                dicParams.Add("@IsMenu", "0");

            //DataSet dataSet = null;
            //await Task.Run(() =>
            //     {
            DataSet dataSet = await proxy.ExecuteDataset("SP_ViewAddUp", dicParams);
            //});
            if (dataSet != null && dataSet.Tables != null && dataSet.Tables.Count > 0)
            {
                if (dataSet.Tables[0].Rows != null && dataSet.Tables[0].Rows.Count > 0)
                {
                    int ViewID = Convert.ToInt32(dataSet.Tables[0].Rows[0][0].ToString());
                    return ViewID;
                }
            }
            return -1;
        }

        public static async Task<int> AddContentStyle(WebApiProxy proxy, ContentStyleModel ContentStyleModelObject)
        {
            Dictionary<string, object> dicParams = new Dictionary<string, object>();
            dicParams.Add("@Oid", "-1");
            dicParams.Add("@Name", ContentStyleModelObject.Name);
            dicParams.Add("@Descr", ContentStyleModelObject.Description);
            dicParams.Add("@View", ContentStyleModelObject.ViewID);

            if (ContentStyleModelObject.IsActive)
                dicParams.Add("@IsActive", "1");
            else
                dicParams.Add("@IsActive", "0");
            //DataSet dataSet = null;
            //await Task.Run(() =>
            //{
            DataSet dataSet = await proxy.ExecuteDataset("SP_StaticContentsAddUp", dicParams);
            //});

            if (dataSet != null && dataSet.Tables != null && dataSet.Tables.Count > 0)
            {
                if (dataSet.Tables[0].Rows != null && dataSet.Tables[0].Rows.Count > 0)
                {
                    int ContentID = Convert.ToInt32(dataSet.Tables[0].Rows[0][0].ToString());
                    return ContentID;
                }
            }
            return -1;
        }

        public static async Task<List<ViewModel>> GetAllViews(WebApiProxy proxy)
        {
            List<SiteModel> Sites = GetAllSites(proxy).Result;
            List<ViewModel> ViewList = new List<ViewModel>();
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("@Oid", -1);
            dict.Add("@LoadOnlyActive", 0);

            var dataSet = await proxy.ExecuteDataset("SP_ViewSelect", dict);
            if (!ReferenceEquals(dataSet, null) && dataSet.Tables.Count > 0)
            {
                return (from DataRow row in dataSet.Tables[0].Rows
                        select new ViewModel
                        {
                            Oid = Convert.ToInt32(row["Oid"].ToString()),
                            Name = row["Name"].ToString(),
                            Title = row["Title"].ToString(),
                            URL = row["url"].ToString(),
                            IsActive = bool.Parse(row["IsActive"].ToString()),
                            IsDem = bool.Parse(row["IsDefault"].ToString()),
                            IsAuth = bool.Parse(row["IsAuth"].ToString()),
                            CreateMenu = bool.Parse(row["IsMenu"].ToString()),
                            SiteID = row["Site"].ToString() == string.Empty ? 0 : Convert.ToInt32(row["Site"].ToString()),
                            SelectSite = Sites.FirstOrDefault(it => it.Oid == Convert.ToInt32(row["Site"].ToString())).Name
                        }).ToList();
            }
            return ViewList;
        }

        public static async Task<List<ContentStyleModel>> GetAllContents(WebApiProxy proxy)
        {
            List<ContentStyleModel> ContentList = new List<ContentStyleModel>();

            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("@Oid", -1);
            dict.Add("@LoadOnlyActive", 0);

            var dataSet = await proxy.ExecuteDataset("SP_StaticContentsSelect", dict);
            if (!ReferenceEquals(dataSet, null) && dataSet.Tables.Count > 0)
            {
                return (from DataRow row in dataSet.Tables[0].Rows
                        select new ContentStyleModel
                        {
                            Oid = Convert.ToInt32(row["Oid"].ToString()),
                            Name = row["Name"].ToString(),
                            Description = row["Descr"].ToString(),
                            IsActive = bool.Parse(row["IsActive"].ToString()),
                            ViewID = row["Views"].ToString() == string.Empty ? 0 : Convert.ToInt32(row["Views"].ToString())
                        }).ToList();
            }
            return ContentList;
        }

        public static async Task<List<MenuModel>> GetAllMenu(WebApiProxy proxy)
        {
            List<ViewModel> Views = GetAllViews(proxy).Result;
            List<SiteModel> Sites = GetAllSites(proxy).Result;
            List<MenuModel> Menus = new List<MenuModel>();

            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("@Oid", -1);

            var dataSet = await proxy.ExecuteDataset("SP_MenuSelect", dict);
            if (!ReferenceEquals(dataSet, null) && dataSet.Tables.Count > 0)
            {
                return (from DataRow row in dataSet.Tables[0].Rows
                        select new MenuModel
                        {
                            Oid = Convert.ToInt32(row["Oid"].ToString()),
                            SelectSite = Sites.FirstOrDefault(it => it.Oid == Convert.ToInt32(row["SiteId"].ToString())).Name,
                            SelectView = Views.FirstOrDefault(it => it.Oid == Convert.ToInt32(row["ViewId"].ToString())).Name,
                        }).ToList();
            }
            return Menus;
        }

        public static async Task<List<SiteModel>> GetSearchSite(WebApiProxy proxy,SiteModel viewObject)
        {
            List<SiteModel> SearchList = new List<SiteModel>();
            Dictionary<string, object> dicParams = new Dictionary<string, object>();
            dicParams.Add("@Name", viewObject.Name );

            var dataSet = await proxy.ExecuteDataset("SP_GetSearchList", dicParams);
            if (!ReferenceEquals(dataSet, null) && dataSet.Tables.Count > 0)
            {
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
            return SearchList;
        }
    }
}