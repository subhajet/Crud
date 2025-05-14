using System.ComponentModel.DataAnnotations;

namespace CountryStateJwtAuthenticationWebApi.Models
{
    public class RegistrationASpSubhajit
    {
        
        [Required(ErrorMessage = "User Name is required")]
        public string Username { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
        [Required]
        public string CountryName { get; set; }

        [Required]
        public string StateName { get; set; }
    }
}
