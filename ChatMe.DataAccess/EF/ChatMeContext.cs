﻿using ChatMe.DataAccess.Entities;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace ChatMe.DataAccess.EF
{
    public class ChatMeContext : IdentityDbContext<User>
    {
        public ChatMeContext() : base("ChatMe") { }

        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Dialog>()
                .HasMany(d => d.Users)
                .WithMany();

            modelBuilder.Entity<Message>()
                .HasRequired(m => m.Dialog)
                .WithMany(d => d.Messages)
                .WillCascadeOnDelete(true);
        }

        public static ChatMeContext Create()
        {
            return new ChatMeContext();
        }
    }
}