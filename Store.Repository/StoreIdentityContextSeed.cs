using Microsoft.AspNetCore.Identity;
using Store.Data.Entities.IdentityEntities;


namespace Store.Repository
{
    public class StoreIdentityContextSeed
    {
        public static async Task SeedUserAsync(UserManager<AppUser> userManager)
        {
            if (!userManager.Users.Any())
            {
                var user = new AppUser
                {
                    DisplayName = "Salma Sherif",
                    Email = "salmakhalifa06991@gmail.com",
                    UserName = "Salma",
                    Address = new Address
                    {
                        FirstName = "Salma",
                        LastName = "Sherif",
                        City = " Tagamo3",
                        State = "Cairo",
                        Street = "3",
                        PostalCode = "123456"
                    },
                };
                await userManager.CreateAsync(user, "Password123!");
            }
        }
           
    }
}
