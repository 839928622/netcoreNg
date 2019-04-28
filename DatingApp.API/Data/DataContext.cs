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

           protected override void OnModelCreating(ModelBuilder builder)
           {
               builder.Entity<Like>()
                       .HasKey(k => new {k.LikerId, k.LikeeId});
               builder.Entity<Like>()
                       .HasOne(u => u.Likee) // 只能给一个人点赞
                       .WithMany(u => u.Likers) // 可以被多个人点赞
                       .HasForeignKey(u => u.LikeeId) // 外键 User表的id
                       .OnDelete(DeleteBehavior.Restrict); // 当取消点赞的时候，不希望删除被点赞的用户

               builder.Entity<Like>()
                       .HasOne(u => u.Liker) // 
                       .WithMany(u => u.Likees) //
                       .HasForeignKey(u => u.LikerId) // 
                       .OnDelete(DeleteBehavior.Restrict); // 
               
                   
               
           }
    }
}