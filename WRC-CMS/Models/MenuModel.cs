using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WRC_CMS.Models
{
    public class MenuModel
    {
        public int Oid { get; set; }

        public List<SiteModel> Site { get; set; }
        public List<ViewModel> View { get; set; }

        [Required(ErrorMessage = "Site is required.")]
        [StringLength(100, ErrorMessage = "Site cannot be longer than 100 characters.")]
        [Display(Name = "Site")]
        public string SelectSite { get; set; }

        [Required(ErrorMessage = "View is required.")]
        [StringLength(100, ErrorMessage = "View cannot be longer than 100 characters.")]
        [Display(Name = "View")]
        public string SelectView { get; set; }
    }
}