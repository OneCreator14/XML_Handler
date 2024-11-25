using XML_Handler.DB.Repositories.Base.Interfaces;

namespace XML_Handler.DB.Repositories.Interfaces
{
    public interface IInvalidsToHeadRepository : IBaseRepository<InvalidsToHead>
    {
        Task<IEnumerable<string?>> GetDistrictWithProcessedInvalids();

        Task<IEnumerable<string?>> GetInvalidsByDistrict(string district);

        Task<InvalidsToHead> GetByNameAsync(string name);
        Task<IEnumerable<InvalidsToHead>> GetListByDistrictAsync(string district);
        Task DelTable();
    }
}