using Application.DTOs.Booking;
using Application.DTOs.Payment;
using Application.DTOs.Service;
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
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //Booking 
            CreateMap<Booking, BookingReadDto>()
                       .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.Service != null ? src.Service.Title : null))
                       .ForMember(dest => dest.ProviderName, opt => opt.MapFrom(src => src.Provider != null ? src.Provider.BusinessName : null));

            CreateMap<BookingCreateDto, Booking>();
            CreateMap<BookingUpdateDto, Booking>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // ===== Payment =====

            CreateMap<PaymentCreateDto, Payment>();

            CreateMap<Payment, PaymentReadDto>();
            // ===== services ===
            CreateMap<ServiceCreateDto, Service>()
            .ConstructUsing(dto =>
            new Service(
                dto.Title,
                dto.Price,
                dto.Description,
                dto.DurationMinutes,
                dto.ImagePath
                              )
                         );

            CreateMap<ServiceUpdateDto, Service>()
                .ForAllMembers(opt => opt.Ignore());

            CreateMap<Service, ServiceReadDto>();



        }
    }
}
