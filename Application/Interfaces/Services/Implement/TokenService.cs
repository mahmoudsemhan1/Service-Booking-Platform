using Application.Interfaces.Services.TokenService;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace Application.Interfaces.Services.Implement
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _key;


        public TokenService(IConfiguration config)
        {
            _config = config;
            // جلب المفتاح السري من appsettings.json
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Key"]));
        }


        public string CreateToken(string userId, string email, string fullName, IList<string> roles)
        {
            // 1. تحديد الـ Claims (المعلومات المشفرة داخل التوكن)
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim("FullName", fullName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // معرف فريد للتوكن
            };

            // 2. إضافة الأدوار (Roles) للـ Claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // 3. تحديد طريقة التشفير (Signing Credentials)
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256Signature);

            // 4. بناء الـ Token Descriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(Convert.ToDouble(_config["JWT:DurationInDays"])),
                SigningCredentials = creds,
                Issuer = _config["JWT:Issuer"],
                Audience = _config["JWT:Audience"]
            };

            // 5. إنشاء الـ Token ومعالجته
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // تحويله لنص (String) لإرساله للموبايل
            return tokenHandler.WriteToken(token);

        }
    }
}
