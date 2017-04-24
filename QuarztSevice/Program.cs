using QuarztSevice;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace QuarztSevice
{
    class Program
    {
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "log4net.config"));
            HostFactory.Run(x =>
            {
                x.UseLog4Net();
                x.Service<ServiceRunner>();
                x.StartAutomatically();
                x.SetDescription("将Redis数据同步到SQLServer");
                x.SetDisplayName("Redis数据同步到SQLServer");
                x.SetServiceName("ChatDB数据同步服务");
                x.EnablePauseAndContinue();
            });
        }
    }
}
