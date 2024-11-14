using Microsoft.AspNetCore.Mvc;

namespace WikiGuesser.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WikipediaController : Controller
    {
        private readonly HttpClient _httpClient;

        public WikipediaController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Get: api/Wikipedia/GetRandomArticle
        [HttpGet("article/{title}")]
        public async Task<IActionResult> GetArticle(string title)
        {
            // Budowanie URL zapytania do API Wikipedii
            var url = $"https://en.wikipedia.org/w/api.php?action=query&prop=revisions&rvslots=main&format=json&titles={title}&rvprop=content";

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
            // Budowanie URL zapytania do API Wikipedii
            var url = "https://en.wikipedia.org/w/api.php?action=query&list=categorymembers&cmpageid=34234467&cmlimit=500&format=json";

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

        [HttpGet("articleCity")]
        public async Task<IActionResult> GetCityArticle()
        {
            // Budowanie URL zapytania do API Wikipedii
            var url = "https://en.wikipedia.org/w/api.php?action=query&titles=Gwangmyeong&prop=extracts&explaintext=true&format=json";


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

    }
}
