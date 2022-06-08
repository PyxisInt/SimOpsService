using Microsoft.EntityFrameworkCore;
using SimOps.Models.Authentication;

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Add fluent modeling for database here...including seed data if any
        }
    }
}