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
        IGenericRepository<User> Users { get; }
        IGenericRepository<Provider> Providers { get; }
        IGenericRepository<Service> Services { get; }
        IGenericRepository<Booking> Bookings { get; }
        IGenericRepository<Payment> Payments { get; }
        IGenericRepository<Review> Reviews { get; }
        IGenericRepository<Image> Images { get; }

        Task<int> SaveAsync();

        
    }
}
