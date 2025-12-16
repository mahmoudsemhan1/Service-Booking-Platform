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
         IGenericRepository<Provider> Providers { get; }
        IGenericRepository<Service> Services { get; }
         IGenericRepository<Payment> Payments { get; }
        IGenericRepository<Review> Reviews { get; }
        IGenericRepository<Image> Images { get; }

        IBookingRepository Bookings { get; }


        Task<int> CompleteAsync();


        
    }
}
