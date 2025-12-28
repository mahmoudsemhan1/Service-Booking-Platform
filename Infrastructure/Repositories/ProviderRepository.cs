using Domain.Interfaces.Repositories;
using Domain.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ProviderRepository : GenericRepository<Provider>, IProviderRepository
    {
        private readonly AppDbContext _context;

        public ProviderRepository(AppDbContext context):base(context)
        {
            _context = context;
        }

        public async Task<Provider?> GetByUserIdWithDetailsAsync(string userId)
        {
            return await _context.Providers
                 .Where(p => p.UserId == userId)
                 .Include(p => p.ProviderServices.Where(ps => !ps.IsDeleted)) // فلترة الخدمات النشطة فقط
                     .ThenInclude(ps => ps.Service)
                 .Include(p => p.Reviews.OrderByDescending(r => r.CreatedAt).Take(10)) // نجيب آخر 5 مراجعات بالمرة
                 .FirstOrDefaultAsync();
        }
    }
}
