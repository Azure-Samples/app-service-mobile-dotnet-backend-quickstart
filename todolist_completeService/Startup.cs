using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(todolist_completeService.Startup))]

namespace todolist_completeService
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
        }
    }
}