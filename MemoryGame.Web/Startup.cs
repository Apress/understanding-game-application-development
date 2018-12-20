using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(MemoryGame.Web.Startup))]
namespace MemoryGame.Web
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}
