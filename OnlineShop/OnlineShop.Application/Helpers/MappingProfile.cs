using AutoMapper;
using Newtonsoft.Json;
using OnlineShop.Application.Models;
using OnlineShop.Application.Models.Admin;
using OnlineShop.Db.Models;
using System.Collections.Generic;

namespace OnlineShopWebApp.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ProductViewModel, Product>()
                .ForMember(destination => destination.SpecificationsJson,
                            option => option.MapFrom(source => JsonConvert.SerializeObject(source.Specifications)))
                .ReverseMap()
                .ForMember(destination => destination.Specifications,
                            option => option.MapFrom(source => JsonConvert.DeserializeObject<Dictionary<string, string>>(source.SpecificationsJson)));

            CreateMap<AddProductViewModel, Product>()
                .ForMember(destination => destination.SpecificationsJson,
                            option => option.MapFrom(source => JsonConvert.SerializeObject(source.Specifications)));

            CreateMap<EditProductViewModel, Product>()
                .ForMember(destination => destination.SpecificationsJson,
                            option => option.MapFrom(source => JsonConvert.SerializeObject(source.Specifications)))
                .ReverseMap()
                .ForMember(destination => destination.Specifications,
                            option => option.MapFrom(source => JsonConvert.DeserializeObject<Dictionary<string, string>>(source.SpecificationsJson)));

            CreateMap<ProductImage, ImageViewModel>();

            CreateMap<CartViewModel, Cart>().ReverseMap();
            CreateMap<CartPositionViewModel, CartPosition>().ReverseMap();

            CreateMap<FavoriteProductViewModel, FavoriteProduct>().ReverseMap();
            CreateMap<ComparisonProductViewModel, ComparisonProduct>().ReverseMap();

            CreateMap<OrderViewModel, Order>().ReverseMap();
            CreateMap<UserDeliveryInfoViewModel, UserDeliveryInfo>().ReverseMap();
            CreateMap<OrderPositionViewModel, OrderPosition>().ReverseMap();

            CreateMap<RoleViewModel, Role>().ReverseMap();
            CreateMap<AddRoleViewModel, Role>();

            CreateMap<UserViewModel, User>().ReverseMap();
            CreateMap<User, EditUserViewModel>();
        }
    }
}
