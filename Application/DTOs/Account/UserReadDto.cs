using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Account
{
    public class UserReadDto
    {
        public string Id { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? PhoneNumber { get; set; }

        // لو حبيت تعرض الـ Roles بتاعة المستخدم (Admin, Provider, Customer)
        public List<string> Roles { get; set; } = new();

        // تاريخ التسجيل (لو موجود في الـ Entity بتاعك)
        public DateTime CreatedAt { get; set; }

        // حالة الحساب (لو حبيت الأدمن يعرف مين مفعل ومين لأ)
        public bool IsEmailConfirmed { get; set; }
    }
}
