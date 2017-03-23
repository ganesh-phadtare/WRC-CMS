
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WRC_CMS.Repository;

namespace WRC_CMS.Models
{
    public class ViewModel : BaseModel, ICommon
    {
        public bool IsDefault { get; set; }
        public bool Authorized { get; set; }
        [Display(Name = "Menu")]
        public bool CreateMenu { get; set; }
        public int SiteID { get; set; }

        public List<ContentStyleModel> Contents { get; set; }

        public List<SiteModel> Site { get; set; }
        [Display(Name = "Site")]
        public string SelectSite { get; set; }

        [Required(ErrorMessage = "Orientation is required.")]
        [StringLength(100, ErrorMessage = "Orientation cannot be longer than 100 characters.")]

        public string Orientation { get; set; }
        public int CurrentObjectId
        {
            get { return Oid; }
        }
    }

    public class CombineModel
    {
        public CombineModel()
        {
            ViewAllContents = new List<ContentStyleModel>();
            ViewContents = new List<ViewContentModel>();
        }

        public List<ViewModel> views { get; set; }
        public ViewModel NewView { get; set; }
        public string SiteName { get; set; }
        public int SiteID { get; set; }
        
        public List<ViewContentModel> ViewContents { get; set; }

        public List<ContentStyleModel> ViewAllContents { get; set; }

        public bool UpdownEnabled
        {
            get
            {
                if (NewView != null && (NewView.Oid != 0 && NewView.Oid != -1))
                    return true;
                return false;
            }
        }
    }
}