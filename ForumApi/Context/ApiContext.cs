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

           

            

            base.OnModelCreating(modelBuilder);
        }
    }
}