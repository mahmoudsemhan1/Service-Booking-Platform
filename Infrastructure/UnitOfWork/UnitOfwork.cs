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
            Services = new GenericRepository<Service>(_context);
             Payments = new GenericRepository<Payment>(_context);
            Reviews = new GenericRepository<Review>(_context);
            Images = new GenericRepository<Image>(_context);
            //services
            Bookings = new BookingRepository(_context);
        }

        // services
        public IBookingRepository Bookings { get; }

        //public IGenericRepository<User> Users { get; }
        public IGenericRepository<Provider> Providers { get; }
        public IGenericRepository<Service> Services { get; }
         public IGenericRepository<Payment> Payments { get; }
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
