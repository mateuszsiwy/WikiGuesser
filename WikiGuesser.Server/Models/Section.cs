namespace WikiGuesser.Server.Models
{
    public class Section
    {
        public int Id { get; set; }
        public int CityId { get; set; }
        public City? City { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }
}
