using Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class BaseDAL<T> where T : class
    {


     public    bool  Add(T model)
        {
            using (var context = new ChatContext())
            {

                context.Entry<T>(model).State = EntityState.Added;

              return  context.SaveChanges()>0;
               

            }
        }

     public bool Delete(T model)
        {
            using (var context = new ChatContext())
            {
                context.Entry<T>(model).State = EntityState.Modified;
                return context.SaveChanges() > 0;
                 

            }
        }

     public bool Update(T model)
        {
            using (var context = new ChatContext())
            {

                
                context.Entry<T>(model).State = EntityState.Modified;


                return context.SaveChanges() > 0;
            }
        }



    }
}
