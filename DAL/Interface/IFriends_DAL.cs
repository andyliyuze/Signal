using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interface
{
 public   interface IFriends_DAL
    {
         bool BeFriends(Friends model);

         bool IsFriend(string uidA, string uidB);
    }
}
