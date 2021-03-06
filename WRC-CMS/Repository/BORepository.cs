﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WRC_CMS.Communication;
using WRC_CMS.Models;

namespace WRC_CMS.Repository
{
    public class BORepository
    {
        public static Dictionary<string, string> GetProcedureInfo(object ModelObject, bool IsDelete = false)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            if (ModelObject is ContentStyleModel && IsDelete == true)
            {
                dic.Add("ProcedureName", "SP_ContentsDel");
                dic.Add("Id", "@Id");
                return dic;
            }
            else if (ModelObject is ViewModel && IsDelete == true)
            {
                dic.Add("ProcedureName", "SP_ViewDel");
                dic.Add("Oid", "@Id");
                return dic;
            }
            else if (ModelObject is MenuModel && IsDelete == true)
            {
                dic.Add("ProcedureName", "SP_MenuDel");
                dic.Add("Id", "@Id");
                return dic;
            }
            else if (ModelObject is SiteDbModel && IsDelete == true)
            {
                dic.Add("ProcedureName", "SP_SiteDBDel");
                dic.Add("Id", "@Id");
                return dic;
            }
            else if (ModelObject is SiteMiscModel && IsDelete == true)
            {
                dic.Add("ProcedureName", "SP_SiteMiscDel");
                dic.Add("Id", "@Id");
                return dic;
            }
            else if (ModelObject is ViewModel)
            {
                dic.Add("ProcedureName", "SP_ViewAddUp");
                dic.Add("Oid", "@Id");
                dic.Add("Name", "@Name");
                dic.Add("Title", "@Title");
                dic.Add("Logo", "@Logo");
                dic.Add("Orientation", "@Orientation");
                dic.Add("IsActive", "@IsActive");
                dic.Add("Authorized", "@Authorized");
                dic.Add("IsDefault", "@IsDefault");
                dic.Add("SiteID", "@SiteId");
                return dic;
            }
            else if (ModelObject is SiteDbModel)
            {
                dic.Add("ProcedureName", "SP_SiteDBAddUp");
                dic.Add("Id", "@Id");
                dic.Add("Name", "@Name");
                dic.Add("Server", "@Server");
                dic.Add("Database", "@Database");
                dic.Add("UserID", "@UserID");
                dic.Add("Password", "@Password");
                dic.Add("Description", "@Description");
                dic.Add("SiteId", "@SiteId");
                return dic;
            }
            else if (ModelObject is SiteMiscModel)
            {
                dic.Add("ProcedureName", "SP_SiteMiscAddUp");
                dic.Add("Id", "@Id");
                dic.Add("Key", "@Key");
                dic.Add("Value", "@Value");
                dic.Add("SiteId", "@SiteId");
                return dic;
            }
            else if (ModelObject is ContentStyleModel)
            {
                dic.Add("ProcedureName", "SP_ContentsAddUp");
                dic.Add("Id", "@Id");
                dic.Add("Name", "@Name");
                dic.Add("Type", "@Type");
                dic.Add("Orientation", "@Orientation");
                dic.Add("Data", "@Data");
                dic.Add("Description", "@Description");
                dic.Add("IsActive", "@IsActive");
                dic.Add("SiteID", "@SiteID");
                return dic;
            }
            else if (ModelObject is ContentOfViewModel)
            {
                dic.Add("ProcedureName", "SP_ContentOfViewAddUp");
                dic.Add("Id", "@Id");
                dic.Add("ContentId", "@ContentId");
                dic.Add("ViewId", "@ViewId");
                dic.Add("SiteId", "@SiteId");
                dic.Add("Order", "@Order");
                return dic;
            }
            else if (ModelObject is MenuModel)
            {
                dic.Add("ProcedureName", "SP_MenuAddUp");
                dic.Add("Id", "@Id");
                dic.Add("Name", "@Name");
                dic.Add("URL", "@URL");
                dic.Add("IsExternal", "@IsExternal");
                dic.Add("Order", "@Order");
                dic.Add("ViewId", "@ViewId");
                dic.Add("SiteId", "@SiteId");
                return dic;
            }

