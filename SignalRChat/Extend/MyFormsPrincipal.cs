using Microsoft.AspNet.SignalR;
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







        /// <summary>
        /// 执行用户登录操作
        /// </summary>
        /// <param name="loginName">登录名</param>
        /// <param name="userData">与登录名相关的用户信息</param>
        /// <param name="expiration">登录Cookie的过期时间，单位：分钟。</param>
        public static void SignIn(string loginName, TUserData userData, int expiration)
        {
            if (string.IsNullOrEmpty(loginName))
                throw new ArgumentNullException("loginName");
            if (userData == null)
                throw new ArgumentNullException("userData");

            // 1. 把需要保存的用户数据转成一个字符串。
            string data = null;
            if (userData != null)
                data = (new JavaScriptSerializer()).Serialize(userData);


            // 2. 创建一个FormsAuthenticationTicket，它包含登录名以及额外的用户数据。
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                2, loginName, DateTime.Now, DateTime.Now.AddDays(1), true, data);


            // 3. 加密Ticket，变成一个加密的字符串。
            string cookieValue = FormsAuthentication.Encrypt(ticket);

            try
            {
                // 4. 根据加密结果创建登录Cookie
                HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, cookieValue);
                cookie.HttpOnly = true;
                cookie.Secure = FormsAuthentication.RequireSSL;
                cookie.Domain = FormsAuthentication.CookieDomain;
                cookie.Path = FormsAuthentication.FormsCookiePath;
                if (expiration > 0)
                    cookie.Expires = DateTime.Now.AddMinutes(expiration);

                HttpContext context = HttpContext.Current;
                if (context == null)
                    throw new InvalidOperationException();

                // 5. 写登录Cookie
                context.Response.Cookies.Remove(cookie.Name);
                context.Response.Cookies.Add(cookie);
            }
            catch(Exception e) { throw e; }
        }

        /// <summary>
        /// 尝试从HttpRequest.Cookies中构造一个MyFormsPrincipal对象
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static MyFormsPrincipal<TUserData> TryParsePrincipal(HttpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            // 1. 读登录Cookie
            HttpCookie cookie = request.Cookies[FormsAuthentication.FormsCookieName];
            if (cookie == null || string.IsNullOrEmpty(cookie.Value))
                return null;

            try
            {
                TUserData userData = null;
                // 2. 解密Cookie值，获取FormsAuthenticationTicket对象
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);

                if (ticket != null && string.IsNullOrEmpty(ticket.UserData) == false)
                    // 3. 还原用户数据
                    userData = (new JavaScriptSerializer()).Deserialize<TUserData>(ticket.UserData);

                if (ticket != null && userData != null)
                    // 4. 构造我们的MyFormsPrincipal实例，重新给context.User赋值。
                    return new MyFormsPrincipal<TUserData>(ticket, userData);
            }
            catch { /* 有异常也不要抛出，防止攻击者试探。 */ }
            return null;
        }



        /// <summary>
        /// 尝试从HttpRequest.Cookies中构造一个MyFormsPrincipal对象
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static MyFormsPrincipal<TUserData> TryParsePrincipalV2(IDictionary<string, Cookie> Cookies)
        {
            if (Cookies == null)
                throw new ArgumentNullException("Cookies");

            // 1. 读登录Cookie
            Cookie cookie = Cookies[FormsAuthentication.FormsCookieName];
            if (cookie == null || string.IsNullOrEmpty(cookie.Value))
                return null;

            try
            {
                TUserData userData = null;
                // 2. 解密Cookie值，获取FormsAuthenticationTicket对象
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);

                if (ticket != null && string.IsNullOrEmpty(ticket.UserData) == false)
                    // 3. 还原用户数据
                    userData = (new JavaScriptSerializer()).Deserialize<TUserData>(ticket.UserData);

                if (ticket != null && userData != null)
                    // 4. 构造我们的MyFormsPrincipal实例，重新给context.User赋值。
                    return new MyFormsPrincipal<TUserData>(ticket, userData);
            }
            catch { /* 有异常也不要抛出，防止攻击者试探。 */ }
            return null;
        }


        /// <summary>
        /// 根据HttpContext对象设置用户标识对象
        /// </summary>
        /// <param name="context"></param>
        public static void TrySetUserInfo(HttpContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            MyFormsPrincipal<TUserData> user = TryParsePrincipal(context.Request);
            if (user != null)
                context.User = user;
        }
    }



  
}