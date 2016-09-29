using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(EmailAnalytics.Startup))]
namespace EmailAnalytics
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
