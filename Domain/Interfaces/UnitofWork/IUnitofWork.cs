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
        IGenericRepository<Review> Reviews { get; }
        IGenericRepository<Image> Images { get; }

        IBookingRepository Bookings { get; }
        IPaymentRepository Payments { get; }

        IServiceRepository Services { get; }


        Task<int> CompleteAsync();


        
    }
}
