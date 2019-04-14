using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
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
        public async Task<IActionResult> GetUsers()
        {
            var users = await _repo.GetUsers();
            
            return Ok(_mapper.Map<IEnumerable<UserForListDto>>(users));
        }

        [HttpGet("{id}",Name = "GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _repo.GetUser(id);
            
            return Ok(_mapper.Map<UserForDetailDto>(user));

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser (int id, UserForUpdateDto userForUpdateDto)
        {
          if (id !=int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) // User.FindFirst(ClaimTypes.NameIdentifier用来判断传递来的这个token中的id与浏览器中参数的id是否一致，如果不一致，说明用户数据可能被篡改，所以返回401
          return Unauthorized();

          var userFromRepo = await _repo.GetUser(id);

          _mapper.Map(userForUpdateDto,userFromRepo);
          if (await _repo.SaveAll())
                return NoContent();

                throw new Exception($"id为{id}的用户资料更新失败");
        }

    }
}