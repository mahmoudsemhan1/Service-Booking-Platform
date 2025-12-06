using Domain.Models.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Image : AuditableEntity
    {
        public int Id { get; set; }


        public string? EntityType { get; set; } // 'Service', 'Provider', إلخ
        public int? EntityId { get; set; }


        [Required]
        public string ImagePath { get; set; } = null!; // مسار الصورة على الجهاز
        public bool IsPrimary { get; set; } = false;
    }
}
