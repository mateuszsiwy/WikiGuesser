namespace WikiGuesser.Server.DTOs;

public class WikipediaArticleDTO
{
    public string ArticleName { get; set; }
    public string Summary { get; set; }
    public LocationDTO Location { get; set; }
    public double Weather { get; set; }
    public string Timezone { get; set; }
    public List<PlayerDTO> Players { get; set; }
}

