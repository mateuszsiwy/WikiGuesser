namespace WikiGuesser.Server.Models;

public class CachedArticle
{
    public int Id { get; set; }
    public string ArticleName { get; set; }
    public string Description { get; set; }
    public string Latitude { get; set; }
    public string Longitude { get; set; }
    public string CountryName { get; set; }
    public DateTime CachedAt { get; set; }
}