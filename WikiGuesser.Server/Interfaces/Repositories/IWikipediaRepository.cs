using WikiGuesser.Server.Models;

namespace WikiGuesser.Server.Interfaces.Repositories;

public interface IWikipediaRepository
{
    Task<bool> IsCityTableEmpty();
    Task AddCountryAsync(Country country);
    Task AddCityAsync(City city);
    Task<string> GetRandomCityAsync();
    Task<CachedArticle> GetRandomCachedArticleAsync();
    Task AddArticleToCacheAsync(CachedArticle article);
    Task<bool> IsCacheEmpty();
    Task<List<City>> GetAllCitiesAsync();
}