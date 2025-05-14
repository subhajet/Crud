using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CountryStateJwtAuthenticationWebApi.Authentication;
using CountryStateJwtAuthenticationWebApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CountryStateJwtAuthenticationWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;

        public AuthenticateController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _context = context;
        }
        [HttpGet("countries")]
        public IActionResult GetCountries()
        {
            var countries = _context.Countries
                .Select(c => new { c.CountryId, c.CountryName })
                .ToList();

            return Ok(countries);
        }

        // GET: api/location/states/{countryName}
        [HttpGet("states/{countryName}")]
        public IActionResult GetStates(string countryName)
        {
            var states = _context.States
                .Where(s => s.CountryName == countryName)
                .Select(s => new { s.StateId, s.StateName })
                .ToList();

            return Ok(states);
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModelAspSubhajit model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

                var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddHours(3),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }

            return Unauthorized();
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationASpSubhajit model)
        {
            var userExists = await _userManager.FindByNameAsync(model.Username);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ResponseAspSubhajit { Status = "Error", Message = "User Already Exists!" });

            ApplicationUser user = new ApplicationUser()
            {
                UserName = model.Username,
                Email = model.Email,
                CountryName = model.CountryName,
                StateName = model.StateName,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ResponseAspSubhajit { Status = "Error", Message = "User creation failed! Please check user details and try again." });

            // Add Country if not exists
            var countryExists = _context.Countries.FirstOrDefault(c => c.CountryName == model.CountryName);
            if (countryExists == null)
            {
                var country = new CountryAspSubhajit
                {
                    CountryName = model.CountryName
                };
                _context.Countries.Add(country);
            }

            // Add State if not exists
            var stateExists = _context.States.FirstOrDefault(s => s.StateName == model.StateName && s.CountryName == model.CountryName);
            if (stateExists == null)
            {
                var state = new SatateAspSubhajit
                {
                    StateName = model.StateName,
                    CountryName = model.CountryName
                };
                _context.States.Add(state);
            }

            await _context.SaveChangesAsync();

            return Ok(new ResponseAspSubhajit { Status = "Success", Message = "User created successfully!" });
        }

        [HttpPost]
        [Route("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegistrationASpSubhajit model)
        {
            var userExists = await _userManager.FindByNameAsync(model.Username);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ResponseAspSubhajit { Status = "Error", Message = "User already exists!" });

            ApplicationUser user = new ApplicationUser()
            {
                UserName = model.Username,
                Email = model.Email,
                CountryName = model.CountryName,
                StateName = model.StateName,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ResponseAspSubhajit { Status = "Error", Message = "User creation failed! Please check user details and try again." });

            // Add Country if not exists
            var countryExists = _context.Countries.FirstOrDefault(c => c.CountryName == model.CountryName);
            if (countryExists == null)
            {
                var country = new CountryAspSubhajit
                {
                    CountryName = model.CountryName
                };
                _context.Countries.Add(country);
            }

            // Add State if not exists
            var stateExists = _context.States.FirstOrDefault(s => s.StateName == model.StateName && s.CountryName == model.CountryName);
            if (stateExists == null)
            {
                var state = new SatateAspSubhajit
                {
                    StateName = model.StateName,
                    CountryName = model.CountryName
                };
                _context.States.Add(state);
            }

            await _context.SaveChangesAsync();

            if (!await _roleManager.RoleExistsAsync(UserRoleAspSubhajit.Admin))
                await _roleManager.CreateAsync(new IdentityRole(UserRoleAspSubhajit.Admin));
            if (!await _roleManager.RoleExistsAsync(UserRoleAspSubhajit.User))
                await _roleManager.CreateAsync(new IdentityRole(UserRoleAspSubhajit.User));

            if (await _roleManager.RoleExistsAsync(UserRoleAspSubhajit.Admin))
            {
                await _userManager.AddToRoleAsync(user, UserRoleAspSubhajit.Admin);
            }

            return Ok(new ResponseAspSubhajit { Status = "Success", Message = "Admin user created successfully!" });
        }
    }
}
