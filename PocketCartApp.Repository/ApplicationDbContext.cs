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
    public virtual DbSet<Product> Products { get; set; }
    public virtual DbSet<Receipt> Receipts { get; set; }
    public virtual DbSet<ShoppingCart> ShoppingCarts { get; set; }
    public virtual DbSet<ProductInShoppingCart> ProductsInShoppingCarts { get; set; }
    public virtual DbSet<Category> Category { get; set; }
}
