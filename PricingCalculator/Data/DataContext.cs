using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PricingCalculator.Models;
using System.Reflection.Emit;

namespace PricingCalculator.Data
{
    public class DataContext(DbContextOptions<DataContext> options) : IdentityDbContext<Models.User, Role, string>(options)
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Customizations for Identity Tables
            builder.Entity<Models.User>().ToTable("Users");
            builder.Entity<Role>().ToTable("Roles");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");

            builder.Entity<Product>()
            .HasIndex(p => p.Sku)
            .IsUnique();
        }
    }
}
