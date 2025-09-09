using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Inventory_Management_iTransition.Startup))]
namespace Inventory_Management_iTransition
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
