using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Interface;
using Model;
namespace DAL
{
   public class Friends_DAL:IFriends_DAL
    {
       public bool BeFriends(Friends model) {

           try
           {
               using (var context = new ChatContext())
               {
                   context.Friends.Add(model);
                   return context.SaveChanges() > 0;


               }
           }
           catch (Exception e) {
               return false;
           }
       
       }


       public bool IsFriend(string uidA, string uidB)
       {
           try
           {
               using (var context = new ChatContext())
               {
                 Guid idA=   Guid.Parse(uidA);
                 Guid idB = Guid.Parse(uidB);
                 return context.Friends.Where(a => a.ApplyUserId == idA && a.ReceiverUserId == idB || a.ApplyUserId==idB && a.ReceiverUserId==idA).Count() > 0;
                   


               }
           }
           catch (Exception e)
           {
               return false;
           }
       }
    }
}
