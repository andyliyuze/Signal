using Autofac;
using Autofac.Core;
using DAL;
using DAL.Interface;
using log4net;
using MeassageCache;
using MeassageCache.Interface;
using Model;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuarztSevice.QuartzJobs
{
    public sealed class BankCardJob : IJob
    {
       
        private readonly ILog _logger = LogManager.GetLogger(typeof(BankCardJob));
        private static IContainer Container { get; set; }
        private readonly IMessageService _Msgservice;

        private readonly IPrivateMessages_DAL _IDAL;
        public BankCardJob() {
            var scope = Container.BeginLifetimeScope();

             _IDAL = scope.Resolve<IPrivateMessages_DAL>();
            _Msgservice = scope.Resolve<IMessageService>();
        
        
        }
        public void Execute(IJobExecutionContext context)
        {
            using (var scope = Container.BeginLifetimeScope())
            {

            
                List<PrivateMessage> MsgList = _Msgservice.PopMesFromMessageList("PrivateMessageList", 500);
                if (MsgList.Count != 0)
                {
                    _IDAL.AddList(MsgList);
                    _logger.InfoFormat("时间：" + DateTime.Now + " 新增消息：" + MsgList.Count + "条");
                }
                return;
            }
        }
    }
}
