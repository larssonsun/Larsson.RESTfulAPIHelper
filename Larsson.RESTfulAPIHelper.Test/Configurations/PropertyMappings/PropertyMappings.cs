using AutoMapper;
using Larsson.RESTfulAPIHelper.Test.DomainModel;
using Larsson.RESTfulAPIHelper.Test.DTO;
using Larsson.RESTfulAPIHelper.Test.Entity;

namespace Larsson.RESTfulAPIHelper.Test.PropertyMapping
{
    public class PropertyMappings : Profile
    {
        public PropertyMappings()
        {
            CreateMap<ProductQueryDTO, ProductQuery>();
            CreateMap<Product, ProductDTO>()
            .ForMember(
                dto => dto.FullName,
                opt => opt.MapFrom(entity => $"{entity.Name} {entity.Description}"))
            .ForMember(dto => dto.Id, opt => opt.MapFrom(e => e.PId));

        }
    }
}
