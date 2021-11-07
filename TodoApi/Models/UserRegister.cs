using System.ComponentModel.DataAnnotations;

namespace TodoApi.Models
{
    public class UserRegister
    {
        [Required(ErrorMessage = "User Name is required")]
        [StringLength(100, ErrorMessage = "The {0} value cannot exceed {1} characters. ")]
        public string UserName { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        [StringLength(100, ErrorMessage = "The {0} value cannot exceed {1} characters. ")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, ErrorMessage = "The {0} value cannot exceed {1} characters. ")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Role is required")]
        public UserRole.Roles Role { get; set; }
    }
}