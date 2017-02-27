using System;
using System.Collections.Generic;
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
    }
}