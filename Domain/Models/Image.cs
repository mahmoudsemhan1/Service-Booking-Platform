using Domain.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class Image : AuditableEntity
    {
        public int Id { get; private set; }

        [Required]
        public string ImagePath { get; private set; } = null!; // مسار الصورة على الجهاز
        public bool IsPrimary { get; private set; }

        private Image() { }

        public Image(string imagePath, bool isPrimary = false)
        {

            ImagePath = imagePath;
            IsPrimary = isPrimary;

        }

        public void MarkAsPrimary()
        {
            IsPrimary = true;
        }
        public void UnmarkPrimary()
        {
            IsPrimary = false;
        }

    }
}
