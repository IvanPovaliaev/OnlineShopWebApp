using AutoMapper;
using Newtonsoft.Json;
using OnlineShop.Application.Models;
using OnlineShop.Application.Models.Admin;
using OnlineShop.Db.Models;
using System.Collections.Generic;

namespace OnlineShopWebApp.Tests.Helpers
{
    public class TestMappingProfile : Profile
    {
        public TestMappingProfile()
        {
            CreateMap<ProductViewModel, Product>()
                .ForMember(destination => destination.SpecificationsJson,
                            option => option.MapFrom(source => JsonConvert.SerializeObject(source.Specifications)))
                .ReverseMap()
                .ForMember(destination => destination.Specifications,
                            option => option.MapFrom(source => JsonConvert.DeserializeObject<Dictionary<string, string>>(source.SpecificationsJson)));

            CreateMap<AddProductViewModel, Product>()
                .ForMember(destination => destination.SpecificationsJson,
                            option => option.MapFrom(source => JsonConvert.SerializeObject(source.Specifications)))
                .ReverseMap()
                .ForMember(destination => destination.Specifications,
                            option => option.MapFrom(source => JsonConvert.DeserializeObject<Dictionary<string, string>>(source.SpecificationsJson)));

            CreateMap<EditProductViewModel, Product>()
                .ForMember(destination => destination.SpecificationsJson,
                            option => option.MapFrom(source => JsonConvert.SerializeObject(source.Specifications)))
                .ReverseMap()
                .ForMember(destination => destination.Specifications,
                            option => option.MapFrom(source => JsonConvert.DeserializeObject<Dictionary<string, string>>(source.SpecificationsJson)));

            CreateMap<CartViewModel, Cart>().ReverseMap();
            CreateMap<CartPositionViewModel, CartPosition>().ReverseMap();

            CreateMap<FavoriteProductViewModel, FavoriteProduct>().ReverseMap();
            CreateMap<ComparisonProductViewModel, ComparisonProduct>().ReverseMap();

            CreateMap<OrderViewModel, Order>().ReverseMap();
            CreateMap<UserDeliveryInfoViewModel, UserDeliveryInfo>().ReverseMap();
            CreateMap<OrderPositionViewModel, OrderPosition>().ReverseMap();

            CreateMap<RoleViewModel, Role>().ReverseMap();
            CreateMap<AddRoleViewModel, Role>().ReverseMap();

            CreateMap<UserViewModel, User>().ReverseMap();
            CreateMap<EditUserViewModel, User>().ReverseMap();
        }
    }
}

