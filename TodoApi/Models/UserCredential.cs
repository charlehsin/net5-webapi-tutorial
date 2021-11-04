using System.ComponentModel.DataAnnotations;

namespace TodoApi.Models
{
    public class UserCredential
    {
        [Required(ErrorMessage = "User Name is required")]  
        public string UserName { get; set; }  
  
        [Required(ErrorMessage = "Password is required")]  
        public string Password { get; set; }  
    }
}