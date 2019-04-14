using System;
using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.Dtos
{
    public class UserForRegisterDto
    {
        [Required]
        public string Username { get; set; }


         [Required]
         [StringLength(15,MinimumLength=6,ErrorMessage="密码长度必须为9-25位数")]
        public string Password { get; set; }
         [Required]
        public string Gender { get; set; }
         [Required]
        public string KnownAs { get; set; }
         [Required]
        public DateTime DateOfBirth { get; set; }
         [Required]
        public string City { get; set; }
         [Required]
        public string Country { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }

        public UserForRegisterDto()
        {
            Created=DateTime.Now;
            LastActive=DateTime.Now;
        }

    }
}