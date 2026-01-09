using Application.DTOs.Review;
using Domain.Interfaces.Repositories;
using Domain.Models;
using Domain.Models.Views;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class ReviewRepository : GenericRepository<Review>, IReviewRepository
    {
        private readonly AppDbContext _context;

        public ReviewRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProviderReviewView>> GetProviderReviewsAsync(int providerId)
        {
            return await _context.Set<ProviderReviewView>()
                                .Where(r => r.ProviderId == providerId && !r.IsDeleted)
                                .OrderByDescending(r => r.CreatedAt)
                                .ToListAsync();


        }
    }
}
