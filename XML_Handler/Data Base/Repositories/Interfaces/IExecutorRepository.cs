using XML_Handler.DB.Repositories.Base.Interfaces;

namespace XML_Handler.DB.Repositories.Interfaces
{
    public interface IExecutorRepository : IBaseRepository<Executor>
    {
        Task<IEnumerable<Executor>> GetTable(string table);

        Task<Executor> GetByNameAsync(string name);
        Task<IEnumerable<string?>> GetNameList();
    }
}
