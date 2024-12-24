using Evarosa.Models;
using Microsoft.EntityFrameworkCore;

namespace Evarosa.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Admin> Admins { get; set; }
    public DbSet<Article> Articles { get; set; }
    public DbSet<ArticleCategory> ArticleCategories { get; set; }
    public DbSet<ConfigSite> ConfigSite { get; set; }
    public DbSet<Contact> Contacts { get; set; }
    public DbSet<Banner> Banners { get; set; }
    public DbSet<Member> Members { get; set; }
    public DbSet<ProductCategory> ProductCategories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<CartItem> ShoppingCartItems { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }
    public DbSet<City> Cities { get; set; }
    public DbSet<District> Districts { get; set; }
    public DbSet<Ward> Wards { get; set; }
    public DbSet<MemberAddress> MemberAddresses { get; set; }
    public DbSet<Sku> Sku { get; set; }
    public DbSet<Option> Options { get; set; }
    public DbSet<OptionSku> OptionSkus { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>()
            .HasOne(o => o.City)
            .WithMany()
            .HasForeignKey(o => o.CityId)
            .OnDelete(DeleteBehavior.NoAction);
        modelBuilder.Entity<Order>()
            .HasOne(o => o.District)
            .WithMany()
            .HasForeignKey(o => o.DistrictId)
            .OnDelete(DeleteBehavior.NoAction);
        modelBuilder.Entity<Order>()
            .HasOne(o => o.Ward)
            .WithMany()
            .HasForeignKey(o => o.WardId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<MemberAddress>()
            .HasOne(o => o.City)
            .WithMany()
            .HasForeignKey(o => o.CityId)
            .OnDelete(DeleteBehavior.NoAction);
        modelBuilder.Entity<MemberAddress>()
            .HasOne(o => o.District)
            .WithMany()
            .HasForeignKey(o => o.DistrictId)
            .OnDelete(DeleteBehavior.NoAction);
        modelBuilder.Entity<MemberAddress>()
            .HasOne(o => o.Ward)
            .WithMany()
            .HasForeignKey(o => o.WardId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Member>(entity =>
        {
            entity.HasIndex(p => p.Email).IsUnique();
        });
        modelBuilder.Entity<ProductCategory>(entity =>
        {
            entity.HasIndex(p => p.Url).IsUnique();
        });
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasIndex(p => p.Url).IsUnique();
        });
        modelBuilder.Entity<ArticleCategory>(entity =>
        {
            entity.HasIndex(p => p.Url).IsUnique();
        });
        modelBuilder.Entity<Article>(entity =>
        {
            entity.HasIndex(p => p.Url).IsUnique();
        });

        modelBuilder.Entity<OptionSku>()
            .HasKey(o => new { o.SkuId, o.OptionId });
    }
}