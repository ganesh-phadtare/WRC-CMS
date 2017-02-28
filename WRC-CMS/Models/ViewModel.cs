using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WRC_CMS.Models
{
    public class ViewModel : BaseModel
    {
        public bool IsDem { get; set; }
        public bool IsAuth { get; set; }
        public bool CreateMenu { get; set; }
        public int SiteID { get; set; }

        public bool IsDefault { get; set; }

        public List<ContentStyleModel> Contents { get; set; }

        public List<SiteModel> Site { get; set; }       
        [Required(ErrorMessage = "Site is required.")]
        [StringLength(100, ErrorMessage = "Site cannot be longer than 100 characters.")]
        [Display(Name = "Site")]
        public string SelectSite { get; set; }
    }
}