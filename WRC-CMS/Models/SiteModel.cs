using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WRC_CMS.Models
{
    public class SiteModel : BaseModel
    {
        ////[Display(Name = "ID")]
        ////public int Empid { get; set; }

        //[Required(ErrorMessage = "Name is required.")]
        //public string Name { get; set; }

        //[Required(ErrorMessage = "URL is required.")]
        //public string URL { get; set; }

        ////[Required(ErrorMessage = "Address is required.")]
        ////public string Logo { get; set; }  

        //[Required(ErrorMessage = "Titile is required.")]
        //public string Title { get; set; }

        //public bool IsActive { get; set; }

        public List<ViewModel> ViewList { get; set; }
    }
}