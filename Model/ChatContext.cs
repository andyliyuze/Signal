﻿using System.Data.Entity;

namespace Model
{
    public class ChatContext : DbContext
    {
        public ChatContext()
            : base("SignalRChat")
        {

          
        }
        public DbSet<UserDetail> UserDetail { get; set; }
        public DbSet<PrivateMessage> PrivateMessage { get; set; }
        public DbSet<BroadcastMessage> BroadcastMessage { get; set; }
        public DbSet<UserInfo> UserInfo { get; set; }

        public DbSet<FriendsApply> FriendsApply { get; set; }
        public DbSet<Friends> Friends { get; set; }


        public DbSet<Group> Group { get; set; }
        public DbSet<GroupMember> GroupMember { get; set; }
        public DbSet<JoinGroupApply> JoinGroupApply{ get; set; }

    }
}
