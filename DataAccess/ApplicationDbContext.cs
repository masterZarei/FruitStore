using FS.Models.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FS.DataAccess
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Slider> Sliders { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<UnitToProduct> UnitToProducts { get; set; }
        public DbSet<Comments> Comments { get; set; }
        public DbSet<Factor> Factors { get; set; }
        public DbSet<FactorDetail> FactorDetails { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<AboutUs> AboutUs { get; set; }
        public DbSet<ContactWays> ContactWays { get; set; }
        public DbSet<CategoryToProduct> CategoryToProducts { get; set; }
        public DbSet<WalletHistory> WalletHistories { get; set; }
        public DbSet<Rating> Ratings { get; set; }
    }
}
