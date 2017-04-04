using System;
using ServiceStack.Redis;
using MeassageCache.Interface;
namespace MeassageCache
{
    public class GroupMemberService : IGroupMemberService
    {
        public bool GetItemByMemberId(Guid MemberId, Guid GroupId)
        {
            try
            {
                using (RedisClient redisClient = new RedisClient("127.0.0.1", 6379))
                {
                    string key = "Group:UserId" + MemberId.ToString();
                    byte[] value = GroupId.ToByteArray();
                    long i = redisClient.SIsMember(key, value);
                    if (i >= 1) { return true; }
                    else { return false; }
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
