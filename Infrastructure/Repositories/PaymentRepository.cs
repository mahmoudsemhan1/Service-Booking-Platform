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
    public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
    {
        private readonly AppDbContext _context;
        public PaymentRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Payment?> GetByTransactionIdAsync(string transactionId)
        {
            if (string.IsNullOrEmpty(transactionId))
                return null;

           return await _context.Payments.FirstOrDefaultAsync(p=>p.TransactionId== transactionId);


            

        }
    }
}
