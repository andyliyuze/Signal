using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
namespace MeassageCache.Interface
{
  public  interface IUserCacheService
    {
      bool BeFriends(string uidA, string uidB);
    

    
    }
}
