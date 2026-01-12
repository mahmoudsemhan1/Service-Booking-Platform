using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Paged
{
    public class PagedResultDto<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();   // البيانات (الخدمات مثلاً)
        public int TotalCount { get; set; }      // إجمالي السجلات في الداتابيز
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        // for calculating total pages 
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

        public bool HasPrevious => PageNumber > 1;
        public bool HasNext => PageNumber < TotalPages;
    }
}
