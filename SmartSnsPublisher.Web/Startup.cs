using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SmartSnsPublisher.Web.Startup))]
namespace SmartSnsPublisher.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
