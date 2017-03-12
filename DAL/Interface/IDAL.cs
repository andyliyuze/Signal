namespace DAL.Interface
{
    public interface IDAL<T>  
    {
             bool Add(T model);
             bool Delete(T model);

             bool Update(T model);

    }
    
 
   
    
}
