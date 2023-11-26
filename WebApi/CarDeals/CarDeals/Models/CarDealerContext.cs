using Microsoft.EntityFrameworkCore;

namespace CarDeals.Models
{
    public class CarDealerContext(DbContextOptions<CarDealerContext> options) : DbContext(options)
    {
        public DbSet<SignedInUser> SignedInUsers { get; set; }

        public DbSet<Car> Cars { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Dealer> Deals { get; set; }
        public DbSet<DealerCars> DealersCars { get; set; }
    }
}
