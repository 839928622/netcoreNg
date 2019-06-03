using DatingApp.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DataContext: IdentityDbContext<User,Role,int,IdentityUserClaim<int>,UserRole,IdentityUserLogin<int>,IdentityRoleClaim<int>,IdentityUserToken<int>>
    {
        public DataContext(DbContextOptions<DataContext> options):base(options)
        {
        }
           public DbSet<Value> Values { get; set; }
        //    public DbSet<User> Users { get; set; }
           public DbSet<Photo> Photos { get; set; }
           public DbSet <Like> Likes { get; set; }
           public DbSet<Message> Messages { get; set; }

           protected override void OnModelCreating(ModelBuilder builder)
           {
                   base.OnModelCreating(builder);

                   builder.Entity<UserRole>(userRole => {

                        userRole.HasKey(s => new {s.UserId,s.RoleId}); // 联合主键

                        userRole.HasOne(ur => ur.Role) // 用户角色表 有一个角色 多个用户角色 有外键 RoleId 必须的
                        .WithMany( r => r.UserRoles  )
                        .HasForeignKey(ur => ur.RoleId)
                        .IsRequired();
// 一个用户可以拥有多个角色、一个角色也可以被多个用户所拥有
                         userRole.HasOne(ur => ur.User)
                        .WithMany( r => r.UserRoles  )
                        .HasForeignKey(ur => ur.UserId)
                        .IsRequired();
                   });
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