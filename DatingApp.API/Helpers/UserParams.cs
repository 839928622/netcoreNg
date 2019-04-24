namespace DatingApp.API.Helpers
{
    public class UserParams
    {
        private const int MaxPageSize = 50 ;
        public int PageNumber { get; set; } = 1 ;
       
        private int pageSize = 10; //初始化的值为10
        public int PageSize
        {
            get { return pageSize ;}
            set { pageSize = (value > MaxPageSize) ? MaxPageSize : value ;}
        }
        
    }
}