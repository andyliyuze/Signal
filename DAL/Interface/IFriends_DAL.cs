using Model;

namespace DAL.Interface
{
    public   interface IFriends_DAL
    {
         bool BeFriends(Friends model);

         bool IsFriend(string uidA, string uidB);
    }
}
