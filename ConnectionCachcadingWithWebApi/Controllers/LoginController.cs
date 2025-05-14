using ConnectionCachcadingWithWebApi.Models;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ConnectionCachcadingWithWebApi.Controllers
{
    public class LoginController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public LoginController(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Login()
        {
            ViewBag.Msg = "";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var client = _httpClientFactory.CreateClient();

            client.BaseAddress = new Uri(_configuration["ApiSettings:BaseUrl"]);

            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("api/Authenticate/Login", content);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var jwtToken = JsonConvert.DeserializeObject<JwtTokenResponse>(responseString);

                HttpContext.Session.SetString("JWToken", jwtToken.token);

                return RedirectToAction("Dashboard");
            }
            else
            {
                ViewBag.Msg = "Invalid Login Attempt";
                return View();
            }
        }

        public class JwtTokenResponse
        {
            public string token { get; set; }
            public DateTime expiration { get; set; }
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            ViewBag.Msg = "";
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["ApiSettings:BaseUrl"]);

            var countries = new List<CountryModel>();
            var countryResponse = await client.GetAsync("api/Authenticate/countries");

            if (countryResponse.IsSuccessStatusCode)
            {
                var data = await countryResponse.Content.ReadAsStringAsync();
                countries = JsonConvert.DeserializeObject<List<CountryModel>>(data);
            }

            var model = new RegistraionViewModel
            {
                Countries = countries,
                States = new List<StateModel>()
            };

            return View(model);
        }

        //[HttpPost]
        //public async Task<IActionResult> Register(RegistraionViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
        //        ViewBag.Msg = string.Join("; ", errors);
        //        await PopulateCountriesAndStates(model);
        //        return View(model);
        //    }

        //    var client = _httpClientFactory.CreateClient();
        //    client.BaseAddress = new Uri(_configuration["ApiSettings:BaseUrl"]);

        //    // 🔍 Step 2a: Get CountryName by ID
        //    var countriesResponse = await client.GetAsync("api/Authenticate/countries");
        //    var countries = new List<CountryModel>();
        //    if (countriesResponse.IsSuccessStatusCode)
        //    {
        //        var data = await countriesResponse.Content.ReadAsStringAsync();
        //        countries = JsonConvert.DeserializeObject<List<CountryModel>>(data);
        //    }
        //    model.CountryName = countries.FirstOrDefault(c => c.CountryId == model.CountryId)?.CountryName;

        //    // 🔍 Step 2b: Get StateName by CountryId and StateId
        //    var statesResponse = await client.GetAsync($"api/location/states/{model.CountryId}");
        //    var states = new List<StateModel>();
        //    if (statesResponse.IsSuccessStatusCode)
        //    {
        //        var data = await statesResponse.Content.ReadAsStringAsync();
        //        states = JsonConvert.DeserializeObject<List<StateModel>>(data);
        //    }
        //    model.StateName = states.FirstOrDefault(s => s.StateId == model.StateId)?.StateName;

        //    // ✅ Build API model to send to Web API
        //    var apiModel = new
        //    {
        //        model.Username,
        //        model.Email,
        //        model.Password,
        //        CountryName = model.CountryName,
        //        StateName = model.StateName
        //    };

        //    var content = new StringContent(JsonConvert.SerializeObject(apiModel), Encoding.UTF8, "application/json");

        //    HttpResponseMessage response;
        //    if (model.UserRole == "User")
        //        response = await client.PostAsync("api/Authenticate/register", content);
        //    else
        //        response = await client.PostAsync("api/Authenticate/register-admin", content);

        //    ViewBag.Msg = response.IsSuccessStatusCode ? "User Registered Successfully" : "Registration Failed";

        //    await PopulateCountriesAndStates(model);
        //    return View(model);
        //}

        [HttpPost]
        public async Task<IActionResult> Register(RegistraionViewModel model)
        {
            // First, manually populate the countries and states in case we need to return to the view
            await PopulateCountriesAndStates(model);

            // Check model state explicitly excluding the CountryName and StateName fields
            // since we'll set them manually
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);

                // Filter out country and state name validation errors since we'll set them ourselves
                var filteredErrors = errors.Where(e =>
                    !e.Contains("CountryName") &&
                    !e.Contains("StateName"));

                if (filteredErrors.Any())
                {
                    ViewBag.Msg = string.Join("; ", filteredErrors);
                    return View(model);
                }
            }

            // Clear any existing errors for CountryName and StateName
            ModelState.Remove("CountryName");
            ModelState.Remove("StateName");

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["ApiSettings:BaseUrl"]);

            // Step 1: Get Countries
            var countriesResponse = await client.GetAsync("api/Authenticate/countries");
            var countries = new List<CountryModel>();
            if (countriesResponse.IsSuccessStatusCode)
            {
                var data = await countriesResponse.Content.ReadAsStringAsync();
                countries = JsonConvert.DeserializeObject<List<CountryModel>>(data);
            }
            else
            {
                ViewBag.Msg = "Failed to retrieve countries from API.";
                return View(model);
            }

            var selectedCountry = countries.FirstOrDefault(c => c.CountryId == model.CountryId);
            if (selectedCountry == null)
            {
                ViewBag.Msg = "Invalid country selected.";
                return View(model);
            }

            // Set the CountryName here
            model.CountryName = selectedCountry.CountryName;

            // Step 2: Get States
            var statesResponse = await client.GetAsync($"api/Authenticate/states/{selectedCountry.CountryName}");
            var states = new List<StateModel>();
            if (statesResponse.IsSuccessStatusCode)
            {
                var data = await statesResponse.Content.ReadAsStringAsync();
                states = JsonConvert.DeserializeObject<List<StateModel>>(data);
            }
            else
            {
                ViewBag.Msg = "Failed to retrieve states from API.";
                return View(model);
            }

            var selectedState = states.FirstOrDefault(s => s.StateId == model.StateId);
            if (selectedState == null)
            {
                ViewBag.Msg = "Invalid state selected.";
                return View(model);
            }

            // Set the StateName here
            model.StateName = selectedState.StateName;

            // Prepare data to send to API
            var apiModel = new
            {
                username = model.Username,
                email = model.Email,
                password = model.Password,
                countryName = model.CountryName,
                stateName = model.StateName
            };

            // Log the request data for debugging
            Console.WriteLine($"Sending registration request: {JsonConvert.SerializeObject(apiModel)}");

            var content = new StringContent(JsonConvert.SerializeObject(apiModel), Encoding.UTF8, "application/json");

            // Step 4: Send the Request to the API
            HttpResponseMessage response;
            try
            {
                string endpoint = model.UserRole == "User"
                    ? "api/Authenticate/register"
                    : "api/Authenticate/register-admin";

                // Add headers if needed
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                // Log the full request details for debugging
                Console.WriteLine($"Sending request to: {client.BaseAddress}{endpoint}");
                Console.WriteLine($"Request body: {await content.ReadAsStringAsync()}");

                response = await client.PostAsync(endpoint, content);

                if (response.IsSuccessStatusCode)
                {
                    ViewBag.Msg = "User Registered Successfully!";
                    // Clear the form by returning a new instance
                    return View(new RegistraionViewModel
                    {
                        Countries = model.Countries,
                        States = new List<StateModel>()
                    });
                }
                else
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    // Log the complete error for debugging
                    Console.WriteLine($"API Error: {responseContent}");

                    // Deserialize to get more detailed error info if available
                    try
                    {
                        var errorResponse = JsonConvert.DeserializeObject<dynamic>(responseContent);
                        string detailedMessage = errorResponse?.message?.ToString() ?? "Unknown error";
                        ViewBag.Msg = $"Registration failed: {detailedMessage}";
                    }
                    catch
                    {
                        // If we can't deserialize, show the raw response
                        ViewBag.Msg = $"Registration failed: {responseContent}";
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Msg = $"An error occurred: {ex.Message}";
            }

            // Return to View with error message
            return View(model);
        }




        private async Task PopulateCountriesAndStates(RegistraionViewModel model)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["ApiSettings:BaseUrl"]);

            // Get all countries
            var countriesResponse = await client.GetAsync("api/Authenticate/countries");
            if (countriesResponse.IsSuccessStatusCode)
            {
                var data = await countriesResponse.Content.ReadAsStringAsync();
                model.Countries = JsonConvert.DeserializeObject<List<CountryModel>>(data);
            }
            else
            {
                model.Countries = new List<CountryModel>();
            }

            // Get states only if a country is selected
            if (model.CountryId > 0)
            {
                var selectedCountry = model.Countries.FirstOrDefault(c => c.CountryId == model.CountryId);
                if (selectedCountry != null)
                {
                    var statesResponse = await client.GetAsync($"api/Authenticate/states/{selectedCountry.CountryName}");
                    if (statesResponse.IsSuccessStatusCode)
                    {
                        var data = await statesResponse.Content.ReadAsStringAsync();
                        model.States = JsonConvert.DeserializeObject<List<StateModel>>(data);
                    }
                    else
                    {
                        model.States = new List<StateModel>();
                    }
                }
            }
            else
            {
                model.States = new List<StateModel>();
            }
        }

        //private async Task PopulateCountriesAndStates(RegistraionViewModel model)
        //{
        //    var client = _httpClientFactory.CreateClient();
        //    client.BaseAddress = new Uri(_configuration["ApiSettings:BaseUrl"]);

        //    var countriesResponse = await client.GetAsync("api/Authenticate/countries");
        //    if (countriesResponse.IsSuccessStatusCode)
        //    {
        //        var data = await countriesResponse.Content.ReadAsStringAsync();
        //        model.Countries = JsonConvert.DeserializeObject<List<CountryModel>>(data);
        //    }

        //    if (model.CountryId > 0)
        //    {
        //        var statesResponse = await client.GetAsync($"api/Authenticate/states/{model.CountryId}");
        //        if (statesResponse.IsSuccessStatusCode)
        //        {
        //            var data = await statesResponse.Content.ReadAsStringAsync();
        //            model.States = JsonConvert.DeserializeObject<List<StateModel>>(data);
        //        }
        //    }
        //    else
        //    {
        //        model.States = new List<StateModel>();
        //    }
        //}
        [HttpGet]
        public async Task<IActionResult> GetStatesByCountry(int countryId)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["ApiSettings:BaseUrl"]);

            // Step 1: Get country name by ID
            var countriesResponse = await client.GetAsync("api/Authenticate/countries");
            if (!countriesResponse.IsSuccessStatusCode)
            {
                return Json(new List<object>());
            }

            var countriesJson = await countriesResponse.Content.ReadAsStringAsync();
            var countries = JsonConvert.DeserializeObject<List<CountryModel>>(countriesJson);
            var selectedCountry = countries.FirstOrDefault(c => c.CountryId == countryId);

            if (selectedCountry == null)
            {
                return Json(new List<object>());
            }

            // Step 2: Get states for the selected country using the country name
            var statesResponse = await client.GetAsync($"api/Authenticate/states/{selectedCountry.CountryName}");
            if (!statesResponse.IsSuccessStatusCode)
            {
                return Json(new List<object>());
            }

            var statesJson = await statesResponse.Content.ReadAsStringAsync();
            return Content(statesJson, "application/json");
        }
    }
}
