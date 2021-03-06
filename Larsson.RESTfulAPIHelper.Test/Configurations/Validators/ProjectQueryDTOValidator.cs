using FluentValidation;
using Larsson.RESTfulAPIHelper.Interface;
using Larsson.RESTfulAPIHelper.Test.DTO;
using Larsson.RESTfulAPIHelper.Test.Entity;

namespace Larsson.RESTfulAPIHelper.Test.Validator
{
    public class ProjectQueryDTOValidator : AbstractValidator<ProductQueryDTO>
    {
        public ProjectQueryDTOValidator(IPropertyMappingContainer propertyMappingContainer, ITypeHelperService typeHelperService)
        {
            RuleFor(e => e.OrderBy).Must(order => propertyMappingContainer.ValidMappingExistsFor<ProductDTO, Product>(order))
                .WithMessage("Can't find the fields in {PropertyName} for sorting.");

            RuleFor(e => e.Fields).Must(fields => typeHelperService.TypeHasProperties<ProductDTO>(fields))
                .WithMessage("Can't find the fields in {PropertyName} on DTO.");
        }
    }
}