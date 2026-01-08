using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Models
{
    public class Result<T>
    {
      
            public bool IsSuccess { get; set; }
            public T Value { get; set; }    
            public string Error { get; set; }   = string.Empty;
       
    }
}
