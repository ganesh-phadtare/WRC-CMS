using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace WRC_CMS.Models
{
    public class SiteMiscModel
    {
        public SiteMiscModel()
        {
            Keys = new List<string>();
            if (!ReferenceEquals(WebConfigurationManager.AppSettings["SiteMiscKeys"], null))
            {
                string StringKeys = WebConfigurationManager.AppSettings["SiteMiscKeys"];
                if (!string.IsNullOrEmpty(StringKeys))
                {
                    var Split = StringKeys.Split(',');
                    if (Split.Count() > 0)
                    {
                        foreach (var key in Split)
                        {
                            Keys.Add(key);
                        }
                    }
                }
            }
        }
        public int Id { get; set; }

        [Required(ErrorMessage = "Key is required.")]
        [StringLength(50, ErrorMessage = "Key cannot be longer than 50 characters.")]
        public string Key { get; set; }

        [Required(ErrorMessage = "Key is required.")]
        public List<string> Keys { get; set; }

        [Required(ErrorMessage = "Value is required.")]
        [StringLength(250, ErrorMessage = "Value cannot be longer than 250 characters.")]
        public string Value { get; set; }
        public int SiteId { get; set; }
        public string SiteName { get; set; }
    }

    public class SiteMiscModelLD
    {
        public List<SiteMiscModel> ListView { get; set; }
        public SiteMiscModel DetailView { get; set; }
        public string SiteName { get; set; }
        public int SiteID { get; set; }
    }
}