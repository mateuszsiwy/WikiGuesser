using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WikiGuesser.Server.Models;
namespace WikiGuesser.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WikipediaController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly WikiGuesserDbContext _context;
        public WikipediaController(HttpClient httpClient, WikiGuesserDbContext context)
        {
            _httpClient = httpClient;
            _context = context;
        }

        [HttpGet("article/{title}")]
        public async Task<IActionResult> GetArticle(string title)
        {
            // Budowanie URL zapytania do API Wikipedii
            var url = $"https://en.wikipedia.org/api/rest_v1/page/html/{title}";

            try
            {
                // Wysyłanie zapytania GET do API Wikipedii
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    // Odczytanie odpowiedzi
                    var content = await response.Content.ReadAsStringAsync();
                    return Ok(content);
                }
                else
                {
                    return NotFound("Artykuł nie został znaleziony.");
                }
            }
            catch
            {
                return StatusCode(500, "Błąd podczas pobierania artykułu z Wikipedii.");
            }
        }

        [HttpGet("cities")]
        public async Task<IActionResult> GetCities()
        {
            List<string> countryCodes = new List<string>
                {
                    "AF", "AL", "DZ", "AD", "AO", "AG", "AR", "AM", "AU", "AT", "AZ",
                    "BS", "BH", "BD", "BB", "BY", "BE", "BZ", "BJ", "BT", "BO", "BA",
                    "BW", "BR", "BN", "BG", "BF", "BI", "CV", "KH", "CM", "CA", "CF",
                    "TD", "CL", "CN", "CO", "KM", "CD", "CR", "HR", "CU", "CY", "CZ",
                    "DK", "DJ", "DM", "DO", "EC", "EG", "SV", "GQ", "ER", "EE", "SZ",
                    "ET", "FJ", "FI", "FR", "GA", "GM", "GE", "DE", "GH", "GR", "GD",
                    "GT", "GN", "GW", "GY", "HT", "HN", "HU", "IS", "IN", "ID", "IR",
                    "IQ", "IE", "IL", "IT", "JM", "JP", "JO", "KZ", "KE", "KI", "KP",
                    "KR", "KW", "KG", "LA", "LV", "LB", "LS", "LR", "LY", "LI", "LT",
                    "LU", "MG", "MW", "MY", "MV", "ML", "MT", "MH", "MR", "MU", "MX",
                    "FM", "MD", "MC", "MN", "ME", "MA", "MZ", "MM", "NA", "NR", "NP",
                    "NL", "NZ", "NI", "NE", "NG", "MK", "NO", "OM", "PK", "PW", "PS",
                    "PA", "PG", "PY", "PE", "PH", "PL", "PT", "QA", "RO", "RU", "RW",
                    "KN", "LC", "VC", "WS", "SM", "ST", "SA", "SN", "RS", "SC", "SL",
                    "SG", "SK", "SI", "SB", "SO", "ZA", "SS", "ES", "LK", "SD", "SR",
                    "SE", "CH", "SY", "TJ", "TZ", "TH", "TL", "TG", "TO", "TT", "TN",
                    "TR", "TM", "TV", "UG", "UA", "AE", "GB", "US", "UY", "UZ", "VU",
                    "VE", "VN", "YE", "ZM", "ZW"
                };
            List<string> cities = new List<string>();

            if (_context.Countries.Count() == 0)
            {
                foreach(var code in countryCodes)
                {
                    cities = await GetRandomCitiesFromCountry(code);
                    if (cities.Count > 0)
                    {
                        var country = new Country { Name = code };
                        _context.Countries.Add(country);
                        await _context.SaveChangesAsync();
                        foreach(var city in cities)
                        {
                            var newCity = new City { Name = city, CountryId = country.Id };
                            _context.Cities.Add(newCity);
                        }
                        await _context.SaveChangesAsync();
                    }
                }   
            }

            /*foreach(var code in countryCodes)
            {
                cities = await GetRandomCitiesFromCountry(code);
                if (cities.Count > 0)
                {
                    return Ok(cities);
                }
            }
            return Ok(cities);*/
            Random random = new Random();
            var citiesList = await _context.Cities.Select(x => x.Name).ToListAsync();
            var randomCity = citiesList.OrderBy(x => random.Next()).FirstOrDefault();
            return Ok(randomCity);
        }

        public async Task<List<string>> GetRandomCitiesFromCountry(string country)
        {
            var url = $"http://api.geonames.org/searchJSON?country={country}&featureClass=P&maxRows=100&username=msiwy";

            try
            {
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    JObject json = JObject.Parse(content); 
                    var cities = json["geonames"].Select(x => x["name"].ToString()).ToList();

                    Random random = new Random();
                    return cities.OrderBy(x => random.Next()).Take(10).ToList();
                }
                else
                {
                    return new List<string>();
                }
            }
            catch
            {
                return new List<string>();
            }
        }


    }
}
