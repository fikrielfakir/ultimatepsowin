using System.Collections.Generic;
using System.Threading.Tasks;
using UltimatePOS.Core.Entities;

namespace UltimatePOS.Core.Interfaces;

public interface IBusinessService
{
    Task<IEnumerable<Business>> GetAllBusinessesAsync();
    Task<Business?> GetBusinessByIdAsync(int id);
    Task<Business> CreateBusinessAsync(Business business);
    Task<Business> UpdateBusinessAsync(Business business);
    Task DeleteBusinessAsync(int id);
}
