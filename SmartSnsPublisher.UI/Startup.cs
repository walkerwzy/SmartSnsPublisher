using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SmartSnsPublisher.UI.Startup))]
namespace SmartSnsPublisher.UI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
