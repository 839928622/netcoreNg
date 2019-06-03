using System.Threading.Tasks;
using DatingApp.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using DatingApp.API.Models;
using DatingApp.API.Dtos;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using System;
using System.IdentityModel.Tokens.Jwt;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace DatingApp.API.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        // public readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AuthController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
             IConfiguration config, IMapper mapper)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _mapper = mapper;

            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {

            // validate request
            // userForRegisterDto.Username = userForRegisterDto.Username.ToLower();
            // if (await _repo.UserExists(userForRegisterDto.Username))
            //     return BadRequest("用户名已经存在");

            var userToCreate = _mapper.Map<User>(userForRegisterDto);
            var result = await _userManager.CreateAsync(userToCreate,userForRegisterDto.Password);

            var userToReturn = _mapper.Map<UserForDetailDto>(userToCreate);

            if (result.Succeeded)
            {
               return CreatedAtRoute("GetUser",
               new {controller = "Users",id = userToCreate.Id},userToReturn);
            }

            return BadRequest(result.Errors);
            // new User
            // {
            //     Username = userForRegisterDto.Username

            // };

            // var createUser = await _repo.Register(userToCreate, userForRegisterDto.Password);

            // var userToReturn = _mapper.Map<UserForDetailDto>(createUser);
            // return CreatedAtRoute("GetUser", new { Controller = "Users", id = createUser.Id }, userToReturn);
            // StatusCode(201);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {

            // var userFromRepo = await _repo.Login(userForLoginDto.Username.ToLower(), userForLoginDto.Password);
            // if (userFromRepo == null) // 确认：我们在数据库里有是否有此人
            //     return Unauthorized();

            var user = await _userManager.FindByNameAsync(userForLoginDto.Username);
        
            var result = await _signInManager.CheckPasswordSignInAsync(user,userForLoginDto.Password, false);

            if (result.Succeeded)
            {
                var appUser = await _userManager.Users.Include( p=> p.Photos)
                .FirstOrDefaultAsync(u => u.NormalizedUserName == userForLoginDto.Username.ToUpper());
            
            var userToReturn = _mapper.Map<UserForListDto>(appUser); // 返回给前端的用户个人信息，包括照片

            return Ok(new
            {
                token = GenerateJwtToken(appUser).Result,// 传给客户端
                user = userToReturn // 返回一个匿名对象/类 包含两个成员 
            });

            }

            return Unauthorized();
        }

        private async Task<string> GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
             { // 我们的token包含两部分 1） 用户的Id ；2) 用户名
               new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
               new Claim(ClaimTypes.Name,user.UserName)
            };
         
          var roles = await _userManager.GetRolesAsync(user); // 一个用户可能有多个角色 
          foreach (var role in roles)
          {
              claims.Add(new Claim(ClaimTypes.Role,role));
          }

            //为了确保 用户发来的token是合法的，因此服务器对token作一个签名。 服务器配置文件里配置一个安全的key，
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            // 然后使用这个key作为 SigningCredentials的一部分，并使用SecurityAlgorithms.HmacSha512Signature来加密它
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescripter = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims), // 用户身份信息
                Expires = DateTime.Now.AddDays(7),// 过期时间
                SigningCredentials = creds // 凭证
            };

            var tokenHandler = new JwtSecurityTokenHandler();// 使用tokenHandler 可以创建token把tokenDescripter传进去
            var token = tokenHandler.CreateToken(tokenDescripter);// 这个是颁发给用户的token 
            return tokenHandler.WriteToken(token);

        }


    }
}