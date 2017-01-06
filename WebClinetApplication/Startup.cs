using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebClinetApplication.Startup))]
namespace WebClinetApplication
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
