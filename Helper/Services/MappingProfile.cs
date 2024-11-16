using AutoMapper;
using YogeshFurnitureAPI.Model;

namespace YogeshFurnitureAPI.Helper.Services
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {

            CreateMap<ProductDTO, Product>();
        }
    }
}
