using XML_Handler.DB.Repositories;
using XML_Handler.DB.Repositories.Interfaces;
using XML_Handler.DB.UnitOfWork.Interface;

namespace XML_Handler.DB.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        #region Properties

        private DistrictRepository _districtRepository;
        public IDistrictRepository DistrictRepository => _districtRepository ??= new DistrictRepository(_dbContext);


        private ExecutorRepository _executorRepository;
        public IExecutorRepository ExecutorRepository => _executorRepository ??= new ExecutorRepository(_dbContext);


        private SignatoryRepository _signatoryRepository;
        public ISignatoryRepository SignatoryRepository => _signatoryRepository ??= new SignatoryRepository(_dbContext);


        private InvalidsToHeadRepository _invalidsToHeadRepository;
        public IInvalidsToHeadRepository InvalidsToHeadRepository => _invalidsToHeadRepository ??= new InvalidsToHeadRepository(_dbContext);

        #endregion

        private readonly AppContext _dbContext;

        public UnitOfWork(AppContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }
        public async Task SaveAsync() => await _dbContext.SaveChangesAsync();

        private bool _disposed;

        public async ValueTask DisposeAsync()
        {
            await DisposeAsync(true);
            GC.SuppressFinalize(this);
        }

        protected virtual async ValueTask DisposeAsync(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    await _dbContext.DisposeAsync();
                }
                _disposed = true;
            }
        }
    }
}
