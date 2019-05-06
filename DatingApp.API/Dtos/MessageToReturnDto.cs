using System;
using DatingApp.API.Models;

namespace DatingApp.API.Dtos
{
    public class MessageToReturnDto
    {
        // 从message类拿一些代码过来
            public int Id { get; set; }
        public int SenderId { get; set; } // 发送者id
        public string  SenderKnownAs { get; set; } // 发送者 在用户类 实体中 有KnownAs,automapper应该可以自动识别
        public string SenderPhotoUrl { get; set; } // 发送者的头像url
        public int RecipientId { get; set; } // 接收者ID
        public string RecipientKnowAs { get; set; }  // 接收者
        public string RecipientPhotoUrl { get; set; } // 接收者的头像Url
        public string Content { get; set; }  // 内容
        public bool  IsRead { get; set; } // 是否已读
        public DateTime? DateRead { get; set; } // 消息被读的时间
        public DateTime MessageSent { get; set; } // 消息发送的时间
        // public bool SenderDeleted { get; set; } // 发送者删除的
        // public bool RecipientDeleted { get; set; } // 接收者删除的
    }
}