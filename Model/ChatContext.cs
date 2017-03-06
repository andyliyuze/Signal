﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class ChatContext : DbContext
    {
        public ChatContext()
            : base("SignalRChat")
        {

            Database.SetInitializer<ChatContext>(new DropCreateDatabaseIfModelChanges<ChatContext>());
        }
        public DbSet<UserDetail> UserDetail { get; set; }
        public DbSet<PrivateMessage> PrivateMessage { get; set; }
        public DbSet<BroadcastMessage> BroadcastMessage { get; set; }
        public DbSet<UserInfo> UserInfo { get; set; }

        public DbSet<FriendsApply> FriendsApply { get; set; }
        public DbSet<Friends> Friends { get; set; }

    }
}
