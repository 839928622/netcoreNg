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

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0 ;
        }

        
    }
}