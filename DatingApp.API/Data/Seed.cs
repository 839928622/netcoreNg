using System.Collections.Generic;
using DatingApp.API.Models;
using Newtonsoft.Json;

namespace DatingApp.API.Data
{
    public class Seed
    {
        private readonly DataContext _context;
        public Seed(DataContext context)
        {
            _context = context;

        }

        public void SeedUsers() 
        {
         var userData = System.IO.File.ReadAllText("Data/UserSeedData.json"); //读取数据出来后反序列化成对象object
        var Users = JsonConvert.DeserializeObject<List<User>>(userData);
        // 接下来循环遍历
        foreach (var user in Users)
        {
            byte[] passwordHash,passwordSalt;
            CreatePasswordHash("password",out passwordHash,out passwordSalt); // 其实这里可以使用AuthRepository的方法 把其设置为public或者static，但是这里是开发者模式，为了方便而做的，我们就copy过来
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.Username = user.Username.ToLower();

            _context.Users.Add(user);
        }
        
        _context.SaveChanges(); // 这里不用async的原因是该操作只执行一次
        // 下一步，去到startup告知应用关于新增十个用户这件事 
        }

         private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using(var hmac=new System.Security.Cryptography.HMACSHA512())
            {
            passwordSalt=hmac.Key;
            passwordHash=hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

            }
        }
    }
}