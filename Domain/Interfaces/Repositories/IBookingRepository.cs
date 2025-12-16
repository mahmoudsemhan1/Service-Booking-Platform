using Domain.Models;
using Domain.Models.Enum;

namespace Domain.Interfaces.Repositories
{
    public interface IBookingRepository:IGenericRepository<Booking>
    {

        Task<IEnumerable<Booking>> GetAsync(int? providerId,
        int? serviceId,
        BookingStatus? status,
        DateTime? fromDate,
        DateTime? toDate);
       


    }
}
