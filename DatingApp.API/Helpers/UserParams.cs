namespace DatingApp.API.Helpers
{
    public class UserParams
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

    public string Gender { get; set; }

    public int MinAge { get; set; } = 18 ;
    public int MaxAge { get; set; } = 99;
    public string OrderBy { get; set; }
    public bool Likees { get; set; } = false; // 默认false
    public bool Likers { get; set; } = false;
    }
}