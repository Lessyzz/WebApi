using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApi.Models;

namespace WebApi.Data;

public class EfContext(DbContextOptions<EfContext> options) : IdentityDbContext<IdentityUser>(options)
{
    public DbSet<JwtToken> JwtTokens { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<BasketProduct> BasketProducts { get; set; }
    public DbSet<PaidProduct> PaidProducts { get; set; }
    public DbSet<PromotionCode> PromotionCodes { get; set; }
    public DbSet<Review> Reviews { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Product>()
            .HasOne<User>(p => p.ProductSeller)
            .WithMany(u => u.Products);
        
        base.OnModelCreating(builder);
    }
}