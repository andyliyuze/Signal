using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Interface;
using Model;
namespace DAL
{
   public  class UserInfo_DAL :IUserInfo_DAL
    {

        public bool Add(UserInfo model)
        {

            using (var context = new ChatContext())
            {
                context.UserInfo.Add(model);
                               
                return context.SaveChanges() > 0;
            }
        }
        public bool Delete(UserInfo model) { bool Flag = false; return Flag; }
        public bool Update(UserInfo model) { bool Flag = false; return Flag; }
    }
}
