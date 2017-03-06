using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public interface IDAL<T>  
    {
             bool Add(T model);
             bool Delete(T model);

             bool Update(T model);

    }
    
 
   
    
}
