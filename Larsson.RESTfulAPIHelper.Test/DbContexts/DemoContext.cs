using Larsson.RESTfulAPIHelper.Test.Entity;
using Microsoft.EntityFrameworkCore;

namespace Larsson.RESTfulAPIHelper.Test.Infrastructure
{
    public class DemoContext : DbContext
    {
        public DemoContext(DbContextOptions<DemoContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Product> Products { get; set; }
    }
}