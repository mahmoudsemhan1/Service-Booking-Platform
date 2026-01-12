using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.page
{
    public class PaginationParams
    {
        private const int MaxPageSize = 50; // حماية للسيرفر
        public int PageNumber { get; set; } = 1;

        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : (value <= 0 ? 10 : value);
        }

        // لو حبيت تضيف بحث بالمرة في كل الـ Paginations
        public string? Search { get; set; }
    }
}
