using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WRC_CMS.Models
{
    public class ContentStyleModel
    {
        public int Oid { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        public string Name { get; set; }

        private string _Description { get; set; }
        [Required(ErrorMessage = "Description is required.")]
        [AllowHtml]
        [UIHint("tinymce_full")]
        public string Description { get; set; }

        public bool IsActive { get; set; }

        public int ViewID { get; set; }

        public bool IsDefault { get; set; }

        [Required(ErrorMessage = "View is required.")]
        [StringLength(100, ErrorMessage = "View cannot be longer than 100 characters.")]
        [Display(Name = "View")]
        public string SelectView { get; set; }
        public List<ViewModel> View { get; set; }

    }
}