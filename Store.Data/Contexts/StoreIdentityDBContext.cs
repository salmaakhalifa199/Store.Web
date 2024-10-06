using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Store.Data.Entities.IdentityEntities;


namespace Store.Data.Contexts
{
    public class StoreIdentityDBContext : IdentityDbContext<AppUser>
    {
        public StoreIdentityDBContext(DbContextOptions options) : base(options)
        {
            
        }
    }
}
