using Microsoft.Owin;
using Owin;
using System;
using System.Configuration;
using WRC_CMS.Repository;

[assembly: OwinStartupAttribute(typeof(WRC_CMS.Startup))]
namespace WRC_CMS
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            AppKeys.DefaultRespondURL = Convert.ToString(ConfigurationManager.AppSettings["DefaultRespondURL"]);
            ConfigureAuth(app);
        }
    }
}
