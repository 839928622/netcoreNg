using System;

namespace DatingApp.API.Dtos
{
    public class UserForListDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
      
        public string Gender { get; set; }
        public int  Age { get; set; } //这里我们不想返回出生日期，
        public string KnownAs { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
        // public string Introdution { get; set; }
        // public string LookingFor { get; set; }
        // public string Interests { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string PhotoUrl { get; set; }
    }
}