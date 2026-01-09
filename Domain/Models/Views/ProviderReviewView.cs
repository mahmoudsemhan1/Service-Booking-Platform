using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Views
{
    public class ProviderReviewView
    {
        public int Id { get;private set; }
        public int ProviderId { get; private set; }
        public int Rating { get; private set; }
        public string? Comment { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public string CustomerName { get; private set; } = null!;
        public string ServiceName { get;private set; } = null!;
        public bool IsDeleted { get; private set; }
    }
}
