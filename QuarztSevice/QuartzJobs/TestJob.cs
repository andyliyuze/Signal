using log4net;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeassageCache;
using Model;
using DAL;
using DAL.Interface;
namespace QuarztSevice.QuartzJobs
{
    public sealed class TestJob : IJob
    {
        private readonly IMessageService _Msgservice;
        private readonly IPrivateMessages_DAL _IDAL;
        public TestJob(MessageService Msgservice,PrivateMessages_DAL PrivateMessageService) {

            _Msgservice = Msgservice;
            _IDAL = PrivateMessageService;
        }
        private readonly ILog _logger = LogManager.GetLogger(typeof(TestJob));

        public void Execute(IJobExecutionContext context)
        {
           

          List<PrivateMessage> MsgList=  _Msgservice.PopMesFromMessageList("PrivateMessageList", 500);
          if (MsgList != null)
          {
              _IDAL.AddList(MsgList);
              _logger.InfoFormat(DateTime.Now + ":添加消息：500条");
          }
        }
    }
}
