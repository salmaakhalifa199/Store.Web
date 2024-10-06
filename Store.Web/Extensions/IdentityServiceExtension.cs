using Microsoft.AspNetCore.Identity;
using Store.Data.Contexts;
using Store.Data.Entities.IdentityEntities;
using System.Runtime.CompilerServices;

namespace Store.Web.Extensions
{
    public static class IdentityServiceExtension
    {
        public static IServiceCollection AddIdentityService(this IServiceCollection services)
        {
            var builder = services.AddIdentityCore<AppUser>();

            builder = new IdentityBuilder(builder.UserType , builder.Services);

            builder.AddEntityFrameworkStores<StoreIdentityDBContext>();

            builder.AddSignInManager<SignInManager<AppUser>>();

            services.AddAuthentication();
        
            return services;
        }
    }
}
