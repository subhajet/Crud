using System.ComponentModel.DataAnnotations;

namespace CountryStateJwtAuthenticationWebApi.Models
{
    public class CountryAspSubhajit
    {
        [Key]
        public int CountryId { get; set; }

        [Required]
        public string CountryName { get; set; }
    }
}
