using Allup_Core.Entities;
using Allup_Service.Dtos.CompareDtos;

namespace Allup_Service.Service.IService
{
    public interface ICompareService
    {
        Task<bool> AddToCompareAsync(int id, int count = 1);
        Task<int> CompareCount();
        Task<CompareCardDto> CompareCardVM();
        Task<bool> RemoveFromCompareAsync(int id);
        Task<List<Compare>> GetCompareAsync();
        Task<int> GetIntAsync();
    }
}
