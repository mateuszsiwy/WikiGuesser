using Microsoft.EntityFrameworkCore;
using WikiGuesser.Server.Interfaces.Repositories;
using WikiGuesser.Server.Models;

namespace WikiGuesser.Server.Repositories;

public class WikipediaRepository : IWikipediaRepository
{
    private readonly WikiGuesserDbContext _context;

    public WikipediaRepository(WikiGuesserDbContext context)
    {
        _context = context;
    }
    
    public async Task<bool> IsCityTableEmpty()
    {
        return await _context.Cities.CountAsync() == 0;
    }
    
    public async Task AddCountryAsync(Country country)
    {
        await _context.Countries.AddAsync(country);
        await _context.SaveChangesAsync();
    }

    public async Task AddCityAsync(City city)
    {
        await _context.Cities.AddAsync(city);
        await _context.SaveChangesAsync();
    }

    public async Task<string> GetRandomCityAsync()
    {
        Random random = new Random();
        var citiesList = await _context.Cities.Select(x => x.Name).ToListAsync();
        var randomCity = citiesList.OrderBy(x => random.Next()).FirstOrDefault();
        return randomCity;
    }
    
    public async Task<CachedArticle> GetRandomCachedArticleAsync()
    {
        var count = await _context.CachedArticles.CountAsync();
        if (count == 0)
            return null;
            
        var random = new Random();
        var skip = random.Next(0, count);
        
        return await _context.CachedArticles.Skip(skip).FirstOrDefaultAsync();
    }
    
    public async Task AddArticleToCacheAsync(CachedArticle article)
    {
        article.CachedAt = DateTime.UtcNow;
        _context.CachedArticles.Add(article);
        await _context.SaveChangesAsync();
    }
    
    public async Task<bool> IsCacheEmpty()
    {
        return !await _context.CachedArticles.AnyAsync();
    }
    
    public async Task<List<City>> GetAllCitiesAsync()
    {
        return await _context.Cities.ToListAsync();
    }
}