using WikiGuesser.Server.Models;

namespace WikiGuesser.Server.Interfaces.Services;

public interface IWikipediaService
{
    Task<WikipediaArticle> GetRandomArticleAsync();
    Task<string> GetArticle(string title);
    Task<string> GetRandomCity();
    Task<List<string>> GetRandomCitiesFromCountry(string country);
    Task<Location> GetLocation(string city);
    Task<string> GetCityDescription(string city);
    Task<WeatherData> GetWeather(string city);
}