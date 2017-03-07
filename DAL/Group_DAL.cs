using System;
using DAL.Interface;
using Model;

namespace DAL
{
    public class Group_DAL : BaseDAL<Group>, IGroup_DAL
    {
        public bool Create(Group model)
        {
            return Add(model);
        }
    }
}
