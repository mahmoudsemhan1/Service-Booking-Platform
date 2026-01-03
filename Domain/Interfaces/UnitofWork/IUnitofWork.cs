using Domain.Interfaces.Repositories;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.UnitofWork
{
    public interface IUnitofWork : IDisposable
    {
        IGenericRepository<Review> Reviews { get; }
        IGenericRepository<Image> Images { get; }
        IGenericRepository<UserProfile> UserProfiles { get; }
        IBookingRepository Bookings { get; }
        IPaymentRepository Payments { get; }

        IProviderRepository Providers { get; }
        IServiceRepository Services { get; }

        Task<int> CompleteAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();



    }
}
