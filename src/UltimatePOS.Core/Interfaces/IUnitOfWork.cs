using System;
using System.Threading.Tasks;
using UltimatePOS.Core.Entities;

namespace UltimatePOS.Core.Interfaces;

/// <summary>
/// Unit of Work pattern interface
/// </summary>
public interface IUnitOfWork : IDisposable
{
    IRepository<Business> Businesses { get; }
    IRepository<Location> Locations { get; }
    IRepository<User> Users { get; }
    IRepository<Role> Roles { get; }
    IRepository<Product> Products { get; }
    IRepository<ProductVariant> ProductVariants { get; }
    IRepository<Brand> Brands { get; }
    IRepository<Category> Categories { get; }
    IRepository<SubCategory> SubCategories { get; }
    IRepository<Unit> Units { get; }
    IRepository<ProductStock> ProductStocks { get; }
    IRepository<StockHistory> StockHistories { get; }
    IRepository<Contact> Contacts { get; }
    IRepository<ContactGroup> ContactGroups { get; }
    IRepository<PaymentTerm> PaymentTerms { get; }
    IRepository<Sale> Sales { get; }
    IRepository<SaleItem> SaleItems { get; }
    IRepository<PurchaseOrder> PurchaseOrders { get; }
    IRepository<PurchaseOrderItem> PurchaseOrderItems { get; }
    IRepository<Payment> Payments { get; }
    IRepository<PaymentAccount> PaymentAccounts { get; }
    IRepository<TaxRate> TaxRates { get; }
    IRepository<Expense> Expenses { get; }
    IRepository<ExpenseCategory> ExpenseCategories { get; }

    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
