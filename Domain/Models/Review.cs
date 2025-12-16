using Domain.Models.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Review : AuditableEntity
    {
        public int Id { get; set; }

        public int ProviderId { get; set; }
        public Provider? Provider { get; set; }


        public string UserId { get; set; } = null!;


        [Range(1, 5)]
        public int Rating { get; set; }
        public string? Comment {  get; set; }
       

    }
}
