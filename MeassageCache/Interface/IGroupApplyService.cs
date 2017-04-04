﻿using Model;
using Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeassageCache.Interface
{
    interface IGroupApplyService
    {
        bool Add(JoinGroupApply model);
        bool SetReadByIds(List<string> ids);
        JoinGroupApply GetItemById(Guid Id);
        bool UpdateResult(JoinGroupApply apply);

        List<GroupApplyViewModel> GetGroupApplyByUId(Guid Id);
        List<GroupReplyViewModel> GetGroupReplyByUId(Guid Id);
    }
}
