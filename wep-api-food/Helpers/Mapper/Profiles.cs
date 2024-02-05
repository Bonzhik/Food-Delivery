using AutoMapper;
using wep_api_food.Dtos;
using wep_api_food.Models;

namespace wep_api_food.Helpers.Mapper
{
    public class Profiles : Profile
    {
        public Profiles()
        {
            CreateMap<ProductCreateModel, Product>();
            CreateMap<Product, ProductReadModel>();
        }
    }
}
