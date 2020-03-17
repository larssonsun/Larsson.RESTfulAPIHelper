using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Microsoft.AspNetCore.Routing;
using AutoMapper;
using Larsson.RESTfulAPIHelper.Pagination;
using Larsson.RESTfulAPIHelper.Shaping;
using Larsson.RESTfulAPIHelper.Test.DomainModel;
using Larsson.RESTfulAPIHelper.Test.DTO;
using Larsson.RESTfulAPIHelper.Test.Entity;
using Larsson.RESTfulAPIHelper.Test.Repository;

namespace Larsson.RESTfulAPIHelper.Test.Controller
{
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly LinkGenerator _generator;
        private readonly ProductRepository _repository;

        public TestController(IMapper mapper, LinkGenerator generator, ProductRepository repository)
        {
            _mapper = mapper;
            _generator = generator;
            _repository = repository;
        }

        [HttpPost("test")]
        public async Task<ActionResult<ProductDTO>> GetProductsAsync([FromQuery] ProductQueryDTO productQueryDTO)
        {
            if (productQueryDTO == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            PagedListBase<Product> pagedProducts;
            var projectQuery = _mapper.Map<ProductQuery>(productQueryDTO);
            pagedProducts = await _repository.GetProducts(projectQuery);

            var filterProps = new Dictionary<string, object>();
            filterProps.Add("name", projectQuery.Name);
            filterProps.Add("description", projectQuery.Description);

            Response.SetPaginationHead(pagedProducts, projectQuery, filterProps,
                values => _generator.GetUriByAction(HttpContext, controller: "Product", action: "GetProducts", values: values),
                meta => JsonSerializer.Serialize(meta, new JsonSerializerOptions
                {
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                })
            );

            var mappedProducts = _mapper.Map<IEnumerable<ProductDTO>>(pagedProducts);
            return Ok(mappedProducts.ToDynamicIEnumerable(projectQuery.Fields));
        }
    }
}