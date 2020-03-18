using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation.AspNetCore;
using Larsson.RESTfulAPIHelper.Test.Infrastructure;
using Larsson.RESTfulAPIHelper.Test.Interface;
using Larsson.RESTfulAPIHelper.Test.PropertyMapping;
using Larsson.RESTfulAPIHelper.Test.Repository;
using Larsson.RESTfulAPIHelper.Test.SortMapping;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Larsson.RESTfulAPIHelper.Test
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DemoContext>(dcob =>
                dcob.UseInMemoryDatabase("TestMemoryDb"));

            services.AddScoped<IProductRepository, ProductRepository>();

            services.AddScoped<DemoContextSeed>();

            var mappingConfig = new MapperConfiguration(ice =>
            {
                // autoMapper: Mapping null strings to string.Empty during mapping
                ice.ValueTransformers.Add<string>(tf => tf ?? string.Empty);

                ice.AddProfile(new PropertyMappings());
            });
            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.AddRESTfulAPIHelper(rho => rho.Register<ProductSortMapping>());

            services.Configure<ApiBehaviorOptions>(abo =>
            {
                abo.SuppressModelStateInvalidFilter = true;
            });

            services.AddDistributedMemoryCache();

            services.AddControllers(mo => mo.ReturnHttpNotAcceptable = true)
               // TODO：System.Text.Json似乎不支持动态类型的属性Camel化
               // .AddJsonOptions(o => {
               //     o.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
               // })
               .AddNewtonsoftJson(options =>
               {
                   options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
               })
               .AddFluentValidation(
                   fvmc => fvmc.RegisterValidatorsFromAssemblyContaining<Startup>().RunDefaultMvcValidationAfterFluentValidationExecutes = false
               );

            FluentValidation.ValidatorOptions.CascadeMode = FluentValidation.CascadeMode.StopOnFirstFailure; // dto validattion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSerilogRequestLogging();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
