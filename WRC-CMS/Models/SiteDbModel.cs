using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WRC_CMS.Models
{
    public class SiteDbModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        public string Name { get; set; }


        [Required(ErrorMessage = "Server is required.")]
        [StringLength(50, ErrorMessage = "Server cannot be longer than 50 characters.")]
        public string Server { get; set; }

        [Required(ErrorMessage = "Database is required.")]
        [StringLength(50, ErrorMessage = "Database cannot be longer than 50 characters.")]
        public string Database { get; set; }


        [Required(ErrorMessage = "User ID is required.")]
        [StringLength(20, ErrorMessage = "User ID cannot be longer than 20 characters.")]
        [Display(Name = "User ID")]
        public string UserID { get; set; }


        [Required(ErrorMessage = "Password is required.")]
        [StringLength(20, ErrorMessage = "Password cannot be longer than 20 characters.")]
        public string Password { get; set; }

        public string Description { get; set; }

        public int SiteId { get; set; }

        public string SiteName { get; set; }

        public List<SiteModel> Site { get; set; }

    }

    public class SiteDbModelLD
    {
        public List<SiteDbModel> ListView { get; set; }
        public SiteDbModel DetailView { get; set; }
        public string SiteName { get; set; }
        public int SiteID { get; set; }
    }
}