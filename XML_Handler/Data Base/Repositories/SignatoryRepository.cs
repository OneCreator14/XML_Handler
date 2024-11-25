using XML_Handler.DB.Repositories.Base;
using XML_Handler.DB.Repositories.Interfaces;

namespace XML_Handler.DB.Repositories
{
    internal class SignatoryRepository : BaseRepository<Signatory>, ISignatoryRepository
    {
        public SignatoryRepository(AppContext dbContext) : base(dbContext) { }

        public async Task<IEnumerable<Signatory>> GetTable(string table)
        {
            return await GetManyAsync();
        }

        public async Task<Signatory> GetByNameAsync(string name)
        {
            var temp = await GetManyAsync(filter: t => t.Name == name);
            return temp.First();
        }

        public async Task<IEnumerable<string?>> GetNameList()
        {
            var temp = await GetColumn(selector: t => t.Name!);
            return temp;
        }
    }
}
