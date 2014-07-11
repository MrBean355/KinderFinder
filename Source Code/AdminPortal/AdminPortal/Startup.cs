using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AdminPortal.Startup))]
namespace AdminPortal
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
