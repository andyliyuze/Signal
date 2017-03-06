using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Web.Mvc;
namespace SignalRChat.Controllers
{
    public abstract class HubController<T> : Controller where T : Hub
    {
        protected IHubCallerConnectionContext<dynamic> Clients { get; private set; }
        protected IGroupManager Gruops { get; private set; }
         
        protected HubController()
        {


            var ctx = GlobalHost.ConnectionManager.GetHubContext<T>();
            Clients = ctx.Clients as IHubCallerConnectionContext<dynamic>;
            
            Gruops = ctx.Groups;
        }

    }
}