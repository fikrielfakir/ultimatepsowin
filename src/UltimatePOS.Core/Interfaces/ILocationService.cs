using System.Collections.Generic;
using System.Threading.Tasks;
using UltimatePOS.Core.Entities;

namespace UltimatePOS.Core.Interfaces;

public interface ILocationService
{
    Task<IEnumerable<Location>> GetLocationsByBusinessIdAsync(int businessId);
    Task<Location?> GetLocationByIdAsync(int id);
    Task<Location> CreateLocationAsync(Location location);
    Task<Location> UpdateLocationAsync(Location location);
    Task DeleteLocationAsync(int id);
}
