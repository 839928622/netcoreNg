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

namespace DatingApp.API.Controllers 
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public readonly IAuthRepository _repo ;
        private readonly IConfiguration _config;

        public AuthController(IAuthRepository repo,IConfiguration config)
        {
             _repo = repo;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {

            // validate request
       userForRegisterDto.Username=userForRegisterDto.Username.ToLower();
            if(await _repo.UserExists(userForRegisterDto.Username))
                 return BadRequest("用户名已经存在");

               var userToCreate=new User
               {
                    Username=userForRegisterDto.Username

               };

               var createUser=await _repo.Register(userToCreate,userForRegisterDto.Password);

               return StatusCode(201);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto) 
        {
           var userFromRepo=await _repo.Login(userForLoginDto.Username.ToLower(),userForLoginDto.Password);
           if(userFromRepo==null) // 确认：我们在数据库里有是否有此人
                return Unauthorized();

            var claims=new[]
            { // 我们的token包含两部分 1） 用户的Id ；2) 用户名
               new Claim(ClaimTypes.NameIdentifier,userFromRepo.Id.ToString()),
               new Claim(ClaimTypes.Name,userFromRepo.Username)
            };


           //为了确保 用户发来的token是合法的，因此服务器对token作一个签名。 服务器配置文件里配置一个安全的key，
            var key =new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value))  ;

         // 然后使用这个key作为 SigningCredentials的一部分，并使用SecurityAlgorithms.HmacSha512Signature来加密它
            var creds = new SigningCredentials(key,SecurityAlgorithms.HmacSha512Signature)  ;

            var tokenDescripter = new SecurityTokenDescriptor
            {
              Subject = new ClaimsIdentity(claims), // 用户身份信息
              Expires = DateTime.Now.AddDays(30),// 过期时间
              SigningCredentials = creds // 凭证
            };

            var tokenHandler = new JwtSecurityTokenHandler();// 使用tokenHandler 可以创建token把tokenDescripter传进去
            var token=tokenHandler.CreateToken(tokenDescripter);// 这个是颁发给用户的token 
            return Ok(new {
                token=tokenHandler.WriteToken(token) // 传给客户端
            });


        }


    }
}