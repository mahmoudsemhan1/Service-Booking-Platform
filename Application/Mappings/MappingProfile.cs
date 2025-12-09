using Application.DTOs.Booking;
using AutoMapper;
using Domain.Models;
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
            CreateMap<Booking, BookingCreateDto>().ReverseMap();
            CreateMap<BookingUpdateDto, Booking>()
                .ForAllMembers(opts => opts.Condition(
                     (src, dest, srcMember) => srcMember != null));
            CreateMap<Booking, BookingReadDto>().ReverseMap();
        }
    }
}
