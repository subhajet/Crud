using System.ComponentModel.DataAnnotations;

namespace ConnectionCachcadingWithWebApi.Models
{
    public class StateModel
    {
        public int StateId { get; set; }

        public string StateName { get; set; }

        public string CountryName { get; set; }
    }
}
