﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WRC_CMS.Models
{
    public class SiteModel : BaseModel
    {  
        //[Display(Name = "Site")]
        //public string SelectSite { get; set; }

        public List<ViewModel> ViewList { get; set; }
        //public ViewModel SiteView { get; set; }
    }

    public class CombineSiteModel
    {
        public List<SiteModel> SiteList { get; set; }
        public SiteModel SiteView { get; set; }
    }
   
}