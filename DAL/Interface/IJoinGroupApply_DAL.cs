using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interface
{ 
  public   interface IJoinGroupApply_DAL

    {
        bool    Add(JoinGroupApply model);
        bool SetReadByIds(List<string> ids);
        JoinGroupApply GetItemById(Guid Id);
        bool  UpdateResult(JoinGroupApply apply);
    }

}