using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebClientApplication.Startup))]
namespace WebClientApplication
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            app.MapSignalR();
        }
    }
}
