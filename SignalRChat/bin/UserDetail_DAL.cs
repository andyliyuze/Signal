using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using Model;
using DAL.Interface;
namespace DAL
{
    public class UserDetail_DAL:IUserDetail_DAL
    {

        public string CheckLogin(string name, string pwd)
        {
            string id = "";
            using (var context = new ChatContext())
            {

                var model = context.UserInfo.Where(a => a.Pwd == pwd && a.UserName == name).FirstOrDefault();
                if (model != null)
                {

                    id = model.UserId.ToString();
                }
            }
            return id;



        }

        public bool Add(UserDetail model)
        {

            using (var context = new ChatContext())
            {
                context.UserDetail.Add(model);
                return context.SaveChanges() > 0;
            }
        }
        public bool Delete(UserDetail model) { bool Flag = false; return Flag; }
        public bool Update(UserDetail model) { bool Flag = false; return Flag; }
        //检查用户名是否已被使用
        public static bool UserNameIsUsed(string Name) {
            using (var context = new ChatContext())
            {

               var model= context.UserDetail.Where<UserDetail>(a => a.UserName == Name).FirstOrDefault();
               if (model != null) { return false; }
               else { return true; }
            }
        
        
        }

    }
}
