using Microsoft.AspNetCore.Mvc;
using Store.Repository.Interfaces;
using Store.Repository.Repositories;
using Store.Services.Services.ProductServices.Dtos;
using Store.Services.Services.ProductServices;
using Store.Services.HandlerResponses;
using Store.Services.Services.CacheService;

namespace Store.Web.Extensions
{
    public static class ApplicationServerExtension
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services)
        {
            services.AddScoped<IUnitWork, UnitWork>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICacheService, CacheService>();
            services.AddAutoMapper(typeof(ProductProfile));

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = actionContext =>
                {
                    var errors = actionContext.ModelState.Where(model => model.Value?.Errors.Count > 0)
                    .SelectMany(model => model.Value?.Errors)
                    .Select(error => error.ErrorMessage)
                    .ToList();

                    var errorResponse = new ValidationErrorResponse
                    {
                        Errors = errors
                    };
                    return new BadRequestObjectResult(errorResponse);
                };
            }
            );
            return services;


        }
    }
}
