using DAL.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using System.Data.Entity;

namespace DAL
{

    
    public class JoinGroupApply_DAL : BaseDAL<JoinGroupApply>, IJoinGroupApply_DAL
    {     
        public   JoinGroupApply GetItemById(Guid Id)
        {
            using (ChatContext context = new ChatContext())
            {
                JoinGroupApply model=  context.JoinGroupApply.Where(a=>a.Id==Id).FirstOrDefault();

                return model;
            }

        }

        public bool SetReadByIds(List<string> ids)
        {
            using (ChatContext context = new ChatContext())
            {
                foreach (var id in ids)
                {
                    Guid Id = Guid.Parse(id);
                    var item = context.JoinGroupApply.Where(a => a.Id == Id).FirstOrDefault();
                    item.HasReadResult = "已读";
                    context.Entry<JoinGroupApply>(item).State = EntityState.Modified;

                }
                int i = context.SaveChanges();
                return i >= ids.Count;
            }
        }

        public bool UpdateResult(JoinGroupApply apply)
        {
            try
            {
                using (var context = new ChatContext())
                {
                    return TryUpdate(apply, context);
                }
            }
            catch
            {
                return false;
            }
        }

        private bool TryUpdate(JoinGroupApply model, ChatContext context)
        { 
            JoinGroupApply apply = context.JoinGroupApply.Where(a => a.Id == model.Id ).FirstOrDefault();
            if (apply != null)
            {
                apply.HasReadResult = model.HasReadResult;
                apply.Result = model.Result;
                apply.ReplyTime = DateTime.Now;
                context.Set<JoinGroupApply>().Attach(apply);
                context.Entry<JoinGroupApply>(apply).State = EntityState.Modified;
                return context.SaveChanges() > 0;
            }
            else
            {
                return false;
            }
        }
    }
}
