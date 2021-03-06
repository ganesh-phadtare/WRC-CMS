﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WRC_CMS.Models
{
    public class SiteModel : BaseModel
    {

        [Required(ErrorMessage = "URL is required.")]
        public string URL { get; set; }
        //[Display(Name = "Site")]       
        public List<ViewModel> ViewList { get; set; }     
    }

    public class CombineSiteModel
    {
        public List<SiteModel> SiteList { get; set; }       
        public SiteModel SiteView { get; set; }
    }
   
}