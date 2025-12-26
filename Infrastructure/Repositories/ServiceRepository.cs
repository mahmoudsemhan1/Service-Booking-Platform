using Domain.Interfaces.Repositories;
using Domain.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class ServiceRepository :GenericRepository<Service>, IServiceRepository
    {
        private readonly AppDbContext _context;

        public ServiceRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Service?> GetByIdWithImagesAsync(int id)
        {
            return await _context.Services
                .Include(s => s.Images)
                .FirstOrDefaultAsync(s => s.Id == id);
        }
    }
}
