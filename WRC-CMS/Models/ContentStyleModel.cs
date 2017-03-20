using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WRC_CMS.Repository;

namespace WRC_CMS.Models
{
    public class ContentStyleModel : ICommon
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        public string Name { get; set; }
        public string Description { get; set; }

        public bool IsActive { get; set; }
        public int ViewID { get; set; }
        public bool IsDefault { get; set; }

        //[Required(ErrorMessage = "View is required.")]
        //[StringLength(100, ErrorMessage = "View cannot be longer than 100 characters.")]
        //[Display(Name = "View")]
        //public string SelectView { get; set; }
        //public List<ViewModel> View { get; set; }
        //public List<ViewModel> ViewList { get; set; }

        public int SiteID { get; set; }
        public string SiteName { get; set; }
        public int Type { get; set; }

        [Required(ErrorMessage = "Data is required.")]
        [AllowHtml]
        [UIHint("tinymce_full")]
        public string Data { get; set; }

        //public int Order { get; set; }
        public string Orientation { get; set; }
        public int SearchType { get; set; }
        public List<int> STyList { get; set; }
        public List<int> VTyList { get; set; }
        public string views { get; set; }
        public string searchty { get; set; }       
        public int CurrentObjectId
        {
            get { return Id; }
        }      
    }


    public struct ViewContentModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
        public string Orientation { get; set; }
        [AllowHtml]
        [UIHint("tinymce_full")]
        public string Data { get; set; }
        public int Order { get; set; }
        public int MaxOrder { get; set; }

        public bool IsUp
        {
            get
            {
                if (Order == 1)
                {
                    return true;
                }
                return false;
            }
        }

        public bool IsDown
        {
            get
            {
                if (MaxOrder == Order)
                {
                    return true;
                }
                return false;
            }
        }
        public int ViewID { get; set; }
        public int SiteID { get; set; }
    }


    public class CombineContentModel
    {
        public List<ContentStyleModel> ContentList { get; set; }
        public ContentStyleModel ContentView { get; set; }
        public string SelectView { get; set; }
        public string SiteName { get; set; }
        public int SiteID { get; set; }
        public List<ViewModel> ViewList { get; set; }

        //public int Type { get; set; }
        //public string Data { get; set; }
       //public int Order { get; set; }       
        //public string Orientation { get; set; }
    }
}