using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interface
{
    public interface IBroadcastMessages_DAL
    {
        List<BroadcastMessage> GetLastMessage(int count);
        bool AddList(List<BroadcastMessage> list);


    }

}
