using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeassageCache.Common
{
   public class RedisCofig
    {

        public static RedisEndpoint DefaultEndpoint = new RedisEndpoint() { Port = 6370, Host = "120.76.120.68", Password="123456" };
    }
}
