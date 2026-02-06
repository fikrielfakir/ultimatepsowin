using System;
using System.Threading.Tasks;
using UltimatePOS.Core.Interfaces;

namespace UltimatePOS.Services;

/// <summary>
/// Base class for all business logic services
/// </summary>
public abstract class ServiceBase
{
    protected readonly IUnitOfWork _unitOfWork;

    protected ServiceBase(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    /// <summary>
    /// Execute an operation within a transaction
    /// </summary>
    protected async Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> operation)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();
            var result = await operation();
            await _unitOfWork.CommitTransactionAsync();
            return result;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
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
            await _unitOfWork.BeginTransactionAsync();
            await operation();
            await _unitOfWork.CommitTransactionAsync();
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}
