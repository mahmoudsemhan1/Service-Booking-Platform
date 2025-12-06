using Domain.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class ProviderService: AuditableEntity
    {
        public int Id { get; set; }


        public int ProviderId { get; set; }
        public Provider? Provider { get; set; }


        public int ServiceId { get; set; }
        public Service? Service { get; set; }
    }
}
