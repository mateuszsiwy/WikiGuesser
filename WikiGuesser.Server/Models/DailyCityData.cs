namespace WikiGuesser.Server.Models;

public class DailyCityData
{
    public string CityName { get; set; }
    public string Description { get; set; }
    public Location Location { get; set; }
    public WeatherData Weather { get; set; }
    public string Population { get; set; }
    public List<string> Landmarks { get; set; }
    public string History { get; set; }
    public string Economy { get; set; }
    public List<string> Photos { get; set; }
    public DateTime Date { get; set; }
    public int RevealedHints { get; set; } = 0;
}