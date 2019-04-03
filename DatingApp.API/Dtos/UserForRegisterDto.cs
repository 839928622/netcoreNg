using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.Dtos
{
    public class UserForRegisterDto
    {
        [Required]
        public string Username { get; set; }


         [Required]
         [StringLength(15,MinimumLength=6,ErrorMessage="密码长度必须为6-15位数")]
        public string Password { get; set; }
    }
}