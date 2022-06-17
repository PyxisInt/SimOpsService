using Microsoft.EntityFrameworkCore;
using SimOps.Models.Authentication;
using SimOps.Models.Common;
using SimOps.Models.Fleet;
using SimOps.Models.Navigation;

namespace SimOpsService.Repository
{
    public class SimOpsContext : DbContext
    {
        public SimOpsContext(DbContextOptions options)
            : base(options)
        {

        }
        
        
        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Airport> Airports { get; set; }
        public DbSet<Aircraft> Aircrafts { get; set; }
        public DbSet<EngineCode> EngineCodes { get; set; }
        public DbSet<EquipmentCode> EquipmentCodes { get; set; }
        public DbSet<Hub> Hubs { get; set; }
        public DbSet<Airline> Airlines { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Add fluent modeling for database here...including seed data if any
            modelBuilder.Entity<Aircraft>()
                .HasKey(a => new {a.Airline, a.Registration});
            modelBuilder.Entity<Airport>()
                .HasIndex(a => a.Iata);
        }
    }
}