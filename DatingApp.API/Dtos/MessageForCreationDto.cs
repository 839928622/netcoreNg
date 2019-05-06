using System;

namespace DatingApp.API.Dtos
{
    public class MessageForCreationDto
    {
        public int SenderId {get;set;}
        public int RecipientId { get; set; }
        public DateTime MessageSent { get; set; } // 消息发送的时间
        public string Content { get; set; } // 消息的内容

        public MessageForCreationDto()
        {
            MessageSent = DateTime.Now ; // 初始化时间
        }
    }
}