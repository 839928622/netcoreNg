using System.Collections.Generic;
using System.Threading.Tasks;
using DatingApp.API.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using DatingApp.API.Helpers;
using System;

namespace DatingApp.API.Data
{
    public class DatingRepository : IDatingRepository
    {
        private readonly DataContext _context;
        public DatingRepository(DataContext context)
        {
            _context = context;

        }
        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity); //这里不适用asyn的原因是：在增加的时候，没有去查询数据库，只是保存在内存中
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<Like> GetLike(int userId, int recipientId)
        {
            return await _context.Likes
            .FirstOrDefaultAsync(
                u => u.LikerId == userId && u.LikeeId == recipientId
                );
        }

        public async Task<Photo> GetMainPhotoForUser(int userId)
        {
           return await _context.Photos.Where( u => u.UserId == userId).FirstOrDefaultAsync(p => p.IsMain); //获取主图
        }

        public async Task<Photo> GetPhoto(int id)
        {
            var photo = await _context.Photos.FirstOrDefaultAsync(p => p.Id == id);

            return photo;
        }

        public async Task<User> GetUser(int id)
        {
            var user = await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(u => u.Id == id);

            return user;
        }

        public async Task<PageList<User>> GetUsers(UserParams userParams)
        {
            var users =  _context.Users
            .Include(u => u.Photos)
            .OrderByDescending(x => x.LastActive)
            .AsQueryable() ; // 排序，按照上次活动的时间排序

            users = users.Where(u => u.Id != userParams.UserId);
           // 第一次筛选：剔除 当前用户自己，即不显示当前用户自己的信息
            users = users.Where(u => u.Gender == userParams.Gender);
           // 第二次筛选：选择传进来的性别参数

        if (userParams.Likers) // 我钟意的人
        {
        var userLikers = await GetUserLikes(userParams
        .UserId,userParams.Likers);
        users = users.Where(u => userLikers.Contains(u.Id)); // 到数据库检索该关注的人

        }
        if (userParams.Likees) // 钟意我的人
        {
             var userLikees = await GetUserLikes(userParams
        .UserId,userParams.Likers);
        users = users.Where(u => userLikees.Contains(u.Id));
        }
           if (userParams.MinAge != 18 || userParams.MaxAge != 99) 
           {
               var minDateOfBirth = DateTime.Today.AddYears(-userParams.MaxAge - 1) ;
               var maxDateOfBirth = DateTime.Today.AddYears(-userParams.MinAge) ;
            // 第三次帅选：选出符合用户期望的年龄段
               users = users
                       .Where (u => u.DateOfBirth >= minDateOfBirth
                              && u.DateOfBirth <= maxDateOfBirth);
           }

           if (!string.IsNullOrEmpty(userParams.OrderBy))
           {
               switch (userParams.OrderBy)
               {
                   case "created":
                    users = users.OrderByDescending(u => u.Created);
                    break;
                    //如果用户传来的参数是按创建账号时排序，则命中第一个条件，默认是按照上次活动的时间排序
                    default:
                    users = users.OrderByDescending(u => u.LastActive);
                    break;
               }
           }
            return await PageList<User>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);
        }

        private async Task<IEnumerable<int>> GetUserLikes(int id, bool likers)
        {
            var user = await _context.Users
            .Include(x=>x.Likers)
            .Include(x=> x.Likees) 
            .FirstOrDefaultAsync(u => u.Id == id); //获取用户关注的人 和 自己被关注的人
          if (likers)
          {
              // 喜欢的人= true，则检索传进来的id中
              return user.Likers.Where(u => u.LikeeId == id).Select(i => i.LikerId);
          }
          else
          {
           return user.Likees.Where(u => u.LikerId == id).Select( i => i.LikeeId);
          }
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0 ;
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages.FirstOrDefaultAsync(m => m.Id == id); //通过id查找该条消息
        }

        public async Task<PageList<Message>> GetMessagesForUser(MessageParams messageParams)
        {
            var messages = _context
            .Messages
            .Include(u => u.Sender).ThenInclude(p => p.Photos) // 第一次见到这种用法 显示在message实体中关联/带出 发送消息的人，然后再带出发送消息的人的头像/照片
            .Include(u => u.Recipient).ThenInclude(p => p.Photos)
            .AsQueryable();

            // 接下来筛选数据
            switch (messageParams.MessageContainer)
            {
                case "Inbox" : // 收件箱 RecipientId = 收件人id
                messages = messages.Where( u => u.RecipientId == messageParams.UserId && u.RecipientDeleted == false);
                break;
                case "Outbox": // 发件箱
                messages = messages.Where(u => u.SenderId == messageParams.UserId && u.SenderDeleted == false );
                break;
                default: // 默认 未读
                 messages = messages.Where( u=> u.RecipientId == messageParams.UserId && u.IsRead == false && u.RecipientDeleted == false);
                 break;
            }

            messages = messages.OrderByDescending(d => d.MessageSent); // 按照发送时间降序排序
            return await PageList<Message>.CreateAsync(messages,messageParams.PageNumber, messageParams.PageSize); // 返回分页的消息
        }

        public async Task<IEnumerable<Message>> GetMessageThread(int userId, int recipientId)
        {
          
            var messages = await _context
            .Messages
            .Include(u => u.Sender).ThenInclude(p => p.Photos) // 第一次见到这种用法 显示在message实体中关联/带出 发送消息的人，然后再带出发送消息的人的头像/照片
            .Include(u => u.Recipient).ThenInclude(p => p.Photos)
            .Where( m => m.RecipientId == userId 
                       && m.RecipientDeleted == false 
                       && m.SenderId == recipientId 
                       || m.RecipientId == recipientId 
                       && m.SenderDeleted == false
                       && m.SenderId == userId)
                       .OrderByDescending(m => m.MessageSent)
                       .ToListAsync(); // 返回两个用户之间的对话  这里的逻辑有点绕

                       return messages;

        }
    }
}