using AutoMapper;
using Newtonsoft.Json;
using OnlineShop.Db.Models;
using OnlineShopWebApp.Models;
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
        }
    }
}
