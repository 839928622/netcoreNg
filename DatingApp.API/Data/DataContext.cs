using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DataContext:DbContext
    {
        public DataContext(DbContextOptions<DataContext> options):base(options)
        {
        }
           public DbSet<Value> Values { get; set; }
           public DbSet<User> Users { get; set; }
           public DbSet<Photo> Photos { get; set; }
           public DbSet <Like> Likes { get; set; }
           public DbSet<Message> Messages { get; set; }

           protected override void OnModelCreating(ModelBuilder builder)
           {
               builder.Entity<Like>()
                       .HasKey(k => new {k.LikerId, k.LikeeId});
               builder.Entity<Like>()
                       .HasOne(u => u.Likee) // 被关注者-可以被多个人关注
                       .WithMany(u => u.Likers) // 
                       .HasForeignKey(u => u.LikeeId) // 外键 User表的id
                       .OnDelete(DeleteBehavior.Restrict); // 当取消点赞的时候，不希望删除被点赞的用户

               builder.Entity<Like>()
                       .HasOne(u => u.Liker) // 关注者
                       .WithMany(u => u.Likees) // 可以关注多个人（被关注者）
                       .HasForeignKey(u => u.LikerId) // 
                       .OnDelete(DeleteBehavior.Restrict); // 

               builder.Entity<Message>()
                       .HasOne( u => u.Sender)
                       .WithMany(m => m.MessagesSent)
                       .OnDelete(DeleteBehavior.Restrict);

                   builder.Entity<Message>()
                       .HasOne( u => u.Recipient)
                       .WithMany(m => m.MessageReceived)
                       .OnDelete(DeleteBehavior.Restrict);

                   
               
           }
    }
}