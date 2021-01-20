using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Larsson.RESTfulAPIHelper.Interface;
using Larsson.RESTfulAPIHelper.SortAndQuery;
using Larsson.RESTfulAPIHelper.Test.DomainModel;
using Larsson.RESTfulAPIHelper.Test.DTO;
using Larsson.RESTfulAPIHelper.Test.Entity;
using Larsson.RESTfulAPIHelper.Test.Infrastructure;
using Larsson.RESTfulAPIHelper.Test.Interface;
using Microsoft.EntityFrameworkCore;


namespace Larsson.RESTfulAPIHelper.Test.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly DemoContext _context;
        private readonly IPropertyMappingContainer _propertyMappingContainer;

        public ProductRepository(DemoContext context, IPropertyMappingContainer propertyMappingContainer)
        {
            _context = context;
            _propertyMappingContainer = propertyMappingContainer;
        }

        public IEnumerable<Product> GetProductsSync(ProductQuery parameters)
        {
            return new List<Product>{ new Product{
                 PId = Guid.NewGuid(),
                 Name = "Sync projects",
                 Description = "Sync get projects",
                 IsOnSale = false,
                 CreateTime = DateTime.Now
             }};
        }

        public async Task<PagedListBase<Product>> GetProducts(ProductQuery parameters)
        {
            var query = _context.Products.AsQueryable();

            if (!string.IsNullOrEmpty(parameters.Name))
            {
                var name = parameters.Name.Trim().ToLowerInvariant();
                query = query.Where(x => x.Name.ToLowerInvariant() == name);
            }

            if (!string.IsNullOrEmpty(parameters.Description))
            {
                var description = parameters.Description.Trim().ToLowerInvariant();
                query = query.Where(x => x.Description.ToLowerInvariant().Contains(description));
            }

            query = query.ApplySort(parameters.OrderBy, _propertyMappingContainer.Resolve<ProductDTO, Product>());

            var count = await query.CountAsync();
            var data = await query
                .Skip(parameters.PageSize * parameters.PageIndex)
                .Take(parameters.PageSize)
                .AsNoTracking().ToListAsync();

            return new PagedListBase<Product>(parameters.PageIndex, parameters.PageSize, count, data);

        }
    }
}