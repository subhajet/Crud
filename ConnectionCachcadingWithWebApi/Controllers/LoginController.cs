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
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                ViewBag.Msg = string.Join("; ", errors);
                await PopulateCountriesAndStates(model);
                return View(model);
            }

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["ApiSettings:BaseUrl"]);

            // Get countries
            var countriesResponse = await client.GetAsync("api/Authenticate/countries");
            var countries = new List<CountryModel>();
            if (countriesResponse.IsSuccessStatusCode)
            {
                var data = await countriesResponse.Content.ReadAsStringAsync();
                countries = JsonConvert.DeserializeObject<List<CountryModel>>(data);
            }

            var selectedCountry = countries.FirstOrDefault(c => c.CountryId == model.CountryId);
            model.CountryName = selectedCountry?.CountryName;

            // Get states by country name
            var statesResponse = await client.GetAsync($"api/Authenticate/states/{model.CountryName}");
            var states = new List<StateModel>();
            if (statesResponse.IsSuccessStatusCode)
            {
                var data = await statesResponse.Content.ReadAsStringAsync();
                states = JsonConvert.DeserializeObject<List<StateModel>>(data);
            }
            model.StateName = states.FirstOrDefault(s => s.StateId == model.StateId)?.StateName;

            var apiModel = new
            {
                model.Username,
                model.Email,
                model.Password,
                CountryName = model.CountryName,
                StateName = model.StateName,
                model.UserRole
            };

            var content = new StringContent(JsonConvert.SerializeObject(apiModel), Encoding.UTF8, "application/json");

            HttpResponseMessage response;
            if (model.UserRole == "User")
                response = await client.PostAsync("api/Authenticate/register", content);
            else
                response = await client.PostAsync("api/Authenticate/register-admin", content);

            ViewBag.Msg = response.IsSuccessStatusCode ? "User Registered Successfully" : "Registration Failed";

            await PopulateCountriesAndStates(model);
            return View(model);
        }

        private async Task PopulateCountriesAndStates(RegistraionViewModel model)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["ApiSettings:BaseUrl"]);

            var countriesResponse = await client.GetAsync("api/Authenticate/countries");
            if (countriesResponse.IsSuccessStatusCode)
            {
                var data = await countriesResponse.Content.ReadAsStringAsync();
                model.Countries = JsonConvert.DeserializeObject<List<CountryModel>>(data);
            }

            if (model.CountryId > 0)
            {
                var statesResponse = await client.GetAsync($"api/Authenticate/states/{model.CountryId}");
                if (statesResponse.IsSuccessStatusCode)
                {
                    var data = await statesResponse.Content.ReadAsStringAsync();
                    model.States = JsonConvert.DeserializeObject<List<StateModel>>(data);
                }
            }
            else
            {
                model.States = new List<StateModel>();
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetStatesByCountry(int countryId)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["ApiSettings:BaseUrl"]);

            // Get country name by ID
            var countriesResponse = await client.GetAsync("api/Authenticate/countries");
            if (!countriesResponse.IsSuccessStatusCode)
                return Json(new List<object>());

            var countriesJson = await countriesResponse.Content.ReadAsStringAsync();
            var countries = JsonConvert.DeserializeObject<List<CountryModel>>(countriesJson);
            var selectedCountry = countries.FirstOrDefault(c => c.CountryId == countryId);

            if (selectedCountry == null)
                return Json(new List<object>());

            var statesResponse = await client.GetAsync($"api/Authenticate/states/{selectedCountry.CountryName}");
            if (!statesResponse.IsSuccessStatusCode)
                return Json(new List<object>());

            var statesJson = await statesResponse.Content.ReadAsStringAsync();
            return Content(statesJson, "application/json");
        }

        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
