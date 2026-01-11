using Domain.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class UserProfile: AuditableEntity
    {
        public int Id { get; private set; }
        public string UserId { get; private set; } = null!; // الربط مع Identity User
        public string? Bio { get; private set; }
        public string? PhotoPath { get; private set; }

        private UserProfile() { }

        public UserProfile(string userId, string? bio = null, string? photoPath = null)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentNullException(nameof(userId), "User ID cannot be null.");

            UserId = userId;
            Bio = bio;
            PhotoPath = photoPath;
        }

        public void UpdateBio(string? newBio)
        {
            Bio = newBio;
        }

        public string? UpdatePhoto(string? newPhotoPath)
        {
            var oldPath = PhotoPath;
            PhotoPath = newPhotoPath;
            return oldPath; // بنرجعه عشان السيرفيس تعرف تمسح الملف القديم
        }



    }
}
