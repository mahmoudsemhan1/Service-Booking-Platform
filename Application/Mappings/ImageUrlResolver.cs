using Application.DTOs.Image;
using AutoMapper;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mappings
{
    public class ImageUrlResolver : IValueResolver<Image, ImageReadDto, string>
    {
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ImageUrlResolver(IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            _config = config;
            _httpContextAccessor = httpContextAccessor;
        }

        public string Resolve(Image source, ImageReadDto destination, string destMember, ResolutionContext context)
        {
            if (string.IsNullOrEmpty(source.ImagePath)) return null;

            var request = _httpContextAccessor.HttpContext.Request;
            // بيجمع البروتوكول (http) + الدومين (localhost) + المسار
            return $"{request.Scheme}://{request.Host}/{source.ImagePath.Replace("\\", "/")}";
        }
    }
}
