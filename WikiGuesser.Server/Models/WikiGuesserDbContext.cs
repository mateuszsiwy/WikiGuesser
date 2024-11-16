
using Microsoft.EntityFrameworkCore;
namespace WikiGuesser.Server.Models
{
    public class WikiGuesserDbContext : DbContext
    {
        public WikiGuesserDbContext(DbContextOptions<WikiGuesserDbContext> options) : base(options)
        {
        }

        public DbSet<City> Cities { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Section> Sections { get; set; }
    }
    
}
