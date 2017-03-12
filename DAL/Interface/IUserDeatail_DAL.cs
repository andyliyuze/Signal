using Model;

namespace DAL.Interface
{
    public interface IUserDetail_DAL : IDAL<UserDetail>
    {

        string CheckLogin(string name, string pwd);

       
    }
}
