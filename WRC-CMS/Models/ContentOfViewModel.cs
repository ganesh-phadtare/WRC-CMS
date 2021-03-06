﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WRC_CMS.Repository;

namespace WRC_CMS.Models
{
    public class ContentOfViewModel : ICommon
    {
        public int Id { get; set; }
        public int ContentId { get; set; }
        public int ViewId { get; set; }
        public int SiteId { get; set; }
        public string SiteName { get; set; }
        public int Order { get; set; }
        public string ContentName { get; set; }
        public string ViewName { get; set; }

        public int CurrentObjectId
        {
            get { return Id; }
        } 
    }
    public class CombineContentViewModel
    {
        public List<ContentOfViewModel> ContentViewList { get; set; }
        public ContentOfViewModel ContentViewDetails { get; set; }
        public List<ViewModel> ViewList { get; set; }
        public List<ContentStyleModel> ContentList { get; set; }
        public int SiteId { get; set; }
        public string SiteName { get; set; }
        public int ContentId { get; set; }
        public int ViewId { get; set; }
    }
}