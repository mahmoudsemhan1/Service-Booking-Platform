using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Account
{
    public class Account
    {
        public record RegisterDto(
         string Email,
         string Password,
         string FullName,
         string bio,
         string Role 
     );

        public record LoginDto(string Email, string Password);

        // النتيجة اللي هترجع للموبايل
        public record AuthResponseDto(
            string Token,
            DateTime ExpiresOn,
            string Message,
            bool IsSuccess
        );
    }
}
