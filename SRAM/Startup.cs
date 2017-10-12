using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SRAM.Startup))]
namespace SRAM
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
