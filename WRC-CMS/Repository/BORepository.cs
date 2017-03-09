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

        public static async Task<List<SiteModel>> GetAllSites(WebApiProxy proxy, int Id = -1, string SiteName = "")
        {
            List<SiteModel> SiteList = new List<SiteModel>();
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("@Id", Id);
            dict.Add("@LoadOnlyActive", 0);
            dict.Add("@SiteName", SiteName);

            var dataSet = await proxy.ExecuteDataset("SP_SiteSelect", dict);
            if (!ReferenceEquals(dataSet, null) && dataSet.Tables.Count > 0)
            {
                return (from DataRow row in dataSet.Tables[0].Rows
                        select new SiteModel
                        {
                            Oid = Convert.ToInt32(row["Id"].ToString()),
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
                dicParams.Add("@Id", -1);
            else
                dicParams.Add("@Id", ViewObject.Oid);
            dicParams.Add("@Name", ViewObject.Name);
            dicParams.Add("@Title", ViewObject.Title);
            dicParams.Add("@Logo", 1014);
            dicParams.Add("@Orientation", ViewObject.Orientation);
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
            dicParams.Add("@SiteId", ViewObject.SiteID);

            DataSet dataSet = await proxy.ExecuteDataset("SP_ViewAddUp", dicParams);

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

        public static async Task<int> AddSiteDB(WebApiProxy proxy, SiteDbModel SiteDBObject, bool IsNewObject = false)
        {
            Dictionary<string, object> dicParams = new Dictionary<string, object>();
            if (IsNewObject)
                dicParams.Add("@Id", -1);
            else
                dicParams.Add("@Id", SiteDBObject.Id);
            dicParams.Add("@Name", SiteDBObject.Name);
            dicParams.Add("@Server", SiteDBObject.Server);
            dicParams.Add("@Database", SiteDBObject.Database);
            dicParams.Add("@UserID", SiteDBObject.UserID);
            dicParams.Add("@Password", SiteDBObject.Password);
            dicParams.Add("@Description", SiteDBObject.Description);
            dicParams.Add("@SiteId", SiteDBObject.SiteId);

            DataSet dataSet = await proxy.ExecuteDataset("SP_SiteDBAddUp", dicParams);

            if (dataSet != null && dataSet.Tables != null && dataSet.Tables.Count > 0)
            {
                if (dataSet.Tables[0].Rows != null && dataSet.Tables[0].Rows.Count > 0)
                {
                    int SiteDB = Convert.ToInt32(dataSet.Tables[0].Rows[0][0].ToString());
                    return SiteDB;
                }
            }
            return -1;
        }

        public static async Task<int> AddSiteMisc(WebApiProxy proxy, SiteMiscModel SiteMiscObject, bool IsNewObject = false)
        {
            Dictionary<string, object> dicParams = new Dictionary<string, object>();
            if (IsNewObject)
                dicParams.Add("@Id", -1);
            else
                dicParams.Add("@Id", SiteMiscObject.Id);
            dicParams.Add("@Key", SiteMiscObject.Key);
            dicParams.Add("@Value", SiteMiscObject.Value);
            dicParams.Add("@SiteId", SiteMiscObject.SiteId);

            DataSet dataSet = await proxy.ExecuteDataset("SP_SiteMiscAddUp", dicParams);

            if (dataSet != null && dataSet.Tables != null && dataSet.Tables.Count > 0)
            {
                if (dataSet.Tables[0].Rows != null && dataSet.Tables[0].Rows.Count > 0)
                {
                    int SiteMisc = Convert.ToInt32(dataSet.Tables[0].Rows[0][0].ToString());
                    return SiteMisc;
                }
            }
            return -1;
        }

        public static async Task<int> AddMenu(WebApiProxy proxy, MenuModel MenuObject, bool IsNewObject = false)
        {
            Dictionary<string, object> dicParams = new Dictionary<string, object>();
            if (IsNewObject)
                dicParams.Add("@Id", -1);
            else
                dicParams.Add("@Id", MenuObject.Oid);
            dicParams.Add("@Name", MenuObject.Name);
            dicParams.Add("@URL", MenuObject.URL);
            dicParams.Add("@IsExternal", MenuObject.IsExternal);
            dicParams.Add("@Order", MenuObject.Order);
            dicParams.Add("@ViewId", MenuObject.ViewId);
            dicParams.Add("@SiteId", MenuObject.SiteId);

            DataSet dataSet = await proxy.ExecuteDataset("SP_MenuAddUp", dicParams);

            if (dataSet != null && dataSet.Tables != null && dataSet.Tables.Count > 0)
            {
                if (dataSet.Tables[0].Rows != null && dataSet.Tables[0].Rows.Count > 0)
                {
                    int SiteMisc = Convert.ToInt32(dataSet.Tables[0].Rows[0][0].ToString());
                    return SiteMisc;
                }
            }
            return -1;
        }

        public static async Task<int> AddContentStyle(WebApiProxy proxy, ContentStyleModel ContentStyleModelObject)
        {
            Dictionary<string, object> dicParams = new Dictionary<string, object>();
            dicParams.Add("@Id", "-1");
            dicParams.Add("@Name", ContentStyleModelObject.Name);
            dicParams.Add("@Type", ContentStyleModelObject.Type);
            dicParams.Add("@Orientation", ContentStyleModelObject.Orientation);
            dicParams.Add("@Data", ContentStyleModelObject.Data);
            dicParams.Add("@Description", ContentStyleModelObject.Description);
            dicParams.Add("@Sequence", ContentStyleModelObject.Sequence);
            if (ContentStyleModelObject.IsActive)
                dicParams.Add("@IsActive", "1");
            else
                dicParams.Add("@IsActive", "0");
            dicParams.Add("@SiteID", ContentStyleModelObject.SiteID);          

           
            //DataSet dataSet = null;
            //await Task.Run(() =>
            //{
            DataSet dataSet = await proxy.ExecuteDataset("SP_ContentsAddUp", dicParams);
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
            dict.Add("@Id", -1);
            dict.Add("@LoadOnlyActive", 0);
            dict.Add("@ViewName", "");
            var dataSet = await proxy.ExecuteDataset("SP_ViewSelect", dict);
            if (!ReferenceEquals(dataSet, null) && dataSet.Tables.Count > 0)
            {
                return (from DataRow row in dataSet.Tables[0].Rows
                        select new ViewModel
                        {
                            Oid = Convert.ToInt32(row["Id"].ToString()),
                            Name = row["Name"].ToString(),
                            Title = row["Title"].ToString(),
                            IsActive = bool.Parse(row["IsActive"].ToString()),
                            IsDem = bool.Parse(row["IsDefault"].ToString()),
                            IsAuth = bool.Parse(row["Authorized"].ToString()),
                            Orientation = row["Orientation"].ToString(),
                            //CreateMenu = bool.Parse(row["IsMenu"].ToString()),
                            SiteID = row["SiteId"].ToString() == string.Empty ? 0 : Convert.ToInt32(row["SiteId"].ToString()),
                            SelectSite = Sites.FirstOrDefault(it => it.Oid == Convert.ToInt32(row["SiteId"].ToString())).Title
                        }).ToList();
            }
            return ViewList;
        }

        public static async Task<List<SiteDbModel>> GetAllSiteDb(WebApiProxy proxy)
        {
            List<SiteModel> Sites = GetAllSites(proxy).Result;
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("@Id", -1);
            var dataSet = await proxy.ExecuteDataset("SP_SiteDBSelect", dict);
            if (!ReferenceEquals(dataSet, null) && dataSet.Tables.Count > 0)
            {
                return (from DataRow row in dataSet.Tables[0].Rows
                        select new SiteDbModel
                        {
                            Id = Convert.ToInt32(row["Id"].ToString()),
                            Name = row["Name"].ToString(),
                            Server = row["Server"].ToString(),
                            Database = row["Database"].ToString(),
                            UserID = row["UserID"].ToString(),
                            Password = row["Password"].ToString(),
                            Description = row["Description"].ToString(),
                            SiteId = row["SiteId"].ToString() == string.Empty ? 0 : Convert.ToInt32(row["SiteId"].ToString()),
                            SiteName = Sites.FirstOrDefault(it => it.Oid == Convert.ToInt32(row["SiteId"].ToString())).Title
                        }).ToList();
            }
            return new List<SiteDbModel>();
        }

        public static async Task<List<SiteMiscModel>> GetAllSiteMISC(WebApiProxy proxy)
        {
            List<SiteModel> Sites = GetAllSites(proxy).Result;
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("@Id", -1);
            var dataSet = await proxy.ExecuteDataset("SP_SiteMiscSelect", dict);
            if (!ReferenceEquals(dataSet, null) && dataSet.Tables.Count > 0)
            {
                return (from DataRow row in dataSet.Tables[0].Rows
                        select new SiteMiscModel
                        {
                            Id = Convert.ToInt32(row["Id"].ToString()),
                            Key = row["Key"].ToString(),
                            Value = row["Value"].ToString(),
                            SiteId = row["SiteId"].ToString() == string.Empty ? 0 : Convert.ToInt32(row["SiteId"].ToString()),
                            SiteName = Sites.FirstOrDefault(it => it.Oid == Convert.ToInt32(row["SiteId"].ToString())).Title
                        }).ToList();
            }
            return new List<SiteMiscModel>();
        }

        public static async Task<List<MenuModel>> GetAllMenu(WebApiProxy proxy)
        {
            List<SiteModel> Sites = GetAllSites(proxy).Result;
            List<ViewModel> Views = GetAllViews(proxy).Result;
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("@Id", -1);
            var dataSet = await proxy.ExecuteDataset("SP_MenuSelect", dict);
            if (!ReferenceEquals(dataSet, null) && dataSet.Tables.Count > 0)
            {
                return (from DataRow row in dataSet.Tables[0].Rows
                        select new MenuModel
                        {
                            Oid = Convert.ToInt32(row["Id"].ToString()),
                            Name = row["Name"].ToString(),
                            URL = row["URL"].ToString(),
                            IsExternal = Convert.ToBoolean(row["IsExternal"].ToString()),
                            Order = Convert.ToInt32(row["Order"].ToString()),
                            ViewId = Convert.ToInt32(row["ViewId"].ToString()),
                            SiteId = row["SiteId"].ToString() == string.Empty ? 0 : Convert.ToInt32(row["SiteId"].ToString()),
                            SiteName = Sites.FirstOrDefault(it => it.Oid == Convert.ToInt32(row["SiteId"].ToString())).Title,
                            ViewName = Views.FirstOrDefault(it => it.Oid == Convert.ToInt32(row["ViewId"].ToString())).Name
                        }).ToList();
            }
            return new List<MenuModel>();
        }


        public static async Task<List<ContentStyleModel>> GetAllContents(WebApiProxy proxy)
        {
            List<ContentStyleModel> ContentList = new List<ContentStyleModel>();

            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("@Oid", -1);
            dict.Add("@LoadOnlyActive", 0);

            var dataSet = await proxy.ExecuteDataset("SP_ContentsSelect", dict);
            if (!ReferenceEquals(dataSet, null) && dataSet.Tables.Count > 0)
            {
                return (from DataRow row in dataSet.Tables[0].Rows
                        select new ContentStyleModel
                        {
                            Id = Convert.ToInt32(row["Oid"].ToString()),
                            Name = row["Name"].ToString(),
                            Description = row["Descr"].ToString(),
                            IsActive = bool.Parse(row["IsActive"].ToString()),
                            ViewID = row["Views"].ToString() == string.Empty ? 0 : Convert.ToInt32(row["Views"].ToString())
                        }).ToList();
            }
            return ContentList;
        }

        public static async Task<List<SiteModel>> GetSearchSite(WebApiProxy proxy, string viewObject)
        {
            List<SiteModel> SearchList = new List<SiteModel>();
            Dictionary<string, object> dicParams = new Dictionary<string, object>();
            dicParams.Add("@Name", viewObject);

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