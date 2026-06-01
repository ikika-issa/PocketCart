using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PocketCartApp.Domain.Domain_Models;
using PocketCartApp.Domain.Identity_Models;

namespace PocketCartApp.Repository;

public class ApplicationDbContext : IdentityDbContext<PocketCartApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<PocketCartApplicationUser>()
            .HasOne(u => u.ShoppingCart)
            .WithOne(c => c.Cashier)
            .HasForeignKey<ShoppingCart>(c => c.CashierOnShift);
    }

    public virtual DbSet<Product> Products { get; set;}
    public virtual DbSet<Receipt> Receipts { get; set; }
    public virtual DbSet<ShoppingCart> ShoppingCarts { get; set; }
    public virtual DbSet<ProductInShoppingCart> ProductsInShoppingCarts { get; set; }
    public virtual DbSet<Category> Category { get; set; }
    public virtual DbSet<Manufacturer> Manufacturers { get; set; }
    public virtual DbSet<ProductInReceipt> ProductsInReceipts { get; set; }
}
