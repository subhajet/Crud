using System.ComponentModel.DataAnnotations;

namespace CountryStateJwtAuthenticationWebApi.Models
{
    public class LoginModelAspSubhajit
    {
        [Required(ErrorMessage = "User Name is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}
