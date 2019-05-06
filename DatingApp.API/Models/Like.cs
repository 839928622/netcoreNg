namespace DatingApp.API.Models
{
    public class Like
    {
        public int LikerId { get; set; } // 用户的Id 关注者的id
        public int LikeeId { get; set; } // 用户的Id 被关注者的id
        public User Liker { get; set; }
        public User Likee { get; set; }

        // 当自己点了关注某个人，那么对方的id就是LikeeId, 自己的Id就是LikerId
    }
}