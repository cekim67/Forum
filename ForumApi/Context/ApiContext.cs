using Microsoft.EntityFrameworkCore;
using ForumApi.Models.Entities;

namespace ForumApi.Context
{
    public class ApiContext : DbContext
    {
        public ApiContext(DbContextOptions<ApiContext> options) : base(options) { }

       
        public DbSet<User> Users { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<Reply> Replies { get; set; }
        public DbSet<Like> Likes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           

            modelBuilder.Entity<Topic>()
                .HasOne(t => t.User)
                .WithMany(u => u.Topics)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.NoAction); 

            modelBuilder.Entity<Reply>()
                .HasOne(r => r.User)
                .WithMany(u => u.Replies)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.NoAction);

           

            modelBuilder.Entity<Like>()
                .HasOne(l => l.User)
                .WithMany(u => u.Likes)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.NoAction);

           
            modelBuilder.Entity<Like>()
                .HasIndex(l => new { l.UserId, l.TopicId })
                .IsUnique()
                .HasFilter("[TopicId] IS NOT NULL AND [ReplyId] IS NULL") 
                .HasDatabaseName("IX_Like_User_Topic"); 

            // Bir kullanıcı bir cevabı yalnızca bir kez beğenebilir
            modelBuilder.Entity<Like>()
                .HasIndex(l => new { l.UserId, l.ReplyId })
                .IsUnique()
                .HasFilter("[ReplyId] IS NOT NULL AND [TopicId] IS NULL") 
                .HasDatabaseName("IX_Like_User_Reply"); 

            base.OnModelCreating(modelBuilder);
        }
    }
}