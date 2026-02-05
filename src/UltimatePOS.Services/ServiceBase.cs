using System;
using System.Threading.Tasks;
using UltimatePOS.Core.Interfaces;

namespace UltimatePOS.Services;

/// <summary>
/// Base class for all business logic services
/// </summary>
public abstract class ServiceBase
{
    protected readonly IUnitOfWork UnitOfWork;

    protected ServiceBase(IUnitOfWork unitOfWork)
    {
        UnitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    /// <summary>
    /// Execute an operation within a transaction
    /// </summary>
    protected async Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> operation)
    {
        try
        {
            await UnitOfWork.BeginTransactionAsync();
            var result = await operation();
            await UnitOfWork.CommitTransactionAsync();
            return result;
        }
        catch
        {
            await UnitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    /// <summary>
    /// Execute an operation within a transaction
    /// </summary>
    protected async Task ExecuteInTransactionAsync(Func<Task> operation)
    {
        try
        {
            await UnitOfWork.BeginTransactionAsync();
            await operation();
            await UnitOfWork.CommitTransactionAsync();
        }
        catch
        {
            await UnitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}
