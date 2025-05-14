using Microsoft.AspNetCore.Identity;

namespace CountryStateJwtAuthenticationWebApi.Authentication
{
    public class ApplicationUser:IdentityUser
    {
        public string CountryName { get; set; }
        public string StateName { get; set; }
    }
}
