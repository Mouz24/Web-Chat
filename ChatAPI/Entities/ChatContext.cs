using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Entities
{
    public class ChatContext : DbContext
    {
        public ChatContext(DbContextOptions options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Message> messages { get; set; }
        public DbSet<Tag> tags { get; set; }
        public DbSet<MessageWithTags> messageWithTags{ get; set;}
    }
}