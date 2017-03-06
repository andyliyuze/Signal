using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using System.Data.SqlClient;
using DAL.Interface;
namespace DAL
{
   public class BroadcastMessages_DAL:IBroadcastMessages_DAL
    {

       public int GetLastId() {

           using (var context = new ChatContext()) {

              
              //int i=Convert.ToInt32( context.BroadcastMessage.SqlQuery("exec proc_GetLastBrocastMessageId").ToString());
               return 0;
           
           }
       
       }
    public bool AddList (List<BroadcastMessage> list) 
       {
           using (var context = new ChatContext())
           {

               //foreach (var model in list)
               //{

               //    context.BroadcastMessage.Add(model);
                
               //}
               //线程池的线程并行处理；
              // Parallel.ForEach(list, item => context.BroadcastMessage.Add(item));//使用foreach
               //Parallel.For(0, list.Count, a => context.BroadcastMessage.Add(list[a]));//使用for
               //int i = context.SaveChanges();
               //return i == list.Count;
               return true;
           }
       }
    public List<BroadcastMessage> GetLastMessage(int count)
    {


        using (var context = new ChatContext())
        {

            //SqlParameter[] par = new SqlParameter[1];
            //par[0] = new SqlParameter("count",10);
            //var result = (from p in context.BroadcastMessage.SqlQuery("exec proc_GetMessage @count", par)
            //              select p).ToList<BroadcastMessage>();
            //return result;

            return null;
        }
    
    
    
    }
    public bool Add<BroadcastMessage>(BroadcastMessage model) { bool Flag = false; return Flag; }
    public bool Delete<BroadcastMessage>(BroadcastMessage model) { bool Flag = false; return Flag; }
    public bool Updata<BroadcastMessage>(BroadcastMessage model) { bool Flag = false; return Flag; }

    }
}
