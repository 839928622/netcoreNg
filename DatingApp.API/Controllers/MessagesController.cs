using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace DatingApp.API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Authorize] //由于这是要返回给用户的，因此要保护起来
    [Route("api/users/{userId}/[controller]")] //{userId} 表示这是一个变量
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        public MessagesController(IDatingRepository repo, IMapper mapper)
        {
           _mapper = mapper;
            _repo = repo;

        }

        [HttpGet("{id}",Name = "GetMessage")]
        public async Task<IActionResult> GetMessage(int userId, int id)
        {
          if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized();

            var messageFromRepo = await _repo.GetMessage(id);
           
         if(messageFromRepo == null)
            return NotFound();

            return Ok(messageFromRepo);

           

        }

        [HttpGet]
        public async Task<IActionResult> GetMessagesForUser(int userId, [FromQuery]MessageParams messageParams) // FromQuery 这些 [FromXXX] 是告诉模型绑定在解析的过程中从HttpContext中那一部分获取信息。
        // http://www.iaspnetcore.com/Blog/BlogPost/594960eb84cd453380655bc9
        {
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized();

            messageParams.UserId = userId ; // 通过验证之后，证明传进来的userId是当前访问控制器用户中token中的userid

            var messageFromRepo = await _repo.GetMessagesForUser(messageParams);

            var messages = _mapper.Map<IEnumerable<MessageToReturnDto>>(messageFromRepo); // 把查询到的消息转换为消息dto

            Response.AddPagination( // 在返回的消息头中含有一下消息
                messageFromRepo.CurrentPage 
                , messageFromRepo.PageSize
                , messageFromRepo.TotalCount
                , messageFromRepo.TotalPages);

            return Ok(messages); // 返回的是多条消息
        }

        [HttpGet("thread/{recipientId}")]
        public async Task<IActionResult> GetMessageThread(int userId, int recipientId)
        {
             if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized();

            var messageFromRepo = await _repo.GetMessageThread(userId,recipientId);

            var messageThread = _mapper.Map<IEnumerable<MessageToReturnDto>>(messageFromRepo);

            return Ok(messageThread);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(int userId, MessageForCreationDto messageForCreationDto)
       {
           if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized();

            messageForCreationDto.SenderId = userId ;
            var recipient = await _repo.GetUser(messageForCreationDto.RecipientId); //查询接收方是否存在

            if (recipient == null )
               return BadRequest("对不起，找不到消息的接收方！"); // 不存在则返回 400
      
           var message = _mapper.Map<Message>(messageForCreationDto); // 把消息dto映射到消息实体
           _repo.Add(message); // 这里没有指定实体，由efcore去判断，该消息写入哪一个实体
           
            var messageToReturn = _mapper.Map<MessageForCreationDto>(message); // 把返回的数据重新封装，不然会返回用户的所有信息
           if (await _repo.SaveAll())
           
               return CreatedAtRoute("GetMessage", new {
                   id = message.Id
               },messageToReturn);

               throw new Exception("您创建的消息没有被系统保存"); // Exception 必须using system
           
       } 
    }
}