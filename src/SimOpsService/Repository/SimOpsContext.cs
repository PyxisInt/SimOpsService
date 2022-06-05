using Microsoft.EntityFrameworkCore;

namespace SimOpsService.Repository
{
    public class SimOpsContext : DbContext
    {
        public SimOpsContext(DbContextOptions options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Add fluent modeling for database here...including seed data if any
        }
    }
}