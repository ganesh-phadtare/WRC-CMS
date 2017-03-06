﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WRC_CMS.Communication;
using WRC_CMS.Models;
using WRC_CMS.Repository;

namespace WRC_CMS.Controllers
{
    public class BulkDataController : Controller
    {
        WebApiProxy proxy = new WebApiProxy();
        public ActionResult BulkData()
        {
            return View();
        }

        [HttpPost]
        public ActionResult BulkData(string commandName, BulkDataModel bulkDataModel)
        {
            try
            {
                if (ModelState.IsValid && bulkDataModel != null && bulkDataModel.NoOfRecords != 0)
                {
                    if (Request.Form["bt1"] != null)
                    {
                        CreateSites(bulkDataModel.NoOfRecords);
                    }
                    else if (Request.Form["bt2"] != null)
                    {
                        CreateViews(bulkDataModel.NoOfRecords);
                    }
                    else if (Request.Form["bt3"] != null)
                    {
                        CreateStaticContent(bulkDataModel.NoOfRecords);
                    }
                    else if (Request.Form["bt4"] != null)
                    {
                        CreateStaticMenu(bulkDataModel.NoOfRecords);
                    }
                }
                return View();
            }
            catch
            {
                return View();
            }
        }

        void CreateSites(int noOfRecords)
        {
            for (int i = 0; i < noOfRecords; i++)
            {
                Dictionary<string, object> dicParams = new Dictionary<string, object>();
                dicParams.Add("@Oid", -1);
                dicParams.Add("@Name", MockData.Company.Name());
                dicParams.Add("@url", MockData.Internet.DomainName());
                dicParams.Add("@Logo", new ComplexDataModel(typeof(Byte[]), GetImage()));
                dicParams.Add("@Title", MockData.Product.ProductName());
                dicParams.Add("@IsActive", MockData.RandomNumber.Next(0, 1));

                proxy.ExecuteNonQuery("SP_SiteAddUp", dicParams);
            }
        }

        private object GetImage()
        {
            using (var ms = new MemoryStream())
            {
                Image imgToResize = Image.FromFile(Server.MapPath(@"..\Images\bb.jpg"));

                Bitmap b = new Bitmap(100, 100);
                Graphics g = Graphics.FromImage((Image)b);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                g.DrawImage(imgToResize, 0, 0, 100, 100);
                g.Dispose();

                b.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);

                return ms.ToArray();// string.Concat(ms.ToArray().Select(k => Convert.ToString(k, 2)));
                //return 0101010101010;
            }
        }

        void CreateViews(int noOfRecords)
        {
            for (int i = 0; i < noOfRecords; i++)
            {
                Dictionary<string, object> dicParams = new Dictionary<string, object>();
                dicParams.Add("@Oid", -1);
                dicParams.Add("@Name", MockData.Company.Name());
                dicParams.Add("@url", MockData.Internet.DomainName());
                dicParams.Add("@Logo", GetImage());
                dicParams.Add("@Title", MockData.Product.ProductName());
                dicParams.Add("@IsActive", MockData.RandomNumber.Next(0, 1));
                dicParams.Add("@IsAuth", MockData.RandomNumber.Next(0, 1));
                dicParams.Add("@IsDefault", MockData.RandomNumber.Next(0, 1));
                dicParams.Add("@IsMenu", MockData.RandomNumber.Next(0, 1));

                proxy.ExecuteNonQuery("SP_ViewAddUp", dicParams);
            }
        }

        void CreateStaticContent(int noOfRecords)
        {
            for (int i = 0; i < noOfRecords; i++)
            {
                Dictionary<string, object> dicParams = new Dictionary<string, object>();

                dicParams.Add("@Oid", -1);
                dicParams.Add("@Views", MockData.RandomNumber.Next(0, GetMaxNumber("Views")));
                dicParams.Add("@Name", MockData.Company.Name());
                dicParams.Add("@Descr", MockData.Address.SecondaryAddress());
                dicParams.Add("@IsActive", MockData.RandomNumber.Next(0, 1));
                proxy.ExecuteNonQuery("SP_ContentsAddUp", dicParams);
            }
        }

        void CreateStaticMenu(int noOfRecords)
        {

        }

        int GetMaxNumber(string type)
        {
            return 1;
        }
    }
}