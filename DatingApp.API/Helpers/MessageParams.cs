namespace DatingApp.API.Helpers
{
    public class MessageParams
    {
         private const int MaxPageSize = 50 ; //每页显示的条数最大50
        public int PageNumber { get; set; } = 1 ; //用户每次请求的时候，默认显示第一页
       
        private int pageSize = 10; //初始化的值为10
        public int PageSize
        {
            get { return pageSize ;}
            set { pageSize = (value > MaxPageSize) ? MaxPageSize : value ;}
        }

        public int UserId { get; set; }
        public string MessageContainer { get; set; } = "Unread"; // 未读的消息
        
    }
}