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
    public class ImageUrlResolver : IValueResolver<object, object, string>
    {
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ImageUrlResolver(IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            _config = config;
            _httpContextAccessor = httpContextAccessor;
        }

        public string Resolve(object source, object destination, string? destMember, ResolutionContext context)
        {
            //git the relative path based on the source type
            // if i wnat to support more types in the future i can just add more cases here
            string? relativePath = source switch
            {
                UserProfile profile => profile.PhotoPath,
                Domain.Models.Image serviceImage => serviceImage.ImagePath,
                _ => null
            };

            if (string.IsNullOrEmpty(relativePath)) return null;
            //get the request info to build the full URL
            var request = _httpContextAccessor.HttpContext.Request;
            var host = request.Host.Value;
            var scheme = request.Scheme;

            //build and return the full URL
            return $"{scheme}://{host}/{relativePath.Replace("\\", "/")}";
        }
    }
}
