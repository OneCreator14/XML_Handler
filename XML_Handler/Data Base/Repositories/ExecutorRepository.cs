
using XML_Handler.DB.Repositories.Base;
using XML_Handler.DB.Repositories.Interfaces;

namespace XML_Handler.DB.Repositories
{
    internal class ExecutorRepository : BaseRepository<Executor>, IExecutorRepository
    {
        public ExecutorRepository(AppContext dbContext) : base(dbContext) { }

        public async Task<IEnumerable<Executor>> GetTable(string table)
        {
            return await GetManyAsync();

            //DataTable dataTable = new();
            //cmd.CommandText = $"SELECT * FROM {table}";
        }

        public async Task<Executor> GetByNameAsync(string name)
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
