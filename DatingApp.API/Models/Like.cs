namespace DatingApp.API.Models
{
    public class Like
    {
        public int LikerId { get; set; } // 用户的Id 给别人点赞
        public int LikeeId { get; set; } // 用户的Id 被别人点赞
        public User Liker { get; set; }
        public User Likee { get; set; }
    }
}