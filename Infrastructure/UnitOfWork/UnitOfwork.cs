 using Domain.Interfaces.Repositories;
using Domain.Interfaces.UnitofWork;
using Domain.Models;
using Infrastructure.Data;
using Infrastructure.Repositories;
 
namespace Infrastructure.UnitOfWork
{
    public class UnitOfwork : IUnitofWork
    {
        private readonly AppDbContext _context;
        public UnitOfwork(AppDbContext context)
        {
            _context = context;

            //Users = new GenericRepository<User>(_context);
            Providers = new GenericRepository<Provider>(_context);
            Reviews = new GenericRepository<Review>(_context);
            Images = new GenericRepository<Image>(_context);
            //services
            Bookings = new BookingRepository(_context);
            Payments = new  PaymentRepository(_context);
            Services = new ServiceRepository(_context);
            provider = new ProviderRepository(_context);
        }

        // services
        public IBookingRepository Bookings { get; }
        public IPaymentRepository Payments {  get; }

        public IServiceRepository Services { get; }

        public IProviderRepository provider { get; }
        // public IProviderRepository ProvidersServices { get; }

        //public IGenericRepository<User> Users { get; }
        public IGenericRepository<Provider> Providers { get; }
        public IGenericRepository<Review> Reviews { get; }
        public IGenericRepository<Image> Images { get; }

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
