using System.Collections.Generic;
using System.Threading.Tasks;
using DatingApp.API.Models;

namespace DatingApp.API.Data
{
    public interface IDatingRepository
    {
         void Add<T>(T entity) where T: class; // 泛型方法 Add，类型为T，这里可能是User或者Photo，使用实体entity作为参数，最后约束一下这个方法：仅支持class
         void Delete<T>(T entity) where T:class;

         Task<bool> SaveAll(); //保存所有
         Task<IEnumerable<User>> GetUsers(); //获取所有用户
         Task<User> GetUser(int id); //获取单个用户
    }
}