using Application.DTOs.Account;
using Application.DTOs.Booking;
using Application.DTOs.Image;
using Application.DTOs.Payment;
using Application.DTOs.providerServiceDto;
using Application.DTOs.Review;
using Application.DTOs.Service;
using Application.DTOs.UserProfile;
using AutoMapper;
using Domain.Models;

namespace Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
             

            //Booking 
            CreateMap<Booking, BookingReadDto>()
                    .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.Service != null ? src.Service.Title : "N/A"))
                    .ForMember(dest => dest.ProviderName, opt => opt.MapFrom(src => src.Provider != null ? src.Provider.BusinessName : "N/A"))
                      .ForMember(dest => dest.UserName, opt => opt.Ignore());

            CreateMap<BookingCreateDto, Booking>();

            CreateMap<BookingUpdateDto, Booking>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));



            // ===== Payment =====

            CreateMap<PaymentCreateDto, Payment>();

            CreateMap<Payment, PaymentReadDto>();
            // ===== services ===
            CreateMap<ServiceCreateDto, Service>()
                 .ForMember(dest => dest.Images, opt => opt.Ignore());
            CreateMap<Service, ServiceReadDto>()
                        .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images));
            CreateMap<Service, ServiceReadDto>();
            //====image====

            CreateMap<Image, ImageReadDto>();
            //  هنا نستخدم الـ Resolver for URL لإنشاء الرابط الكامل للصورة
            CreateMap<Image, ImageReadDto>()
                .ForMember(dest => dest.ImagePath, opt => opt.MapFrom<ImageUrlResolver>());

            //===== ProviderService =====
            CreateMap<Provider, ProviderProfileReadDto>()
            .ForMember(dest => dest.AssignedServices, opt => opt.MapFrom(src => src.ProviderServices));

            CreateMap<Provider, ProviderProfileReadDto>()
            .ForMember(dest => dest.AssignedServices, opt => opt.MapFrom(src => src.ProviderServices));

            CreateMap<ProviderService, ProviderServiceReadDto>()
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.DiscountedPrice, opt => opt.MapFrom(src => src.DiscountedPrice))
                .ForMember(dest => dest.DiscountPercentage, opt => opt.MapFrom(src => src.DiscountPercentage))

                .ForMember(dest => dest.ServiceId, opt => opt.MapFrom(src => src.ServiceId))
                .ForMember(dest => dest.ServiceTitle, opt => opt.MapFrom(src => src.Service!.Title))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Service!.Description))

                .ForMember(dest => dest.PrimaryImageUrl, opt => opt.MapFrom(src =>
                    src.Service!.Images.FirstOrDefault(img => img.IsPrimary) != null
                    ? src.Service.Images.FirstOrDefault(img => img.IsPrimary)!.ImagePath
                    : null));

            //Review 
            CreateMap<Review, ReadReviewDto>()
            .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.Booking!.Service!.Title))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.ToString("yyyy-MM-dd")));
            //userProfile
            // في الـ UserProfile
            CreateMap<UserProfile, UserProfileReadDto>()
                .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom<ImageUrlResolver>());


        }


    }
}
