using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Security;

namespace SignalRChat.Extend
{
   
    
        //通用的用户实体
        public class MyFormsPrincipal<TUserData> : IPrincipal
            where TUserData : class, new()
        {
            //当前用户实例
            public IIdentity Identity { get; private set; }
            //用户数据
            public TUserData UserData { get; private set; }


            public MyFormsPrincipal(FormsAuthenticationTicket ticket, TUserData userData)
            {
                if (ticket == null)
                    throw new ArgumentNullException("ticket");
                if (userData == null)
                    throw new ArgumentNullException("userData");

                Identity = new FormsIdentity(ticket);
                UserData = userData;
            }





            public bool IsInRole(string role)
            {
                throw new NotImplementedException();
            }
        }
    
}