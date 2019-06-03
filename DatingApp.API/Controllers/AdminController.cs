using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly UserManager<User> _userManager;
        public AdminController(DataContext context, UserManager<User> userManager)
        {
            _userManager = userManager;
            _context = context;

        }
        [Authorize(Policy = "RequireAdmin")]
        [HttpGet("usersWithRoles")]
        public async Task<IActionResult> GetUsersWithRoles()
        {
            var userList = await (from user in _context.Users
                                  orderby user.UserName
                                  select new
                                  {
                                      Id = user.Id,
                                      UserName = user.UserName,
                                      Roles = (from userRole in user.UserRoles
                                               join role in _context.Roles
                                                on userRole.RoleId equals role.Id
                                               select role.Name
                                                 ).ToList()
                                  }
            ).ToListAsync();
            return Ok(userList);
        }

        [HttpPost("editRoles/{userName}")]
        public async Task<IActionResult> EditRoles(string userName, RoleEditDto roleEditDto)
        {
            var user = await _userManager.FindByNameAsync(userName); // 通过名称获取用户实体

            var userRoles = await _userManager.GetRolesAsync(user); // 把用户的角色带出来 Ilist<string>

            var selectedRoles = roleEditDto.RoleNames;

// 两个??表示 selected = selectedRoles != null ? selectedRoles : new string[] {}
            selectedRoles = selectedRoles ?? new string[] {};

    var result = await _userManager.AddToRolesAsync(user,selectedRoles.Except(userRoles)); // 添加角色，除了原有的角色

    if (!result.Succeeded)
    {
        return BadRequest("添加角色失败");
    }

result = await _userManager.RemoveFromRolesAsync(user,userRoles.Except(selectedRoles)); // 删除角色，除了管理员提交来的角色,其他全部删除

if (!result.Succeeded)
return BadRequest("删除角色失败");

return Ok(await _userManager.GetRolesAsync(user)); // 最终返回用户的角色

            // if ()


        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpGet("photosForModeration")]
        public IActionResult GetPhotoForModerration()
        {
            return Ok("Admins or moderatores can see this");
        }
    }
}