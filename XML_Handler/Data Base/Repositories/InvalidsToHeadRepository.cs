using XML_Handler.DB.Repositories.Base;
using XML_Handler.DB.Repositories.Interfaces;

namespace XML_Handler.DB.Repositories
{
    internal class InvalidsToHeadRepository : BaseRepository<InvalidsToHead>, IInvalidsToHeadRepository
    {
        public InvalidsToHeadRepository(AppContext dbContext) : base(dbContext){ }

        public async Task<IEnumerable<string?>> GetDistrictWithProcessedInvalids()
        {
            return await GetColumn(
                selector: t => t.District!,
                distinct: true);
        }

        public async Task<IEnumerable<string?>> GetInvalidsByDistrict(string district)
        {
            return await GetColumn(
                filter: t => t.District == district,
                selector: t => t.Name!);
        }

        public Task DelTable()
        {
            _dbSet.RemoveRange(_dbSet);
            return Task.CompletedTask;
        }

        public async Task<InvalidsToHead> GetByNameAsync(string name)
        {
            var temp = await GetManyAsync(filter: t => t.Name == name);
            return temp.First();
        }

        public async Task<IEnumerable<InvalidsToHead>> GetListByDistrictAsync(string district)
        {
            var temp = await GetManyAsync(filter: t => t.District == district);
            return temp;
        }
    }
}
