using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Payment
{
    public class PaymentUpdateStatusDto
    {
        public string TransactionId { get; set; } = null!;
        public string? RawResponse { get; set; }
    }
}
