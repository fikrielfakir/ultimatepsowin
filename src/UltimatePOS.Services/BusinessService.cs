using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UltimatePOS.Core.Entities;
using UltimatePOS.Core.Interfaces;

namespace UltimatePOS.Services;

public class BusinessService : IBusinessService
{
    private readonly IRepository<Business> _businessRepository;
    private readonly IUnitOfWork _unitOfWork;

    public BusinessService(IRepository<Business> businessRepository, IUnitOfWork unitOfWork)
    {
        _businessRepository = businessRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Business>> GetAllBusinessesAsync()
    {
        return await _businessRepository.GetAllAsync();
    }

    public async Task<Business?> GetBusinessByIdAsync(int id)
    {
        return await _businessRepository.GetByIdAsync(id);
    }

    public async Task<Business> CreateBusinessAsync(Business business)
    {
        await _businessRepository.AddAsync(business);
        await _unitOfWork.SaveChangesAsync();
        return business;
    }

    public async Task<Business> UpdateBusinessAsync(Business business)
    {
        await _businessRepository.UpdateAsync(business);
        await _unitOfWork.SaveChangesAsync();
        return business;
    }

    public async Task DeleteBusinessAsync(int id)
    {
        await _businessRepository.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
    }
}
