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

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "URL is required.")]
        [StringLength(100, ErrorMessage = "URL cannot be longer than 100 characters.")]
        public string URL { get; set; }

        public bool IsExternal { get; set; }

        public int Order { get; set; }

        public int ViewId { get; set; }

        [Display(Name = "View")]
        public string ViewName { get; set; }

        public int SiteId { get; set; }
        public string SiteName { get; set; }

        public List<ViewModel> View { get; set; }
        public List<SiteModel> Site { get; set; }
    }

    public class MenuModelLD
    {
        public List<MenuModel> ListView { get; set; }
        public MenuModel DetailView { get; set; }
        public string SiteName { get; set; }
        public int SiteID { get; set; }
    }
}