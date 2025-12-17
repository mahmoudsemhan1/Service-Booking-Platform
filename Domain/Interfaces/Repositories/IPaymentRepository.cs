using System;
using Domain.Models;

namespace Domain.Interfaces.Repositories
{
    public interface IPaymentRepository :IGenericRepository<Payment>
    {
        Task<Payment?> GetByTransactionIdAsync(string transactionId);

    }
}
