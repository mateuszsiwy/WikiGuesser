using System.Text.Json;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using WikiGuesser.Server.Interfaces.Repositories;
using WikiGuesser.Server.Interfaces.Services;
using WikiGuesser.Server.Models;

namespace WikiGuesser.Server.Services;

public class WikipediaService : IWikipediaService
{
    private readonly HttpClient _httpClient;
    private readonly IWikipediaRepository _wikipediaRepository;
    private readonly ILogger<WikipediaService> _logger;

    public WikipediaService(HttpClient httpClient, IWikipediaRepository wikipediaRepository,
        ILogger<WikipediaService> logger)
    {
        _httpClient = httpClient;
        _wikipediaRepository = wikipediaRepository;
        _logger = logger;
    }
    

    public async Task<string> GetArticle(string title)
    {
        var url = $"https://en.wikipedia.org/api/rest_v1/page/html/{title}";

        try
        {
            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode) return await response.Content.ReadAsStringAsync();

            throw new Exception("Article not found.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching article from Wikipedia");
            throw;
        }
    }

    public async Task<string> GetRandomCity()
    {
        var countryCodes = new List<string>
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
        var cities = new List<string>();

        if (await _wikipediaRepository.IsCityTableEmpty())
            foreach (var code in countryCodes)
            {
                cities = await GetRandomCitiesFromCountry(code);
                if (cities.Count > 0)
                {
                    var country = new Country { Name = code };
                    await _wikipediaRepository.AddCountryAsync(country);

                    foreach (var city in cities)
                    {
                        var newCity = new City { Name = city, CountryId = country.Id };
                        await _wikipediaRepository.AddCityAsync(newCity);
                    }
                }
            }

        return await _wikipediaRepository.GetRandomCityAsync();
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
                var json = JObject.Parse(content);
                var cities = json["geonames"].Select(x => x["name"].ToString()).ToList();

                var random = new Random();
                return cities.OrderBy(x => random.Next()).Take(10).ToList();
            }

            return new List<string>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching random cities from country");
            return new List<string>();
        }
    }

    public async Task<Location> GetLocation(string city)
    {
        var url = $"http://api.geonames.org/searchJSON?q={city}&maxRows=1&username=msiwy";

        try
        {
            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(content);
                var locationData = json["geonames"].FirstOrDefault();

                if (locationData == null) throw new Exception("Location not found");

                return new Location
                {
                    Latitude = locationData["lat"].ToString(),
                    Longitude = locationData["lng"].ToString(),
                    CountryName = locationData["countryName"].ToString()
                };
            }

            throw new Exception("Location not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching location data");
            throw;
        }
    }

    public async Task<string> GetCityDescription(string city)
    {
        var url = $"https://en.wikipedia.org/api/rest_v1/page/summary/{city}";

        try
        {
            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(content);
                return json["extract_html"].ToString();
            }

            return "Strona nie została znaleziona.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching city description");
            throw;
        }
    }

    public async Task<WeatherData> GetWeather(string city)
    {
        var url = $"http://api.weatherapi.com/v1/current.json?key=214f2be23ac3438b9c7151949240512&q={city}&aqi=no";

        try
        {
            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var jsonObject = JObject.Parse(content);
                var weatherData = new WeatherData
                {
                    Current = new Current
                    {
                        TempC = jsonObject["current"]["temp_c"].ToString()
                    },
                    Location = new WeatherLocation
                    {
                        LocalTime = jsonObject["location"]["localtime"].ToString()
                    }
                };
                
                return weatherData;
            }

            throw new Exception($"Weather data not found. Status code: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching weather data");
            throw;
        }
    }

    public async Task<WikipediaArticle> GetRandomArticleAsync()
    {
        try
        {
            var cachedArticle = await _wikipediaRepository.GetRandomCachedArticleAsync();

            if (cachedArticle != null)
            {
                var weatherData = await GetWeather(cachedArticle.ArticleName);

                return new WikipediaArticle
                {
                    ArticleName = cachedArticle.ArticleName,
                    Summary = cachedArticle.Description,
                    Location = new Location
                    {
                        Latitude = cachedArticle.Latitude,
                        Longitude = cachedArticle.Longitude,
                        CountryName = cachedArticle.CountryName
                    },
                    Weather = weatherData.Current.TempC,
                    Timezone = weatherData.Location.LocalTime
                };
            }
            await PopulateCacheAsync();
            return await FetchAndCacheArticleSingle();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting random article data");
            throw;
        }
    }
    
    private async Task<WikipediaArticle> FetchAndCacheArticleSingle()
    {
        var cityName = await GetRandomCity();
        var cityDesc = await GetCityDescription(cityName);

        while (cityDesc.Contains("may refer to") ||
               cityDesc.Contains("usually refers to") ||
               cityDesc == "Strona nie została znaleziona." ||
               cityDesc.Contains("may mean"))
        {
            cityName = await GetRandomCity();
            cityDesc = await GetCityDescription(cityName);
        }

        var location = await GetLocation(cityName);
        var weatherData = await GetWeather(cityName);

        if (location?.CountryName != null) 
            cityDesc = cityDesc.Replace(location.CountryName, "COUNTRY");

        await _wikipediaRepository.AddArticleToCacheAsync(new CachedArticle
        {
            ArticleName = cityName,
            Description = cityDesc,
            Latitude = location.Latitude,
            Longitude = location.Longitude,
            CountryName = location.CountryName
        });

        return new WikipediaArticle
        {
            ArticleName = cityName,
            Summary = cityDesc,
            Location = location,
            Weather = weatherData.Current.TempC,
            Timezone = weatherData.Location.LocalTime
        };
    }

    public async Task PopulateCacheAsync(int count = 500)
    {
        if (await _wikipediaRepository.IsCacheEmpty())
        {
            for (int i = 0; i < count; i++)
            {
                try
                {
                    await FetchAndCacheArticleSingle();
                    _logger.LogInformation($"Cached article {i+1}/{count}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to cache article {i+1}/{count}");
                }
            }
        }
    }
    
    public async Task<DailyCityData> GetDailyCityData(string city)
    {
        try {
            var location = await GetLocation(city);
            var description = await GetCityDescription(city);
            var weatherData = await GetWeather(city);
            
            var population = await GetPopulationData(city);
            var landmarks = await GetCityLandmarks(city);
            var history = Regex.Replace(await GetCityDescription(city), city, "COUNTRY", RegexOptions.IgnoreCase);
            var economy = await GetCityEconomy(city);
            var photos = await GetCityPhotos(city);
            
            return new DailyCityData {
                
                CityName = city,
                Description = description,
                Location = location,
                Weather = weatherData,
                Population = population,
                Landmarks = landmarks,
                History = history,
                Economy = economy,
                Photos = photos,
                Date = DateTime.UtcNow.Date
            };
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error fetching daily city data for {CityName}", city);
            throw;
        }
    }
    
    private async Task<string> GetPopulationData(string city)
    {
        var url = $"http://api.geonames.org/searchJSON?q={city}&maxRows=1&username=msiwy&style=full";
        var response = await _httpClient.GetAsync(url);
        if (response.IsSuccessStatusCode) {
            var content = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(content);
            return json["geonames"]?.FirstOrDefault()?["population"]?.ToString() ?? "Unknown";
        }
        return "Population data unavailable";
    }
    
    private async Task<List<string>> GetCityLandmarks(string city)
    {
        var url = $"https://en.wikipedia.org/w/api.php?action=query&prop=links&titles={city}&pllimit=50&format=json";
        var response = await _httpClient.GetAsync(url);
        
        if (response.IsSuccessStatusCode) {
            var content = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(content);
            var page = json["query"]["pages"].First.First;
            
            var landmarks = new List<string>();
            if (page["links"] != null) {
                foreach (var link in page["links"]) {
                    var title = link["title"].ToString();
                    if (title.Contains("Museum") || title.Contains("Palace") || 
                        title.Contains("Cathedral") || title.Contains("Monument")) {
                        landmarks.Add(title);
                    }
                }
            }
            return landmarks.Take(5).ToList();
        }
        return new List<string>() { "No landmarks found" };
    }
    
    private async Task<string> GetCityHistory(string city)
    {
        var url = $"https://en.wikipedia.org/w/api.php?action=parse&page={city}&prop=text&section=1&format=json";
        var response = await _httpClient.GetAsync(url);
        
        if (response.IsSuccessStatusCode) {
            var content = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(content);
            if (json["parse"] != null && json["parse"]["text"] != null) {
                var htmlContent = json["parse"]["text"]["*"].ToString();
                return htmlContent;
            }
        }
        return "Historical information unavailable";
    }
    
    private async Task<string> GetCityEconomy(string city)
    {
        var url = $"https://en.wikipedia.org/w/api.php?action=parse&page={city}&prop=text&section=2&format=json";
        var response = await _httpClient.GetAsync(url);
        
        if (response.IsSuccessStatusCode) {
            var content = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(content);
            if (json["parse"] != null && json["parse"]["text"] != null) {
                return json["parse"]["text"]["*"].ToString();
            }
        }
        return "Economic information unavailable";
    }
    
    private async Task<List<string>> GetCityPhotos(string city)
    {
        var url = $"https://commons.wikimedia.org/w/api.php?action=query&list=search&srsearch={city} city&srnamespace=6&format=json";
        var response = await _httpClient.GetAsync(url);
        
        if (response.IsSuccessStatusCode) {
            var content = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(content);
            var photos = new List<string>();
            
            if (json["query"] != null && json["query"]["search"] != null) {
                foreach (var item in json["query"]["search"]) {
                    photos.Add(item["title"].ToString().Replace("File:", ""));
                    if (photos.Count >= 3) break;
                }
            }
            return photos;
        }
        return new List<string>();
    }
    
    private static readonly List<string> LargeCities = new List<string>
    {
        // North America
        "New York", "Los Angeles", "Chicago", "Houston", "Phoenix", "Philadelphia", "San Antonio", "San Diego", 
        "Dallas", "Toronto", "Montreal", "Vancouver", "Mexico City", "Guadalajara", "Monterrey",
        
        // South America
        "São Paulo", "Rio de Janeiro", "Brasília", "Salvador", "Buenos Aires", "Lima", "Bogotá", "Santiago", 
        "Caracas", "Quito", "Medellín", "Cali", "Montevideo", "La Paz", "Asunción",
        
        // Europe
        "London", "Berlin", "Madrid", "Rome", "Paris", "Vienna", "Amsterdam", "Barcelona", "Munich", "Milan",
        "Warsaw", "Prague", "Brussels", "Dublin", "Lisbon", "Stockholm", "Copenhagen", "Helsinki", "Oslo",
        "Athens", "Budapest", "Bucharest", "Sofia", "Belgrade", "Zagreb", "Kyiv", "Minsk", "Riga", "Vilnius",
        
        // Africa
        "Cairo", "Lagos", "Kinshasa", "Johannesburg", "Algiers", "Casablanca", "Nairobi", "Khartoum", "Addis Ababa",
        "Dar es Salaam", "Cape Town", "Tunis", "Rabat", "Accra", "Dakar", "Luanda", "Abidjan", "Maputo",
        
        // Asia
        "Tokyo", "Delhi", "Shanghai", "Mumbai", "Beijing", "Dhaka", "Istanbul", "Karachi", "Guangzhou", "Seoul",
        "Jakarta", "Bangkok", "Ho Chi Minh City", "Taipei", "Hong Kong", "Singapore", "Kuala Lumpur", "Manila",
        "Bangalore", "Chennai", "Kolkata", "Osaka", "Shenzhen", "Chengdu", "Lahore", "Hyderabad", "Riyadh",
        "Dubai", "Abu Dhabi", "Jeddah", "Tehran", "Baghdad", "Almaty", "Tashkent", "Hanoi", "Yangon", "Phnom Penh",
        
        // Oceania
        "Sydney", "Melbourne", "Brisbane", "Perth", "Auckland", "Wellington", "Christchurch", "Adelaide",
        
        // Additional Major Cities
        "Moscow", "Bangalore", "Ahmedabad", "Chongqing", "Wuhan", "Tianjin", "Xi'an", "Chengdu",
        "Nanjing", "Dongguan", "Hangzhou", "Shenyang", "Busan", "Incheon", "Fukuoka", "Sapporo", "Kyoto",
        "Casablanca", "Alexandria", "Ankara", "Izmir", "Damascus", "Kabul", "Kathmandu", "Colombo", "Pune",
        "Surat", "Jaipur", "Lucknow", "Kanpur", "Nagpur", "Indore", "Thane", "Bhopal", "Visakhapatnam",
        "Brussels", "Manchester", "Birmingham", "Glasgow", "Liverpool", "Lyon", "Marseille", "Lille", "Naples",
        "Turin", "Valencia", "Seville", "Zaragoza", "Frankfurt", "Düsseldorf", "Stuttgart", "Portland", "Denver",
        "Boston", "Detroit", "Seattle", "Minneapolis", "Austin", "San Jose", "Jacksonville", "Charlotte",
        "San Francisco", "Indianapolis", "Columbus", "Fort Worth", "Washington", "Nashville", "Memphis",
        "Baltimore", "Louisville", "Milwaukee", "Albuquerque", "Tucson", "Fresno", "Sacramento",
        "Guadalajara", "Monterrey", "Puebla", "Toluca", "León", "Tijuana", "Montreal", "Calgary", "Edmonton",
        "Ottawa", "Winnipeg", "Quebec City", "Hamilton", "Havana", "Osaka", "Nagoya", "Sapporo", "Kobe",
        "Kyoto", "Fukuoka", "Kawasaki", "Saitama", "Hiroshima", "Yekaterinburg", "Novosibirsk", "Samara"
    };
    
    public async Task<string> GetDailyCity()
    {
        var today = DateTime.UtcNow.Date - TimeSpan.FromDays(1);
        var seed = today.Year * 10000 + today.Month * 100 + today.Day;
        var random = new Random(seed);
        
        return LargeCities[random.Next(LargeCities.Count)];
    }
    
    
}