using AutoMapper;
using WebApplication1.Entities;
using WebApplication1.Models;

namespace WebApplication1
{
    public class RestaurantMappingProfile : Profile
    {
        public RestaurantMappingProfile()
        {
            // Jeśli typy i nazwy się zgadzają to AutoMapper automatycznie je zmapuje
            CreateMap<Restaurant, RestaurantDto>()
                .ForMember(m => m.City, c => c.MapFrom(s => s.Address.City))
                .ForMember(m => m.Street, c => c.MapFrom(s => s.Address.Street))
                .ForMember(m => m.PostalCode, c => c.MapFrom(s => s.Address.PostalCode));

            CreateMap<Dish, DishDto>();

            CreateMap<CreateRestaurantDto, Restaurant>()
                .ForMember(r => r.Address, 
                c => c.MapFrom(dto => new Address() 
                { City = dto.City, PostalCode = dto.PostalCode, Street = dto.Street }));
        }
    }
}
