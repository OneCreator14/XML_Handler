using Microsoft.EntityFrameworkCore;
using XML_Handler.DB.Repositories.Base;
using XML_Handler.DB.Repositories.Interfaces;

namespace XML_Handler.DB.Repositories
{
    internal class DistrictRepository : BaseRepository<District>, IDistrictRepository
    {
        public DistrictRepository(DbContext dbContext) : base(dbContext) { }

        // список уникальных районов
        public async Task<IEnumerable<string?>> GetDistrictList()
        {
            return await GetColumn(
                selector: t => t.Name!);
                 //distinct: true,
        }

        public async Task<District> GetByNameAsync(string name)
        {
            var temp = await GetManyAsync(filter: t => t.Name == name);
            return temp.First();
        }
    }
}
