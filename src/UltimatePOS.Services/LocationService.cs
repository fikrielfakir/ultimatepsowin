using System.Collections.Generic;
using System.Threading.Tasks;
using UltimatePOS.Core.Entities;
using UltimatePOS.Core.Interfaces;

namespace UltimatePOS.Services;

public class LocationService : ILocationService
{
    private readonly IRepository<Location> _locationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public LocationService(IRepository<Location> locationRepository, IUnitOfWork unitOfWork)
    {
        _locationRepository = locationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Location>> GetLocationsByBusinessIdAsync(int businessId)
    {
        return await _locationRepository.FindAsync(l => l.BusinessId == businessId);
    }

    public async Task<Location?> GetLocationByIdAsync(int id)
    {
        return await _locationRepository.GetByIdAsync(id);
    }

    public async Task<Location> CreateLocationAsync(Location location)
    {
        await _locationRepository.AddAsync(location);
        await _unitOfWork.SaveChangesAsync();
        return location;
    }

    public async Task<Location> UpdateLocationAsync(Location location)
    {
        await _locationRepository.UpdateAsync(location);
        await _unitOfWork.SaveChangesAsync();
        return location;
    }

    public async Task DeleteLocationAsync(int id)
    {
        await _locationRepository.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
    }
}
