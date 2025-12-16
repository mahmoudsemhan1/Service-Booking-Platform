using Domain.Interfaces.Repositories;
using Domain.Models;
using Domain.Models.Enum;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class BookingRepository : GenericRepository<Booking>, IBookingRepository
    {

        private readonly AppDbContext _context;

        public BookingRepository(AppDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Booking>> GetAsync(
            int? providerId,
            int? serviceId,
            BookingStatus? status,
            DateTime? fromDate,
            DateTime? toDate)
        {
            IQueryable<Booking> query = _context.Bookings
                .Include(b => b.Service)
               .Include(b => b.Provider);

            if (providerId.HasValue)
                query = query.Where(b => b.ProviderId == providerId);

            if (serviceId.HasValue)
                query = query.Where(b => b.ServiceId == serviceId);

            if (status.HasValue)
                query = query.Where(b => b.Status == status);

            if (fromDate.HasValue)
                query = query.Where(b => b.BookingDate >= fromDate);

            if (toDate.HasValue)
                query = query.Where(b => b.BookingDate <= toDate);

            return await query.ToListAsync();

        }
    }
}