            return null;
        }

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
                            Logo = GetRowData<byte[]>(row["Logo"]),
                            IsActive = bool.Parse(row["IsActive"].ToString())
                        }).ToList();
            }
            return SiteList;
        }

        public static async Task<string> AddUpdateRecord(ICommon CommonModelObjet, ModelStateDictionary ModelState, WebApiProxy proxy)
        {
            if (ModelState.IsValid)
            {
                int ViewID = 0;
                await Task.Run(() =>
                {
                    if (CommonModelObjet.CurrentObjectId == 0)
                        ViewID = BORepository.AddUpdateRecord<ICommon>(proxy, CommonModelObjet, true).Result;
                    else
                        ViewID = BORepository.AddUpdateRecord<ICommon>(proxy, CommonModelObjet, false).Result;
                });

                if (ViewID > 0)
                    return "Record Saved successfully.";
            }

            return "Problem occured while saving record, kindly contact our support team.";
        }

        public static async Task<int> AddUpdateRecord<T>(WebApiProxy proxy, T ModelObject, bool IsNewObject = false) where T : class
        {
            Dictionary<string, string> ProcedureInfo = GetProcedureInfo(ModelObject);
            System.ComponentModel.PropertyDescriptorCollection pdc = System.ComponentModel.TypeDescriptor.GetProperties(ModelObject);
            Dictionary<string, object> dicParams = pdc.Cast<PropertyDescriptor>().Where(item => ProcedureInfo.ContainsKey(item.Name)).ToDictionary(key => ProcedureInfo[key.Name], value => value.GetValue(ModelObject));
            if (IsNewObject)
                dicParams["@Id"] = -1;

            if (dicParams.ContainsKey("@Logo"))
                dicParams["@Logo"] = 1014;
            DataSet dataSet = await proxy.ExecuteDataset(ProcedureInfo["ProcedureName"], dicParams);

            if (dataSet != null && dataSet.Tables != null && dataSet.Tables.Count > 0)
            {
                if (dataSet.Tables[0].Rows != null && dataSet.Tables[0].Rows.Count > 0)
                {
                    int ID = Convert.ToInt32(dataSet.Tables[0].Rows[0][0].ToString());
                    return ID;
                }
            }
            return -1;
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

            if (ViewObject.Authorized)
                dicParams.Add("@Authorized", "1");
            else
                dicParams.Add("@Authorized", "0");

            if (ViewObject.IsDefault)
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
                dicParams.Add("@Id", MenuObject.Id);
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
            //dicParams.Add("@Order", ContentStyleModelObject.Order);
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

        public static async Task<List<ViewModel>> GetAllViews(WebApiProxy proxy, int SiteId, int Id = -1)
        {
            List<SiteModel> Sites = GetAllSites(proxy, SiteId).Result;
            List<ViewModel> ViewList = new List<ViewModel>();
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("@Id", Id);
            dict.Add("@SiteId", SiteId);
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
                            IsDefault = bool.Parse(row["IsDefault"].ToString()),
                            Authorized = bool.Parse(row["Authorized"].ToString()),
                            Orientation = row["Orientation"].ToString(),
                            CreateMenu = CheckMenuExistOrNot(proxy, SiteId, Convert.ToInt32(row["Id"].ToString())).Result,
                            SiteID = row["SiteId"].ToString() == string.Empty ? 0 : Convert.ToInt32(row["SiteId"].ToString()),
                            SelectSite = Sites.FirstOrDefault(it => it.Oid == Convert.ToInt32(row["SiteId"].ToString())).Title,
                        }).ToList();
            }
            return ViewList;
        }

        public static async Task<List<SiteDbModel>> GetAllSiteDb(WebApiProxy proxy, int SiteId)
        {
            List<SiteModel> Sites = GetAllSites(proxy, SiteId).Result;
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("@Id", -1);
            dict.Add("@SiteId", SiteId);
            var dataSet = await proxy.ExecuteDataset("SP_SiteDBSelect", dict);
            if (!ReferenceEquals(dataSet, null) && dataSet.Tables.Count > 0 && Sites.Count > 0)
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
                            SiteName = Sites.FirstOrDefault().Title
                        }).ToList();
            }
            return new List<SiteDbModel>();
        }

        public static async Task<List<SiteMiscModel>> GetAllSiteMISC(WebApiProxy proxy, int siteId)
        {
            List<SiteModel> Sites = GetAllSites(proxy).Result;
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("@Id", -1);
            dict.Add("@SiteId", siteId);
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

        public static async Task<bool> CheckMenuExistOrNot(WebApiProxy proxy, int siteId, int ViewID)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("@Id", -1);
            dict.Add("@SiteId", siteId);
            dict.Add("@ViewId", ViewID);
            var dataSet = await proxy.ExecuteDataset("SP_MenuSelect", dict);
            if (!ReferenceEquals(dataSet, null) && dataSet.Tables.Count > 0)
            {
                if (dataSet.Tables[0].Rows.Count > 0)
                    return true;
            }
            return false;
        }

        public static async Task<List<MenuModel>> GetAllMenu(WebApiProxy proxy, int SiteId)
        {
            List<SiteModel> Sites = GetAllSites(proxy, SiteId).Result;
            List<ViewModel> Views = GetAllViews(proxy, SiteId).Result;
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("@Id", -1);
            dict.Add("@SiteId", SiteId);
            dict.Add("@ViewId", -1);
            var dataSet = await proxy.ExecuteDataset("SP_MenuSelect", dict);
            if (!ReferenceEquals(dataSet, null) && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
            {
                return (from DataRow row in dataSet.Tables[0].Rows
                        select new MenuModel
                        {
                            Id = Convert.ToInt32(row["Id"].ToString()),
                            Name = row["Name"].ToString(),
                            URL = row["URL"].ToString(),
                            IsExternal = Convert.ToBoolean(row["IsExternal"].ToString()),
                            Order = Convert.ToInt32(row["Order"].ToString()),
                            MaxOrder = Convert.ToInt32(row["MaxOrder"].ToString()),
                            ViewId = Convert.ToInt32(row["ViewId"].ToString()),
                            SiteId = row["SiteId"].ToString() == string.Empty ? 0 : Convert.ToInt32(row["SiteId"].ToString()),
                            SiteName = Sites.FirstOrDefault(it => it.Oid == Convert.ToInt32(row["SiteId"].ToString())).Title,
                            ViewName = Views.FirstOrDefault(it => it.Oid == Convert.ToInt32(row["ViewId"].ToString())).Name
                        }).ToList();
            }
            return new List<MenuModel>();
        }


        public static async Task<List<ContentStyleModel>> GetAllContents(WebApiProxy proxy, int SiteId, bool loadOnlyActive = false)
        {
            List<SiteModel> Sites = GetAllSites(proxy).Result;
            List<ContentStyleModel> ContentList = new List<ContentStyleModel>();
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("@Id", -1);

            if (loadOnlyActive)
                dict.Add("@LoadOnlyActive", 1);
            else
                dict.Add("@LoadOnlyActive", 0);
            dict.Add("@SiteId", SiteId);

            var dataSet = await proxy.ExecuteDataset("SP_ContentsSelect", dict);
            if (!ReferenceEquals(dataSet, null) && dataSet.Tables.Count > 0)
            {
                return (from DataRow row in dataSet.Tables[0].Rows
                        select new ContentStyleModel
                        {
                            Id = Convert.ToInt32(row["Id"].ToString()),
                            Name = row["Name"].ToString(),
                            Description = row["Description"].ToString(),
                            IsActive = bool.Parse(row["IsActive"].ToString()),
                            SiteID = Convert.ToInt32(row["SiteId"].ToString()),
                            Type = Convert.ToInt32(row["Type"].ToString()),
                            Orientation = row["Orientation"].ToString(),
                            Data = JsonConvert.DeserializeObject(row["Data"].ToString()).ToString(),
                            //Order = Convert.ToInt32(row["Order"].ToString()),
                            SiteName = Sites.FirstOrDefault(it => it.Oid == Convert.ToInt32(row["SiteId"].ToString())).Title,
                            // SiteName = Sites.FirstOrDefault(sit => sit.Oid == SiteId).Name,
                        }).ToList();
            }
            return ContentList;
        }

        public static async Task<List<ViewContentModel>> GetAllContents(WebApiProxy proxy, int siteId, int viewId)
        {
            List<SiteModel> Sites = GetAllSites(proxy).Result;
            List<ViewContentModel> ContentList = new List<ViewContentModel>();
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("@ViewId", viewId);
            dict.Add("@SiteId", siteId);

            var dataSet = await proxy.ExecuteDataset("SP_GetViewContents", dict);
            if (!ReferenceEquals(dataSet, null) && dataSet.Tables.Count > 0)
            {
                ContentList = dataSet.Tables[0].AsEnumerable().Select(row =>
                               new ViewContentModel
                               {
                                   Id = Convert.ToInt32(row["Id"].ToString()),
                                   Name = row["Name"].ToString(),
                                   Type = GetRowData<Int32>(row["Type"]),
                                   Orientation = GetRowData<string>(row["Orientation"]),
                                   Data = JsonConvert.DeserializeObject(row["Data"].ToString()).ToString(),
                                   Order = GetRowData<Int32>(row["Order"]),
                                   MaxOrder = Convert.ToInt32(row["MaxOrder"]),
                                   ViewID = viewId,
                                   SiteID = siteId
                               }).OrderBy(t => t.Order).ToList();
            }
            return ContentList;
        }

        public static T GetRowData<T>(object dr)
        {
            if (!object.ReferenceEquals(dr, null) && !(dr is System.DBNull))
            {
                if (typeof(T) == typeof(byte[]))
                    return (T)Convert.ChangeType(System.Text.Encoding.ASCII.GetBytes(Convert.ToString(dr)), typeof(T));
                return (T)Convert.ChangeType(dr, typeof(T));
            }
            return default(T);
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
                            Oid = Convert.ToInt32(row["Id"].ToString()),
                            Name = row["Name"].ToString(),
                            Title = row["Title"].ToString(),
                            URL = row["url"].ToString(),
                            Logo = GetRowData<byte[]>(row["Logo"]),
                            IsActive = bool.Parse(row["IsActive"].ToString())
                        }).ToList();
            }
            return SearchList;
        }

        public static async Task<List<ContentOfViewModel>> GetContentViews(WebApiProxy proxy, int SiteId)
        {
            List<SiteModel> Sites = GetAllSites(proxy, SiteId).Result;
            List<ViewModel> Views = GetAllViews(proxy, SiteId).Result;
            List<ContentStyleModel> Contents = GetAllContents(proxy, SiteId).Result;

            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("@Id", -1);
            dict.Add("@LoadOnlyActive", 0);
            dict.Add("@SiteId", SiteId);

            var dataSet = await proxy.ExecuteDataset("SP_ContentOfViewSelect", dict);
            if (!ReferenceEquals(dataSet, null) && dataSet.Tables.Count > 0)
            {
                return (from DataRow row in dataSet.Tables[0].Rows
                        select new ContentOfViewModel
                        {
                            Id = Convert.ToInt32(row["Id"].ToString()),
                            ContentId = Convert.ToInt32(row["ContentId"].ToString()),
                            ViewId = Convert.ToInt32(row["ViewId"].ToString()),
                            ViewName = Views.FirstOrDefault(i => i.Oid == Convert.ToInt32(row["ViewId"].ToString())).Name,
                            ContentName = Contents.FirstOrDefault(c => c.Id == Convert.ToInt32(row["ContentId"].ToString())).Name,
                            SiteId = row["SiteId"].ToString() == string.Empty ? 0 : Convert.ToInt32(row["SiteId"].ToString()),
                            SiteName = Sites.FirstOrDefault(it => it.Oid == Convert.ToInt32(row["SiteId"].ToString())).Title,
                            Order = Convert.ToInt32(row["Order"].ToString()),
                        }).ToList();
            }
            return new List<ContentOfViewModel>();
        }

        public static int Delete<T>(WebApiProxy proxy, T ModelObject) where T : class
        {
            try
            {
                Dictionary<string, string> ProcedureInfo = GetProcedureInfo(ModelObject, true);
                System.ComponentModel.PropertyDescriptorCollection pdc = System.ComponentModel.TypeDescriptor.GetProperties(ModelObject);
                Dictionary<string, object> dicParams = pdc.Cast<PropertyDescriptor>().Where(item => ProcedureInfo.ContainsKey(item.Name)).ToDictionary(key => ProcedureInfo[key.Name], value => value.GetValue(ModelObject));

                proxy.ExecuteNonQuery(ProcedureInfo["ProcedureName"], dicParams);
                return 1;
            }
            catch (SqlException ex)
            {
                throw new HttpException(500, ex.ToString());
            }
        }
    }
}