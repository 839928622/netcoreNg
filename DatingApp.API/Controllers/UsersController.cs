using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Authorize] //由于这是要返回给用户的，因此要保护起来
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        public UsersController(IDatingRepository repo, IMapper mapper) //注入各种依赖
        {
            _mapper = mapper;
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery]UserParams userParams)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var userFromRepo = await _repo.GetUser(currentUserId, true);

            userParams.UserId = currentUserId;

            if (string.IsNullOrEmpty(userParams.Gender))
            {
                userParams.Gender = userFromRepo.Gender == "male" ? "female" : "male" ;
            }
            var users = await _repo.GetUsers(userParams); // pagelist of users
            
            Response.AddPagination(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);
           
            return Ok(_mapper.Map<IEnumerable<UserForListDto>>(users));
        }

        [HttpGet("{id}",Name = "GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            var isCurrentUser = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier).Value
                ) == id; //先获取当前用户的NameIdentifier，在这里的话是int类型的id，然后转成int，最后比较是否等于传进来的id。相等，就是当前用户，不等就不是
            var user = await _repo.GetUser(id, isCurrentUser);
            
            return Ok(_mapper.Map<UserForDetailDto>(user));

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser (int id, UserForUpdateDto userForUpdateDto)
        {
          if (id !=int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) // User.FindFirst(ClaimTypes.NameIdentifier用来判断传递来的这个token中的id与浏览器中参数的id是否一致，如果不一致，说明用户数据可能被篡改，所以返回401
          return Unauthorized();

          var userFromRepo = await _repo.GetUser(id, true);

          _mapper.Map(userForUpdateDto,userFromRepo);
          if (await _repo.SaveAll())
                return NoContent();

                throw new Exception($"id为{id}的用户资料更新失败"); // $符号表示String.Format
        }

        [HttpPost("{id}/like/{recipientId}")]
        public async Task<IActionResult> LikeUser(int id ,int recipientId) // 第二个参数：recipientId是列表中展示的用户的id，第一个参数：id是当前账户所有者的id
        {
            if ( id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var like = await _repo.GetLike(id,recipientId);
            if (like != null)
            return BadRequest("您已经关注过该用户了");

            if(await _repo.GetUser(recipientId, false) == null)
             return NotFound();
             like = new Like // 如果当前账户的所有者没有关注id=recipientId的用户，那么就新增一条记录
             {
               LikerId = id , // 关注者的id
               LikeeId = recipientId // 被关注者的id
             };
             _repo.Add<Like>(like);

             if (await _repo.SaveAll())
             return Ok();

             return BadRequest("关注该用户失败");
        }

    }
}