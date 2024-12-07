
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace WikiGuesser.Server.Models
{
    public class WikiGuesserDbContext : IdentityDbContext<IdentityUser>
    {
        public WikiGuesserDbContext(DbContextOptions<WikiGuesserDbContext> options) : base(options)
        {
        }

        public DbSet<City> Cities { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Section> Sections { get; set; }
    }
    
}
