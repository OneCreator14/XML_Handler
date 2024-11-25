using XML_Handler.DB.Repositories.Base.Interfaces;

namespace XML_Handler.DB.Repositories.Interfaces
{
    public interface ISignatoryRepository : IBaseRepository<Signatory>
    {
        Task<IEnumerable<Signatory>> GetTable(string table);

        Task<Signatory> GetByNameAsync(string name);
        Task<IEnumerable<string?>> GetNameList();
    }
}
