using Application.DTOs.Booking;
using Application.DTOs.Identity;
using AutoMapper;
using Domain.Models;
using Domain.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mappings
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            //Booking 
            CreateMap<Booking, BookingReadDto>()
                       .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.Service != null ? src.Service.Title : null))
                       .ForMember(dest => dest.ProviderName, opt => opt.MapFrom(src => src.Provider != null ? src.Provider.BusinessName : null));

            CreateMap<BookingCreateDto, Booking>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => BookingStatus.Pending));

            CreateMap<BookingUpdateDto, Booking>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));


            //// User mappings
            //CreateMap<ApplicationUser, UserReadDto>()
            //    .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
            //    .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName));

            //CreateMap<UserCreateDto, ApplicationUser>()
            //    .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
            //    .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Phone))
            //    .ForMember(dest => dest.Id, opt => opt.Ignore()); // Ignore Id

            //CreateMap<AppUserUpdateDto, ApplicationUser>()
            //    .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
            //    .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
