using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using DAL.Interface;
namespace DAL
{
   public class PrivateMessages_DAL:IPrivateMessages_DAL
    {
       public bool AddList(List<PrivateMessage> list)
       {

           using (var context = new ChatContext())
           {


               foreach (var model in list)
               {
                   try
                   
                   {
                       List<PrivateMessage> l = context.PrivateMessage.ToList();

                       context.PrivateMessage.Add(model);
                   }
                   catch (Exception e)
                   {
                       throw e;
                   }

               }
               try
               {
                   int i = context.SaveChanges();
                   return i == list.Count;
               }
               catch (Exception e)
               {

                   throw e;
               }
           }
           

       }

       
    }
}
