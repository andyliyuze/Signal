using Model;
using System.Collections.Generic;

namespace DAL.Interface
{
    public interface IBroadcastMessages_DAL
    {
        List<BroadcastMessage> GetLastMessage(int count);
        bool AddList(List<BroadcastMessage> list);


    }

}
