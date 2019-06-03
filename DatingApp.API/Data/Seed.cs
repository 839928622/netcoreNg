using System.Collections.Generic;
using System.Linq;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace DatingApp.API.Data
{
    public class Seed
    {
        // private readonly DataContext _context;
        // public Seed(DataContext context)
        // {
        //     _context = context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;

        // }
        public Seed(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;

        }

        public void SeedUsers()
        {
            if (!_userManager.Users.Any())
            {

                var userData = System.IO.File.ReadAllText("Data/UserSeedData.json"); //读取数据出来后反序列化成对象object
                var Users = JsonConvert.DeserializeObject<List<User>>(userData);

var roles = new List<Role>
{
    new Role{Name = "Member"},
    new Role{Name = "Admin"},
    new Role{Name= "Moderator"},
    new Role{Name = "VIP"}
};

 foreach (var role in roles)
 {
     _roleManager.CreateAsync(role).Wait();// 创建四个角色
 }

                // 接下来循环遍历
                foreach (var user in Users)
                {
                    // byte[] passwordHash, passwordSalt;
                    // CreatePasswordHash("password", out passwordHash, out passwordSalt); // 其实这里可以使用AuthRepository的方法 把其设置为public或者static，但是这里是开发者模式，为了方便而做的，我们就copy过来
                    // user.PasswordHash = passwordHash;
                    // user.PasswordSalt = passwordSalt;
                    // user.UserName = user.UserName.ToLower();

                    // _context.Users.Add(user);
                   user.Photos.SingleOrDefault().IsApproved = true; // 手动添加的时候设置图片为已经通过审核
                    _userManager.CreateAsync(user, "passwordD@1").Wait();
                    _userManager.AddToRoleAsync(user,"Member").Wait(); // 把用户添加至角色
                }

                var adminUser = new User 
                {
                    UserName = "Admin" // 创建一个Admin的用户
                };

                IdentityResult result = _userManager.CreateAsync(adminUser,"passwordD@1").Result; // 手动创建管理员 用户名 adminUser 密码是一样的

                if (result.Succeeded)
                {
                    var admin = _userManager.FindByNameAsync("Admin").Result;
                    _userManager.AddToRolesAsync(admin, new[] {"Admin","Moderator"}).Wait(); // 把Admin这个用户添加到两个角色 Admin和Moderator
                }

                //_context.SaveChanges(); // 这里不用async的原因是该操作只执行一次
                // 下一步，去到startup告知应用关于新增十个用户这件事 
            }
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

            }
        }
    }
}