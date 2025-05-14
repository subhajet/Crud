using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CountryStateJwtAuthenticationWebApi.Models
{
    public class SatateAspSubhajit
    {
        [Key]
        public int StateId { get; set; }

        [Required]
        public string StateName { get; set; }

        [Required]
        public string CountryName { get; set; }

    }
}