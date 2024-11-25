using XML_Handler.DB.Repositories.Base.Interfaces;
using XML_Handler.DB.Repositories.Interfaces;

namespace XML_Handler.DB.UnitOfWork.Interface
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        #region Properties

        IDistrictRepository DistrictRepository { get; }
        IExecutorRepository ExecutorRepository { get; }
        ISignatoryRepository SignatoryRepository { get; }
        IInvalidsToHeadRepository InvalidsToHeadRepository { get; }

        #endregion

        #region Methods

        Task SaveAsync();

        #endregion
    }
}
