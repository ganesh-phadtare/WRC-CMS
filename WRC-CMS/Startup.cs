using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WRC_CMS.Startup))]
namespace WRC_CMS
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
