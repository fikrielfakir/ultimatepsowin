using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Threading.Tasks;
using UltimatePOS.Core.Entities;
using UltimatePOS.Core.Interfaces;

namespace UltimatePOS.Data.Repositories;

/// <summary>
/// Unit of Work implementation
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;

    // Repository instances
    private IRepository<Business>? _businesses;
    private IRepository<Location>? _locations;
    private IRepository<User>? _users;
    private IRepository<Role>? _roles;
    private IRepository<Product>? _products;
    private IRepository<ProductVariant>? _productVariants;
    private IRepository<Brand>? _brands;
    private IRepository<Category>? _categories;
    private IRepository<SubCategory>? _subCategories;
    private IRepository<Unit>? _units;
    private IRepository<ProductStock>? _productStocks;
    private IRepository<StockHistory>? _stockHistories;
    private IRepository<Contact>? _contacts;
    private IRepository<ContactGroup>? _contactGroups;
    private IRepository<PaymentTerm>? _paymentTerms;
    private IRepository<Sale>? _sales;
    private IRepository<SaleItem>? _saleItems;
    private IRepository<PurchaseOrder>? _purchaseOrders;
    private IRepository<PurchaseOrderItem>? _purchaseOrderItems;
    private IRepository<Payment>? _payments;
    private IRepository<PaymentAccount>? _paymentAccounts;
    private IRepository<TaxRate>? _taxRates;
    private IRepository<Expense>? _expenses;
    private IRepository<ExpenseCategory>? _expenseCategories;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IRepository<Business> Businesses => 
        _businesses ??= new Repository<Business>(_context);

    public IRepository<Location> Locations => 
        _locations ??= new Repository<Location>(_context);

    public IRepository<User> Users => 
        _users ??= new Repository<User>(_context);

    public IRepository<Role> Roles => 
        _roles ??= new Repository<Role>(_context);

    public IRepository<Product> Products => 
        _products ??= new Repository<Product>(_context);

    public IRepository<ProductVariant> ProductVariants => 
        _productVariants ??= new Repository<ProductVariant>(_context);

    public IRepository<Brand> Brands => 
        _brands ??= new Repository<Brand>(_context);

    public IRepository<Category> Categories => 
        _categories ??= new Repository<Category>(_context);

    public IRepository<SubCategory> SubCategories => 
        _subCategories ??= new Repository<SubCategory>(_context);

    public IRepository<Unit> Units => 
        _units ??= new Repository<Unit>(_context);

    public IRepository<ProductStock> ProductStocks => 
        _productStocks ??= new Repository<ProductStock>(_context);

    public IRepository<StockHistory> StockHistories => 
        _stockHistories ??= new Repository<StockHistory>(_context);

    public IRepository<Contact> Contacts => 
        _contacts ??= new Repository<Contact>(_context);

    public IRepository<ContactGroup> ContactGroups => 
        _contactGroups ??= new Repository<ContactGroup>(_context);

    public IRepository<PaymentTerm> PaymentTerms => 
        _paymentTerms ??= new Repository<PaymentTerm>(_context);

    public IRepository<Sale> Sales => 
        _sales ??= new Repository<Sale>(_context);

    public IRepository<SaleItem> SaleItems => 
        _saleItems ??= new Repository<SaleItem>(_context);

    public IRepository<PurchaseOrder> PurchaseOrders => 
        _purchaseOrders ??= new Repository<PurchaseOrder>(_context);

    public IRepository<PurchaseOrderItem> PurchaseOrderItems => 
        _purchaseOrderItems ??= new Repository<PurchaseOrderItem>(_context);

    public IRepository<Payment> Payments => 
        _payments ??= new Repository<Payment>(_context);

    public IRepository<PaymentAccount> PaymentAccounts => 
        _paymentAccounts ??= new Repository<PaymentAccount>(_context);

    public IRepository<TaxRate> TaxRates => 
        _taxRates ??= new Repository<TaxRate>(_context);

    public IRepository<Expense> Expenses => 
        _expenses ??= new Repository<Expense>(_context);

    public IRepository<ExpenseCategory> ExpenseCategories => 
        _expenseCategories ??= new Repository<ExpenseCategory>(_context);

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
