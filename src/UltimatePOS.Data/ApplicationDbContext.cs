using Microsoft.EntityFrameworkCore;
using UltimatePOS.Core.Entities;

namespace UltimatePOS.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Business and Location
    public DbSet<Business> Businesses { get; set; } = null!;
    public DbSet<Location> Locations { get; set; } = null!;

    // User Management
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Role> Roles { get; set; } = null!;

    // Products
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<ProductVariant> ProductVariants { get; set; } = null!;
    public DbSet<Brand> Brands { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<SubCategory> SubCategories { get; set; } = null!;
    public DbSet<Unit> Units { get; set; } = null!;
    public DbSet<SellingPriceGroup> SellingPriceGroups { get; set; } = null!;

    // Inventory
    public DbSet<ProductStock> ProductStocks { get; set; } = null!;
    public DbSet<StockHistory> StockHistories { get; set; } = null!;

    // Contacts
    public DbSet<Contact> Contacts { get; set; } = null!;
    public DbSet<ContactGroup> ContactGroups { get; set; } = null!;
    public DbSet<PaymentTerm> PaymentTerms { get; set; } = null!;

    // Sales
    public DbSet<Sale> Sales { get; set; } = null!;
    public DbSet<SaleItem> SaleItems { get; set; } = null!;

    // Purchases
    public DbSet<PurchaseOrder> PurchaseOrders { get; set; } = null!;
    public DbSet<PurchaseOrderItem> PurchaseOrderItems { get; set; } = null!;

    // Payments
    public DbSet<Payment> Payments { get; set; } = null!;
    public DbSet<PaymentAccount> PaymentAccounts { get; set; } = null!;

    // Tax and Expenses
    public DbSet<TaxRate> TaxRates { get; set; } = null!;
    public DbSet<Expense> Expenses { get; set; } = null!;
    public DbSet<ExpenseCategory> ExpenseCategories { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure global query filters for soft delete
        modelBuilder.Entity<BaseEntity>().HasQueryFilter(e => !e.IsDeleted);

        // Configure indexes
        ConfigureIndexes(modelBuilder);

        // Configure relationships
        ConfigureRelationships(modelBuilder);

        // Seed default data
        SeedData(modelBuilder);
    }

    private void ConfigureIndexes(ModelBuilder modelBuilder)
    {
        // Business and Location
        modelBuilder.Entity<Business>()
            .HasIndex(b => b.Name);

        modelBuilder.Entity<Location>()
            .HasIndex(l => new { l.BusinessId, l.Name });

        // User
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => new { u.BusinessId, u.Email });

        // Product
        modelBuilder.Entity<Product>()
            .HasIndex(p => p.SKU);

        modelBuilder.Entity<Product>()
            .HasIndex(p => p.Barcode);

        modelBuilder.Entity<Product>()
            .HasIndex(p => new { p.BusinessId, p.Name });

        // Stock
        modelBuilder.Entity<ProductStock>()
            .HasIndex(ps => new { ps.ProductId, ps.LocationId });

        // Contact
        modelBuilder.Entity<Contact>()
            .HasIndex(c => new { c.BusinessId, c.Name });

        modelBuilder.Entity<Contact>()
            .HasIndex(c => c.Phone);

        // Sale
        modelBuilder.Entity<Sale>()
            .HasIndex(s => s.InvoiceNumber)
            .IsUnique();

        modelBuilder.Entity<Sale>()
            .HasIndex(s => new { s.BusinessId, s.InvoiceDate });

        // Purchase Order
        modelBuilder.Entity<PurchaseOrder>()
            .HasIndex(po => po.OrderNumber)
            .IsUnique();

        modelBuilder.Entity<PurchaseOrder>()
            .HasIndex(po => new { po.BusinessId, po.OrderDate });
    }

    private void ConfigureRelationships(ModelBuilder modelBuilder)
    {
        // Configure Sale-Payment relationship
        modelBuilder.Entity<Sale>()
            .HasMany(s => s.Payments)
            .WithOne(p => p.Sale)
            .HasForeignKey(p => p.SaleId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure PurchaseOrder-Payment relationship
        modelBuilder.Entity<PurchaseOrder>()
            .HasMany(po => po.Payments)
            .WithOne(p => p.PurchaseOrder)
            .HasForeignKey(p => p.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure cascade deletes
        modelBuilder.Entity<Business>()
            .HasMany(b => b.Locations)
            .WithOne(l => l.Business)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Product>()
            .HasMany(p => p.Variants)
            .WithOne(v => v.Product)
            .OnDelete(DeleteBehavior.Cascade);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Seed default roles
        modelBuilder.Entity<Role>().HasData(
            new Role
            {
                Id = 1,
                Name = "Admin",
                Description = "Full system access",
                Permissions = "[\"*\"]",
                IsSystemRole = true,
                CreatedDate = DateTime.UtcNow
            },
            new Role
            {
                Id = 2,
                Name = "Cashier",
                Description = "POS and sales only",
                Permissions = "[\"pos.view\",\"pos.create\",\"sales.view\",\"sales.create\",\"products.view\"]",
                IsSystemRole = true,
                CreatedDate = DateTime.UtcNow
            },
            new Role
            {
                Id = 3,
                Name = "Warehouse Manager",
                Description = "Stock and inventory management",
                Permissions = "[\"products.view\",\"products.create\",\"products.edit\",\"stock.view\",\"stock.create\",\"stock.edit\",\"purchases.view\",\"purchases.create\"]",
                IsSystemRole = true,
                CreatedDate = DateTime.UtcNow
            }
        );

        // Seed default units
        modelBuilder.Entity<Unit>().HasData(
            new Unit { Id = 1, Name = "Piece", ShortName = "pc", CreatedDate = DateTime.UtcNow },
            new Unit { Id = 2, Name = "Kilogram", ShortName = "kg", CreatedDate = DateTime.UtcNow },
            new Unit { Id = 3, Name = "Liter", ShortName = "L", CreatedDate = DateTime.UtcNow },
            new Unit { Id = 4, Name = "Meter", ShortName = "m", CreatedDate = DateTime.UtcNow },
            new Unit { Id = 5, Name = "Box", ShortName = "box", CreatedDate = DateTime.UtcNow }
        );

        // Seed default payment terms
        modelBuilder.Entity<PaymentTerm>().HasData(
            new PaymentTerm { Id = 1, Name = "Cash on Delivery", DaysUntilDue = 0, CreatedDate = DateTime.UtcNow },
            new PaymentTerm { Id = 2, Name = "Net 15", DaysUntilDue = 15, CreatedDate = DateTime.UtcNow },
            new PaymentTerm { Id = 3, Name = "Net 30", DaysUntilDue = 30, CreatedDate = DateTime.UtcNow },
            new PaymentTerm { Id = 4, Name = "Net 60", DaysUntilDue = 60, CreatedDate = DateTime.UtcNow }
        );

        // Seed Default Business
        modelBuilder.Entity<Business>().HasData(
            new Business
            {
                Id = 1,
                Name = "Default Business",
                Address = "123 Business St",
                Phone = "555-0123",
                Email = "admin@ultimatepos.com",
                Currency = "USD",
                CreatedDate = DateTime.UtcNow
            }
        );

        // Seed Admin User
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                BusinessId = 1,
                Username = "admin",
                PasswordHash = "$2a$11$e8GqCK4yhtza7xBs90ufr.qbqNRxIVjmSb7Q9zfs2RswJvDpmMVl6", // 123456
                FullName = "System Administrator",
                Email = "admin@ultimatepos.com",
                RoleId = 1, // Admin Role
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            }
        );
    }
}
