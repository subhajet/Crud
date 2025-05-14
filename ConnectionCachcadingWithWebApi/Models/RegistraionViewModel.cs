using System.ComponentModel.DataAnnotations;
using Mono.TextTemplating;
using System.Diagnostics.Metrics;

namespace ConnectionCachcadingWithWebApi.Models
{
    public class RegistraionViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please select a country.")]
        public int CountryId { get; set; }

        [Required(ErrorMessage = "Please select a state.")]
        public int StateId { get; set; }

        // These are filled from the backend; no need for validation
        public string CountryName { get; set; }
        public string StateName { get; set; }

        // Populated from API
        public List<CountryModel> Countries { get; set; } = new();
        public List<StateModel> States { get; set; } = new();

        [Required]
        public string UserRole { get; set; }
    }
}
