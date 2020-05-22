using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Microsoft.AspNetCore.Routing;
using AutoMapper;
using Larsson.RESTfulAPIHelper.Pagination;
using Larsson.RESTfulAPIHelper.Shaping;
using Larsson.RESTfulAPIHelper.Caching;
using Larsson.RESTfulAPIHelper.Test.DomainModel;
using Larsson.RESTfulAPIHelper.Test.DTO;
using Larsson.RESTfulAPIHelper.Test.Entity;
using Larsson.RESTfulAPIHelper.Test.Interface;
using Microsoft.Extensions.Caching.Distributed;
using System;
using MessagePack;

namespace Larsson.RESTfulAPIHelper.Test.Controller
{
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly LinkGenerator _generator;
        private readonly IProductRepository _repository;
        private readonly IDistributedCache _cache;

        public TestController(IMapper mapper, LinkGenerator generator, IDistributedCache cache, IProductRepository repository)
        {
            _mapper = mapper;
            _generator = generator;
            _cache = cache;
            _repository = repository;
        }

        [HttpGet("test")]
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

            var projectQuery = _mapper.Map<ProductQuery>(productQueryDTO);

            var cacheKey = $"{nameof(TestController)}_{nameof(GetProductsAsync)}_{Request.QueryString.Value}";

            var gotCache = await _cache.GetCacheAsync<Product>("notExist");
            Console.WriteLine(gotCache is null);

            await _cache.CreateCacheAsync<Product>(
                "notExist",
                async () => await Task<string>.Run(() => { return new Product { Id = Guid.NewGuid() }; }),
                options => options.SetSlidingExpiration(TimeSpan.FromSeconds(15)));

            gotCache = await _cache.GetCacheAsync<Product>("notExist");
            Console.WriteLine(gotCache is null);

            var pagedProducts =
                await _cache.CreateOrGetCacheAsync<PagedListBase<Product>>(cacheKey,
                    async () => await _repository.GetProducts(projectQuery),
                    options => options.SetSlidingExpiration(TimeSpan.FromSeconds(15)));

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