 using Domain.Interfaces.Repositories;
using Domain.Interfaces.UnitofWork;
using Domain.Models;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.UnitOfWork
{
    public class UnitOfwork : IUnitofWork
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction? _currentTransaction;
        public UnitOfwork(AppDbContext context)
        {
            _context = context;

            //Users = new GenericRepository<User>(_context);
            //Providers = new GenericRepository<Provider>(_context);
            Images = new GenericRepository<Image>(_context);
            ProviderServices= new GenericRepository<ProviderService>(_context);
            Bookings = new BookingRepository(_context);
            Payments = new  PaymentRepository(_context);
            Services = new ServiceRepository(_context);
            Providers = new ProviderRepository(_context);
            Reviews = new ReviewRepository(_context);
            UserProfiles = new GenericRepository<UserProfile>(_context);

        }

        public IGenericRepository<Image> Images { get; }
        public IGenericRepository<ProviderService> ProviderServices { get; }
        public IBookingRepository Bookings { get; }
        public IPaymentRepository Payments { get; }
        public IServiceRepository Services { get; }

       public  IProviderRepository Providers { get; }
        public IReviewRepository Reviews { get; }

        public IGenericRepository<UserProfile> UserProfiles { get; }

        public async Task BeginTransactionAsync()
        {
            _currentTransaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await _context.SaveChangesAsync();

                if (_currentTransaction != null)
                {
                    await _currentTransaction.CommitAsync();
                }
            }
            finally
            {
                _currentTransaction?.Dispose();
                _currentTransaction = null; 
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.RollbackAsync();
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task<int> CompleteAsync()
        {
         return await  _context.SaveChangesAsync();
        }
    }
}
