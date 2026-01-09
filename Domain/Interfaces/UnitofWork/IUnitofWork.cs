using Domain.Interfaces.Repositories;
using Domain.Models;

namespace Domain.Interfaces.UnitofWork
{
    public interface IUnitofWork : IDisposable
    {
        IGenericRepository<Image> Images { get; }

        IGenericRepository<ProviderService> ProviderServices { get; }
        IGenericRepository<UserProfile> UserProfiles { get; }
        IBookingRepository Bookings { get; }
        IPaymentRepository Payments { get; }

        IProviderRepository Providers { get; }
        IServiceRepository Services { get; }
        IReviewRepository Reviews { get; }

        Task<int> CompleteAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();



    }
}
