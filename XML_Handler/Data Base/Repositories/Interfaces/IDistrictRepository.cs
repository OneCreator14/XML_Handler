using XML_Handler.DB.Repositories.Base.Interfaces;

namespace XML_Handler.DB.Repositories.Interfaces
{
    public interface IDistrictRepository : IBaseRepository<District>
    {
        Task<IEnumerable<string?>> GetDistrictList();
        Task<District> GetByNameAsync(string name);
    }
}