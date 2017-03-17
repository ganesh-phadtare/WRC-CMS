using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WRC_CMS.Models
{
    public class BaseModel
    {
        public int Oid { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        //        [Remote("SiteExists", "Site", "Username is already taken.")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(250, ErrorMessage = "Name cannot be longer than 250 characters.")]
        public string Title { get; set; }

        //[Column(TypeName = "image")]
        public byte[] Logo { get; set; }

        public string LogoImage
        {
            get
            {
                if (Logo != null)
                {
                    string mimeType = "image/jpeg";
                    string base64 = Convert.ToBase64String(Logo);
                    return string.Format("data:{0};base64,{1}", mimeType, base64);
                }
                return string.Empty;
            }
        }

        public bool IsActive { get; set; }
    }
}