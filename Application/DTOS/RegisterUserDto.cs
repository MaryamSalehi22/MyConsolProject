using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOS
{
    public class RegisterUserDto
    {
        [Required(ErrorMessage = "نام کاربری الزامی است.")]
        [StringLength(20, MinimumLength = 3)]
        public string Username { get; set; }

        [Required(ErrorMessage = "رمز عبور الزامی است.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
