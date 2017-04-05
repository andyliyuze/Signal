using System.Web;
using Microsoft.AspNet.SignalR;
using Autofac;
using Model;
using DAL.Interface;
using SignalRChat.Extend;
using MeassageCache.Interface;

namespace SignalRChat
{

    public class MyBaseHub : Hub
    {
         private readonly IUserService _service ;
        private readonly IGroup_DAL _IGroupDal;
        private readonly IMessageService _Msgservice;
        private readonly ILifetimeScope _hubLifetimeScope;
        public MyBaseHub(ILifetimeScope lifetimeScope)
        {
            _hubLifetimeScope = lifetimeScope.BeginLifetimeScope();
            _service = _hubLifetimeScope.Resolve<IUserService>();
            _IGroupDal = _hubLifetimeScope.Resolve<IGroup_DAL>();
            _Msgservice = _hubLifetimeScope.Resolve<IMessageService>();
        }
        public MyBaseHub()
        {
            string a = "1";
        }
        protected virtual MyFormsPrincipal<UserDetail> User
        {
            get {
                MyFormsPrincipal<UserDetail> Principal;
                try
                {                
                    if (HttpContext.Current != null)
                    {

                        Principal = MyFormsPrincipal<UserDetail>.TryParsePrincipal(HttpContext.Current.Request);
                    }
                    else
                    {
                        Principal = MyFormsPrincipal<UserDetail>.TryParsePrincipalV2(Context.RequestCookies);
                    }
                    return Principal;
                }
                catch {
                    return null;
                }      
            }
        }

        protected  string UserId { get; set; }

        public void SetUserCId(string cid)
        {


            User.UserData.UserCId = cid;
        }


    }




 
}