using ServiceStack;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeassageCache.Common
{
   public static class RedisExtend
    {
        public static T GetEntity<T>(this RedisClient redisClient, string key)
        {

            T model = redisClient.GetAllEntriesFromHash(key).ToJson().FromJson<T>();
            return model;

        }
    }
}
