namespace WikiGuesser.Server.Models;

public class WikipediaArticle
{
    public string ArticleName { get; set; }
    public string Summary { get; set; }
    public string Weather { get; set; }
    public string Timezone { get; set; }
    public Location Location { get; set; }
}