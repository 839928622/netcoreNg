using System.Collections.Generic;
using System.Threading.Tasks;
using DatingApp.API.Helpers;
using DatingApp.API.Models;

namespace DatingApp.API.Data
{
    public interface IDatingRepository
    {
         void Add<T>(T entity) where T: class; // 泛型方法 Add，类型为T，这里可能是User或者Photo，使用实体entity作为参数，最后约束一下这个方法：仅支持class
         void Delete<T>(T entity) where T:class;

         Task<bool> SaveAll(); //保存所有
         Task<PageList<User>> GetUsers(UserParams userParams); //获取分页的用户
         Task<User> GetUser(int id); //获取单个用户
         Task<Photo> GetPhoto(int id);
         Task<Photo> GetMainPhotoForUser(int userId);
         Task<Like> GetLike (int userId ,int recipientId); //这两个都是用户的Id
         Task<Message> GetMessage(int id);//id是消息id
         Task<PageList<Message>> GetMessagesForUser(MessageParams messageParams); // 这里将是 收件箱 发件箱 未读消息 依赖的方法
         Task<IEnumerable<Message>> GetMessageThread(int userId ,int recipientId); //这里是消息发送者id和接收者recipientId之间的通信


}
}