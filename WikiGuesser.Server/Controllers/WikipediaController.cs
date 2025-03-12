using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using WikiGuesser.Server.Interfaces.Services;
using WikiGuesser.Server.Models;

namespace WikiGuesser.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WikipediaController : Controller
    {
        private readonly IWikipediaService _wikipediaService;
        
        public WikipediaController(IWikipediaService wikipediaService)
        {
            _wikipediaService = wikipediaService;
        }

        [HttpGet("article/{title}")]
        public async Task<IActionResult> GetArticle(string title)
        {
            try
            {
                var article = await _wikipediaService.GetArticle(title);
                return Ok(article);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error fetching article: {ex.Message}");
            }
        }

        [HttpGet("cities")]
        public async Task<IActionResult> GetCities()
        {
            try
            {
                var city = await _wikipediaService.GetRandomCity();
                return Ok(city);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error fetching random city: {ex.Message}");
            }
        }

        [HttpGet("cities/{country}")]
        public async Task<IActionResult> GetRandomCitiesFromCountry(string country)
        {
            try
            {
                var cities = await _wikipediaService.GetRandomCitiesFromCountry(country);
                return Ok(cities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error fetching cities: {ex.Message}");
            }
        }

        [HttpGet("location/{city}")]
        public async Task<IActionResult> GetLocation(string city)
        {
            try
            {
                var location = await _wikipediaService.GetLocation(city);
                return Ok(location);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error fetching location: {ex.Message}");
            }
        }

        [HttpGet("citydesc/{id}")]
        public async Task<IActionResult> GetDesc(string id)
        {
            try
            {
                var desc = await _wikipediaService.GetCityDescription(id);
                return Ok(desc);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error fetching description: {ex.Message}");
            }
        }

        [HttpGet("citydesc/{city}/weather")]
        public async Task<IActionResult> GetWeather(string city)
        {
            try
            {
                var weather = await _wikipediaService.GetWeather(city);
                return Ok(weather);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error fetching weather: {ex.Message}");
            }
        }
        [HttpGet("randomArticle")]
        public async Task<IActionResult> GetRandomArticle()
        {
            try
            {
                var article = await _wikipediaService.GetRandomArticleAsync();
                return Ok(article);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error fetching random article: {ex.Message}");
            }
        }
        [HttpGet("daily")]
        public async Task<IActionResult> GetDailyCity()
        {
            try
            {
                var city = await _wikipediaService.GetDailyCity();
                return Ok(city);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error fetching daily city: {ex.Message}");
            }
        }
        
        [HttpGet("daily/{city}/hint/{hintLevel}")]
        public async Task<IActionResult> GetDailyHint(string city, int hintLevel)
        {
            try
            {
                var dailyData = await _wikipediaService.GetDailyCityData(city);
                var hint = GetHintForLevel(dailyData, hintLevel);
                return Ok(hint);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error fetching hint: {ex.Message}");
            }
        }
        
        private object GetHintForLevel(DailyCityData data, int hintLevel)
        {
            switch (hintLevel)
            {
                case 1:
                    return new { Type = "Weather", Content = data.Weather };
                case 2:
                    return new { Type = "Population", Content = data.Population };
                case 3:
                    return new { Type = "Landmarks", Content = data.Landmarks };
                case 4:
                    return new { Type = "History", Content = data.History };
                case 5:
                    return new { Type = "ImageClue", Content = data.Photos.FirstOrDefault() };
                default:
                    return new { Type = "NoMoreHints", Content = "No more hints available" };
            }
        }
    }
}