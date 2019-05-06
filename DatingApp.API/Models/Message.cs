using System;

namespace DatingApp.API.Models
{
    public class Message
    {
        public int Id { get; set; }
        public int SenderId { get; set; } // 发送者id
        public User Sender { get; set; } // 发送者
        public int RecipientId { get; set; } // 接收者ID
        public User Recipient { get; set; }  // 接收者
        public string Content { get; set; }  // 内容
        public bool  IsRead { get; set; } // 是否已读
        public DateTime? DateRead { get; set; } // 消息被读的时间
        public DateTime MessageSent { get; set; } // 消息发送的时间
        public bool SenderDeleted { get; set; } // 发送者删除的
        public bool RecipientDeleted { get; set; } // 接收者删除的

    }
}